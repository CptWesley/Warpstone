namespace Warpstone.Tests.Expressions;

/// <summary>
/// Left-recursive grammars from Umeda &amp; Maeda (JIP vol. 29, 2021) and Warth et al.
/// Every rule concatenates the text it matched, so a successful parse returns the input.
/// </summary>
public static class LeftRecursionGrammars
{
    private static readonly ParserExecutionMode[] Modes =
    [
        ParserExecutionMode.Auto,
        ParserExecutionMode.Recursive,
        ParserExecutionMode.Iterative,
    ];

    /// <summary>Automatic growing, with and without automatic memoization, in every execution mode.</summary>
    private static ParseOptions[] AutoOptions
        => Modes.SelectMany(m => new[] { false, true }.Select(memo => ParseOptions.Default with
        {
            ExecutionMode = m,
            EnableAutomaticGrowingRecursion = true,
            EnableAutomaticMemoization = memo,
        })).ToArray();

    /// <summary>Automatic growing and memoization off: only explicit <c>Grow</c> parsers grow.</summary>
    private static ParseOptions[] ManualOptions
        => Modes.Select(m => ParseOptions.Default with
        {
            ExecutionMode = m,
            EnableAutomaticGrowingRecursion = false,
            EnableAutomaticMemoization = false,
        }).ToArray();

    private static IEnumerable<object[]> Cross(ParseOptions[] options, params string[] inputs)
        => options.SelectMany(o => inputs.Select(i => new object[] { o, i }));

    private static IParser<string> S(string value) => String(value);

    private static IParser<string> Seq(params IParser<string>[] parsers)
        => parsers.Aggregate((l, r) => l.ThenAdd(r).Transform((x, y) => x + y));

    private static void ShouldParse(IParser<string> parser, string input, ParseOptions options)
    {
        var result = parser.ThenEnd().TryParse(input, options);
        result.Success.Should().BeTrue();
        result.Value.Should().Be(input);
    }

    private static void ShouldNotParse(IParser<string> parser, string input, ParseOptions options)
        => parser.ThenEnd().TryParse(input, options).Success.Should().BeFalse();

    // S <- S 'a' / 'a'
    private static IParser<string> Direct()
    {
        IParser<string> s = null!;
        s = Or(Seq(Lazy(() => s), S("a")), S("a"));
        return s;
    }

    // Expr <- Expr '+' Num / Num ; Num <- Num [0-9] / [0-9]
    private static IParser<string> ExprNum()
    {
        IParser<string> expr = null!;
        IParser<string> num = null!;
        var digit = Regex("[0-9]");
        num = Or(Seq(Lazy(() => num), digit), digit);
        expr = Or(Seq(Lazy(() => expr), S("+"), num), num);
        return expr;
    }

    // S <- A 'b' / 'b' ; A <- A 'a' / S 'a'
    private static IParser<string> IndirectSA(bool manualLeader)
    {
        IParser<string> s = null!;
        IParser<string> a = null!;
        s = Or(Seq(Lazy(() => a), S("b")), S("b"));
        a = Or(Seq(Lazy(() => a), S("a")), Seq(Lazy(() => s), S("a")));

        // A lies on every cycle (A->A and S->A->S).
        if (manualLeader)
        {
            a = Grow(a);
        }

        return s;
    }

    // S <- A '-' A ; A <- B 'b' / 'b' ; B <- B 'a' / A 'a'
    private static IParser<string> IndirectSAB(bool manualLeader)
    {
        IParser<string> a = null!;
        IParser<string> b = null!;
        a = Or(Seq(Lazy(() => b), S("b")), S("b"));
        b = Or(Seq(Lazy(() => b), S("a")), Seq(Lazy(() => a), S("a")));

        // B lies on every cycle (B->B and A->B->A).
        if (manualLeader)
        {
            b = Grow(b);
        }

        return Seq(a, S("-"), a);
    }

    // Subset of the Java primary grammar from Warth et al.
    private static IParser<string> JavaPrimary()
    {
        IParser<string> primary = null!;
        var classOrInterfaceType = Or(S("C"), S("D"), S("I"), S("J"));
        var identifier = Or(S("x"), S("y"), classOrInterfaceType);
        var methodName = Or(S("m"), S("n"));
        var expression = Or(S("i"), S("j"));
        var classInstanceCreation = Or(
            Seq(S("new"), classOrInterfaceType, S("()")),
            Seq(Lazy(() => primary), S(".new"), identifier, S("()")));
        var methodInvocation = Or(
            Seq(Lazy(() => primary), S("."), methodName, S("()")),
            Seq(methodName, S("()")));
        var fieldAccess = Or(
            Seq(Lazy(() => primary), S("."), identifier),
            Seq(S("super."), identifier));
        var arrayAccess = Or(
            Seq(Lazy(() => primary), S("["), expression, S("]")),
            Seq(identifier, S("["), expression, S("]")));
        primary = Or(classInstanceCreation, methodInvocation, fieldAccess, arrayAccess, S("this"));
        return primary;
    }

    // Interlocking cycles A<->B, B<->C, C<->D: no rule lies on every cycle.
    // A <- B 'x' / 'a' ; B <- A 'z' / C 'y' ; C <- B 'w' / D 'v' ; D <- C 'u'
    private static IParser<string> Interlocking(bool manualLeaders)
    {
        IParser<string> a = null!;
        IParser<string> b = null!;
        IParser<string> c = null!;
        IParser<string> d = null!;
        a = Or(Seq(Lazy(() => b), S("x")), S("a"));
        b = Or(Seq(Lazy(() => a), S("z")), Seq(Lazy(() => c), S("y")));
        c = Or(Seq(Lazy(() => b), S("w")), Seq(Lazy(() => d), S("v")));
        d = Seq(Lazy(() => c), S("u"));

        // No single leader exists; B and C together cover every cycle.
        if (manualLeaders)
        {
            b = Grow(b);
            c = Grow(c);
        }

        return a;
    }

    // E <- '(' E ')' / 'n'
    private static IParser<string> RightRecursive()
    {
        IParser<string> e = null!;
        e = Or(Seq(S("("), Lazy(() => e), S(")")), S("n"));
        return e;
    }

    public static IEnumerable<object[]> DirectData => Cross(AutoOptions, "a", "aa", "aaaa");

    public static IEnumerable<object[]> DirectRejectData => Cross(AutoOptions, "", "b", "aab");

    public static IEnumerable<object[]> ExprNumData => Cross(AutoOptions, "7", "12+34", "1+2+345");

    public static IEnumerable<object[]> IndirectSAData => Cross(AutoOptions, "b", "bab", "baab", "baabab");

    public static IEnumerable<object[]> IndirectSAManualData => Cross(ManualOptions, "b", "bab", "baab", "baabab");

    public static IEnumerable<object[]> IndirectSABData => Cross(AutoOptions, "b-b", "bab-bab", "babab-babab");

    public static IEnumerable<object[]> IndirectSABManualData => Cross(ManualOptions, "b-b", "bab-bab", "babab-babab");

    public static IEnumerable<object[]> JavaPrimaryData
        => Cross(AutoOptions, "this", "this.x", "this.x.y", "this.x.m()", "x[i][j].y", "m()", "super.x");

    public static IEnumerable<object[]> InterlockingData => Cross(AutoOptions, "a", "azx", "azxzx");

    public static IEnumerable<object[]> InterlockingManualData => Cross(ManualOptions, "a", "azx", "azxzx");

    public static IEnumerable<object[]> RightRecursiveData => Cross(AutoOptions, "n", "(n)", "((n))");

    [Theory]
    [MemberData(nameof(DirectData))]
    public static void Direct_left_recursion(ParseOptions options, string input)
        => ShouldParse(Direct(), input, options);

    [Theory]
    [MemberData(nameof(DirectRejectData))]
    public static void Direct_left_recursion_rejects_invalid_input(ParseOptions options, string input)
        => ShouldNotParse(Direct(), input, options);

    [Theory]
    [MemberData(nameof(ExprNumData))]
    public static void Nested_left_recursion_at_different_positions(ParseOptions options, string input)
        => ShouldParse(ExprNum(), input, options);

    [Theory]
    [MemberData(nameof(IndirectSAData))]
    public static void Indirect_left_recursion(ParseOptions options, string input)
        => ShouldParse(IndirectSA(manualLeader: false), input, options);

    [Theory]
    [MemberData(nameof(IndirectSAManualData))]
    public static void Indirect_left_recursion_with_manual_leader(ParseOptions options, string input)
        => ShouldParse(IndirectSA(manualLeader: true), input, options);

    [Theory]
    [MemberData(nameof(IndirectSABData))]
    public static void Indirect_left_recursion_reached_twice(ParseOptions options, string input)
        => ShouldParse(IndirectSAB(manualLeader: false), input, options);

    [Theory]
    [MemberData(nameof(IndirectSABManualData))]
    public static void Indirect_left_recursion_reached_twice_with_manual_leader(ParseOptions options, string input)
        => ShouldParse(IndirectSAB(manualLeader: true), input, options);

    [Theory]
    [MemberData(nameof(JavaPrimaryData))]
    public static void Java_primary(ParseOptions options, string input)
        => ShouldParse(JavaPrimary(), input, options);

    [Theory]
    [MemberData(nameof(InterlockingData))]
    public static void Interlocking_cycles_without_common_rule(ParseOptions options, string input)
        => ShouldParse(Interlocking(manualLeaders: false), input, options); // No leader: Umeda–Maeda fallback.

    [Theory]
    [MemberData(nameof(InterlockingManualData))]
    public static void Interlocking_cycles_with_manual_leaders(ParseOptions options, string input)
        => ShouldParse(Interlocking(manualLeaders: true), input, options);

    [Theory]
    [MemberData(nameof(RightRecursiveData))]
    public static void Right_recursion(ParseOptions options, string input)
        => ShouldParse(RightRecursive(), input, options);

    public static IEnumerable<object[]> AutoOptionsData => AutoOptions.Select(o => new object[] { o });

    [Theory]
    [MemberData(nameof(AutoOptionsData))]
    public static void Right_recursion_is_not_grown(ParseOptions options)
    {
        var count = 0;
        IParser<string> e = null!;
        e = Or(Seq(S("("), Lazy(() => e), S(")")), S("n").Transform(x => { count++; return x; }));

        ShouldParse(e, "((n))", options);
        count.Should().Be(1);
    }

    [Theory]
    [MemberData(nameof(AutoOptionsData))]
    public static void Explicit_grow_combined_with_automatic_growing(ParseOptions options)
    {
        ShouldParse(IndirectSA(manualLeader: true), "baabab", options);
        ShouldParse(IndirectSAB(manualLeader: true), "babab-babab", options);
    }

    [Theory]
    [MemberData(nameof(AutoOptionsData))]
    public static void Explicit_memo_inside_cycle_is_bypassed(ParseOptions options)
    {
        // S <- S 'a' / 'a', memoized by hand.
        IParser<string> direct = null!;
        direct = Memo(Or(Seq(Lazy(() => direct), S("a")), S("a")));
        ShouldParse(direct, "aaaa", options);

        // S <- A 'b' / 'b' ; A <- A 'a' / S 'a', both memoized by hand.
        IParser<string> s = null!;
        IParser<string> a = null!;
        s = Memo(Or(Seq(Lazy(() => a), S("b")), S("b")));
        a = Memo(Or(Seq(Lazy(() => a), S("a")), Seq(Lazy(() => s), S("a"))));
        ShouldParse(s, "baabab", options);
    }

    [Theory]
    [MemberData(nameof(AutoOptionsData))]
    public static void Explicit_memo_inside_leaderless_cycle_is_bypassed(ParseOptions options)
    {
        IParser<string> a = null!;
        IParser<string> b = null!;
        IParser<string> c = null!;
        IParser<string> d = null!;
        a = Memo(Or(Seq(Lazy(() => b), S("x")), S("a")));
        b = Memo(Or(Seq(Lazy(() => a), S("z")), Seq(Lazy(() => c), S("y"))));
        c = Memo(Or(Seq(Lazy(() => b), S("w")), Seq(Lazy(() => d), S("v"))));
        d = Memo(Seq(Lazy(() => c), S("u")));
        ShouldParse(a, "azxzx", options);
    }
}
