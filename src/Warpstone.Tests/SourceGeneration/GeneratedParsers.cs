namespace Warpstone.Tests.SourceGeneration;

/// <summary>
/// Generated parsers give the same results and errors as the Warpstone parsers they were generated from.
/// </summary>
public static class GeneratedParsers
{
    [TestCase("1 + 2 * 3")]
    [TestCase("(1 + 2) * 3")]
    [TestCase("1 + * 2")]
    [TestCase("10-4-3")]
    [TestCase("8/4/2")]
    [TestCase("1+*2")]
    [TestCase("(1+2")]
    [TestCase("")]
    public static void Calculator(string input)
        => GeneratedParser.ShouldMatchWarpstone(nameof(Calculator), Grammars.Calculator, input);

    [TestCase("b")]
    [TestCase("bab")]
    [TestCase("baab")]
    [TestCase("babab")]
    [TestCase("ba")]
    [TestCase("a")]
    public static void IndirectCycle(string input)
        => GeneratedParser.ShouldMatchWarpstone(nameof(IndirectCycle), Grammars.IndirectCycle, input);

    [TestCase("1")]
    [TestCase("12+345+6")]
    [TestCase("1+")]
    [TestCase("+1")]
    public static void NestedLeftRecursion(string input)
        => GeneratedParser.ShouldMatchWarpstone(nameof(NestedLeftRecursion), Grammars.NestedLeftRecursion, input);

    [TestCase("")]
    [TestCase("a=1")]
    [TestCase("a = 1, bc=23,d =4")]
    [TestCase("a=1,")]
    [TestCase("a=x")]
    public static void KeyValues(string input)
        => GeneratedParser.ShouldMatchWarpstone(nameof(KeyValues), Grammars.KeyValues, input);

    [TestCase("let")]
    [TestCase("let!")]
    [TestCase("letter")]
    [TestCase("()")]
    [TestCase("(")]
    [TestCase("1")]
    public static void Keywords(string input)
        => GeneratedParser.ShouldMatchWarpstone(nameof(Keywords), Grammars.Keywords, input);

    [TestCase("SELECT x.x.xSTRASSE")]
    [TestCase("select  xstraße")]
    [TestCase("select x.x.x.x")]
    [TestCase("selectx")]
    public static void Mixed(string input)
        => GeneratedParser.ShouldMatchWarpstone(nameof(Mixed), Grammars.Mixed, input);

    [TestCase("12!")]
    [TestCase("99999999999!")]
    [TestCase("12")]
    public static void Throwing(string input)
        => GeneratedParser.ShouldMatchWarpstone(nameof(Throwing), Grammars.Throwing, input, "System.Globalization");

    [TestCase("abc-21-x")]
    [TestCase("abc-21-x!!")]
    [TestCase("abc-21-x!")]
    [TestCase("abc-99999999999-x")]
    public static void Lambdas(string input)
        => GeneratedParser.ShouldMatchWarpstone(nameof(Lambdas), Grammars.Lambdas, input);

    [TestCase("abc.")]
    [TestCase("abc;")]
    [TestCase("abc")]
    public static void Memoized(string input)
        => GeneratedParser.ShouldMatchWarpstone(nameof(Memoized), Grammars.Memoized, input);

    [Test]
    public static void Deep_input_throws_instead_of_overflowing_the_stack()
    {
        var parse = GeneratedParser.Compile(nameof(Calculator), Grammars.Calculator);

        var act = () => parse(new string('(', 100_000) + "1" + new string(')', 100_000));

        act.Should().Throw<TargetInvocationException>().WithInnerException<InsufficientExecutionStackException>();
    }

    [Test]
    public static void TryParse_without_error_parses_once()
    {
        var source = Grammars.Calculator.GenerateSource("Generated", "GeneratedParser");
        var tryParse = GeneratedParser.Load(source).GetMethods().Single(m => m.Name == "TryParse" && m.GetParameters().Length == 2);

        object?[] good = ["1 + 2 * 3", null];
        object?[] bad = ["1 + * 2", null];

        ((bool)tryParse.Invoke(null, good)!).Should().BeTrue();
        good[1].Should().Be(7);
        ((bool)tryParse.Invoke(null, bad)!).Should().BeFalse();
    }

    [Test]
    public static void Bind_is_not_supported()
    {
        var parser = Char('a').Then(c => Char(c));

        var act = () => parser.GenerateSource("Generated", "GeneratedParser");

        act.Should().Throw<NotSupportedException>().WithMessage("*BindParser*");
    }

    [Test]
    public static void AsResult_is_not_supported()
    {
        var act = () => Char('a').AsResult().GenerateSource("Generated", "GeneratedParser");

        act.Should().Throw<NotSupportedException>().WithMessage("*AsResultParser*");
    }

    [Test]
    public static void Transformations_capturing_variables_are_not_supported()
    {
        var offset = 1;
        var parser = Regex("[0-9]").Transform(s => int.Parse(s, System.Globalization.CultureInfo.InvariantCulture) + offset);

        var act = () => parser.GenerateSource("Generated", "GeneratedParser");

        act.Should().Throw<NotSupportedException>().WithMessage("*local variables*");
    }

    [Test]
    public static void Values_without_a_literal_are_not_supported()
    {
        var act = () => Create(new List<int>()).GenerateSource("Generated", "GeneratedParser");

        act.Should().Throw<NotSupportedException>().WithMessage("*literal*");
    }

    [Test]
    public static void Literal_values_are_written_back()
    {
        var parser = Create(-2.5).ThenAdd(Create('\n')).ThenAdd(Create("a\"b")).ThenAdd(Create(StringComparison.Ordinal)).ThenAdd(Create<int?>(3)).ThenAdd(Create(ulong.MaxValue));

        var result = GeneratedParser.CompileSource(parser.GenerateSource("Generated", "GeneratedParser"))(string.Empty);

        result.Value.Should().Be((-2.5, '\n', "a\"b", StringComparison.Ordinal, (int?)3, ulong.MaxValue));
    }
}
