#pragma warning disable S107 // Max 8 parameters. Disabled for tests.

namespace Warpstone.Tests.Legacy;

/// <summary>
/// Test class for the <see cref="BasicParsers"/> class.
/// </summary>
public static class BasicParsersTests
{
    public static readonly IEnumerable<object[]> Options = new ParseOptions[][]
    {
        [
            ParseOptions.Default with
            {
                ExecutionMode = ParserExecutionMode.Auto,
                EnableAutomaticGrowingRecursion = true,
                EnableAutomaticMemoization = true,
            }
        ],
        [
            ParseOptions.Default with
            {
                ExecutionMode = ParserExecutionMode.Auto,
                EnableAutomaticGrowingRecursion = false,
                EnableAutomaticMemoization = true,
            }
        ],
        [
            ParseOptions.Default with
            {
                ExecutionMode = ParserExecutionMode.Auto,
                EnableAutomaticGrowingRecursion = true,
                EnableAutomaticMemoization = false,
            }
        ],
        [
            ParseOptions.Default with
            {
                ExecutionMode = ParserExecutionMode.Auto,
                EnableAutomaticGrowingRecursion = false,
                EnableAutomaticMemoization = false,
            }
        ],
        [
            ParseOptions.Default with
            {
                ExecutionMode = ParserExecutionMode.Recursive,
                EnableAutomaticGrowingRecursion = true,
                EnableAutomaticMemoization = true,
            }
        ],
        [
            ParseOptions.Default with
            {
                ExecutionMode = ParserExecutionMode.Recursive,
                EnableAutomaticGrowingRecursion = false,
                EnableAutomaticMemoization = true,
            }
        ],
        [
            ParseOptions.Default with
            {
                ExecutionMode = ParserExecutionMode.Recursive,
                EnableAutomaticGrowingRecursion = true,
                EnableAutomaticMemoization = false,
            }
        ],
        [
            ParseOptions.Default with
            {
                ExecutionMode = ParserExecutionMode.Recursive,
                EnableAutomaticGrowingRecursion = false,
                EnableAutomaticMemoization = false,
            }
        ],
        [
            ParseOptions.Default with
            {
                ExecutionMode = ParserExecutionMode.Iterative,
                EnableAutomaticGrowingRecursion = true,
                EnableAutomaticMemoization = true,
            }
        ],
        [
            ParseOptions.Default with
            {
                ExecutionMode = ParserExecutionMode.Iterative,
                EnableAutomaticGrowingRecursion = false,
                EnableAutomaticMemoization = true,
            }
        ],
        [
            ParseOptions.Default with
            {
                ExecutionMode = ParserExecutionMode.Iterative,
                EnableAutomaticGrowingRecursion = true,
                EnableAutomaticMemoization = false,
            }
        ],
        [
            ParseOptions.Default with
            {
                ExecutionMode = ParserExecutionMode.Iterative,
                EnableAutomaticGrowingRecursion = false,
                EnableAutomaticMemoization = false,
            }
        ],
    };

    /// <summary>
    /// Checks that parsing chars works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void CharParserCorrect(ParseOptions options)
        => AssertThat(Char('x').Parse("xyz", options)).IsEqualTo('x');

    /// <summary>
    /// Checks that parsing chars works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void CharParserIncorrect(ParseOptions options)
        => AssertThat(() => Char('x').Parse("yz", options)).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that parsing strings works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void StringParserCorrect(ParseOptions options)
        => AssertThat(String("abc").Parse("abcdef", options)).IsEqualTo("abc");

    /// <summary>
    /// Checks that parsing strings works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void StringParserIncorrect(ParseOptions options)
        => AssertThat(() => String("abc").Parse("def", options)).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that parsing regexes works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void RegexParserCorrect(ParseOptions options)
        => AssertThat(Regex("[abc]+").Parse("abcabcdef", options)).IsEqualTo("abcabc");

    /// <summary>
    /// Checks that parsing regexes works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void RegexParserEmptyOnEmptyCorrect(ParseOptions options)
        => AssertThat(Regex(@"\s*").Parse("", options)).IsEqualTo("");

    /// <summary>
    /// Checks that parsing regexes works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void RegexParserNonEmptyOnEmptyCorrect(ParseOptions options)
        => AssertThat(Regex(@"\s+").TryParse("", options).Success).IsFalse();

    /// <summary>
    /// Checks that parsing regexes works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void RegexParserIncorrect(ParseOptions options)
        => AssertThat(() => Regex("[abc]+").Parse("def", options)).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that branch parsing works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void OrParserCorrectLeft(ParseOptions options)
        => AssertThat(Or(Char('x'), Char('y')).Parse("xyz", options)).IsEqualTo('x');

    /// <summary>
    /// Checks that branch parsing works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void OrParserCorrectRight(ParseOptions options)
        => AssertThat(Or(Char('x'), Char('y')).Parse("yz", options)).IsEqualTo('y');

    /// <summary>
    /// Checks that branch parsing works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void OrParserCorrect3(ParseOptions options)
        => AssertThat(Or(Char('x'), Char('y'), Char('z')).Parse("z", options)).IsEqualTo('z');

    /// <summary>
    /// Checks that branch parsing works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void OrParserIncorrect(ParseOptions options)
        => AssertThat(() => Or(Char('x'), Char('y')).Parse("zzz", options)).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that parsing one or more works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void OneOrMoreParserCorrectOne(ParseOptions options)
        => AssertThat(OneOrMore(Char('x')).Parse("xyz", options)).ContainsExactly('x');

    /// <summary>
    /// Checks that parsing one or more works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void OneOrMoreParserCorrectMore(ParseOptions options)
        => AssertThat(OneOrMore(Char('x')).Parse("xxxyz", options)).ContainsExactly('x', 'x', 'x');

    /// <summary>
    /// Checks that parsing one or more works correctly with delimiters.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void OneOrMoreParserCorrectMoreDelimited1(ParseOptions options)
        => AssertThat(OneOrMore(Char('x'), Char('y')).Parse("xyxyxzzz", options)).ContainsExactly('x', 'x', 'x');

    /// <summary>
    /// Checks that parsing one or more works correctly with delimiters.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void OneOrMoreParserCorrectMoreDelimited2(ParseOptions options)
        => AssertThat(OneOrMore(Char('x'), Char('y')).Parse("xyxyxy", options)).ContainsExactly('x', 'x', 'x');

    /// <summary>
    /// Checks that parsing one or more works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void OneOrMoreParserIncorrect(ParseOptions options)
        => AssertThat(() => OneOrMore(Char('x')).Parse("yz", options)).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that parsing one or more works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void OneOrMoreParserFollowedByOtherCorrect(ParseOptions options)
    {
        var parser = OneOrMore(Char('x'), Char('y'))
            .ThenSkip(Regex("z+"));
        AssertThat(parser.Parse("xyxyxzzz", options))
            .ContainsExactly('x', 'x', 'x');
    }

    /// <summary>
    /// Checks that parsing many works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void ManyParserCorrectNone(ParseOptions options)
        => AssertThat(Many(Char('x')).Parse("yz", options)).IsEmpty();

    /// <summary>
    /// Checks that parsing many works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void ManyParserCorrectOne(ParseOptions options)
        => AssertThat(Many(Char('x')).Parse("xyz", options)).ContainsExactly('x');

    /// <summary>
    /// Checks that parsing many works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void ManyParserIgnoreLastDelimiter(ParseOptions options)
    {
        var result = Many(Char('x'), Char('y')).TryParse("xyxyxy", options);
        AssertThat(result.Success).IsTrue();
        AssertThat(result.Value).ContainsExactly('x', 'x', 'x');
        AssertThat(result.NextPosition).IsEqualTo(5);
    }

    /// <summary>
    /// Checks that parsing many works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void MultipleParserVerifyMinWithoutDelimiterCorrect(ParseOptions options)
    {
        var result = Multiple(Char('x'), 10, 100).TryParse("xxxxxxxxxx", options);
        AssertThat(result.Success).IsTrue();
        AssertThat(result.Value).ContainsExactly('x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x', 'x');
        AssertThat(result.NextPosition).IsEqualTo(10);
    }

    /// <summary>
    /// Checks that parsing many works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void MultipleParserVerifyMinWithoutDelimiterIncorrect(ParseOptions options)
    {
        var result = Multiple(Char('x'), 10, 100).TryParse("xxxxxxxxx", options);
        AssertThat(result.Success).IsFalse();
    }

    /// <summary>
    /// Checks that parsing many works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void MultipleParserVerifyMinMaxWithoutDelimiterCorrect1(ParseOptions options)
    {
        var result = Multiple(Char('x'), 1, 2).TryParse("x", options);
        AssertThat(result.Success).IsTrue();
        AssertThat(result.Value).ContainsExactly('x');
        AssertThat(result.NextPosition).IsEqualTo(1);
    }

    /// <summary>
    /// Checks that parsing many works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void MultipleParserVerifyMinMaxWithoutDelimiterCorrect2(ParseOptions options)
    {
        var result = Multiple(Char('x'), 1, 2).TryParse("xx", options);
        AssertThat(result.Success).IsTrue();
        AssertThat(result.Value).ContainsExactly('x', 'x');
        AssertThat(result.NextPosition).IsEqualTo(2);
    }

    /// <summary>
    /// Checks that parsing many works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void MultipleParserVerifyMinMaxWithoutDelimiterIncorrect1(ParseOptions options)
    {
        var result = Multiple(Char('x'), 1, 2).TryParse("", options);
        AssertThat(result.Success).IsFalse();
    }

    /// <summary>
    /// Checks that parsing many works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void MultipleParserVerifyMinMaxWithoutDelimiterIncorrect2(ParseOptions options)
    {
        var result = Multiple(Char('x'), 1, 2).ThenEnd().TryParse("xxx", options);
        AssertThat(result.Success).IsFalse();
    }

    /// <summary>
    /// Checks that parsing many works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void ManyParserCorrectMore(ParseOptions options)
        => AssertThat(Many(Char('x')).Parse("xxxyz", options)).ContainsExactly('x', 'x', 'x');

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void ThenParserCorrect(ParseOptions options)
        => AssertThat(Char('x').Then(Char('y')).Parse("xyz", options)).IsEqualTo('y');

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void ThenParserIncorrectLeft(ParseOptions options)
        => AssertThat(() => Char('x').Then(Char('y')).Parse("yz", options)).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void ThenParserIncorrectRight(ParseOptions options)
        => AssertThat(() => Char('x').Then(Char('y')).Parse("xz", options)).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void ThenSkipParserCorrect(ParseOptions options)
        => AssertThat(Char('x').ThenSkip(Char('y')).Parse("xyz", options)).IsEqualTo('x');

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void ThenSkipParserIncorrectLeft(ParseOptions options)
        => AssertThat(() => Char('x').ThenSkip(Char('y')).Parse("yz", options)).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void ThenSkipParserIncorrectRight(ParseOptions options)
        => AssertThat(() => Char('x').ThenSkip(Char('y')).Parse("xz", options)).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void ThenAddParserCorrect2(ParseOptions options)
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .Parse("abcdefghijklmnop", options)).IsEqualTo(('a', 'b'));

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void ThenAddParserCorrect3(ParseOptions options)
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .ThenAdd(Char('c'))
            .Parse("abcdefghijklmnop", options)).IsEqualTo(('a', 'b', 'c'));

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void ThenAddParserCorrect4(ParseOptions options)
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .ThenAdd(Char('c'))
            .ThenAdd(Char('d'))
            .Parse("abcdefghijklmnop", options)).IsEqualTo(('a', 'b', 'c', 'd'));

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void ThenAddParserCorrect5(ParseOptions options)
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .ThenAdd(Char('c'))
            .ThenAdd(Char('d'))
            .ThenAdd(Char('e'))
            .Parse("abcdefghijklmnop", options)).IsEqualTo(('a', 'b', 'c', 'd', 'e'));

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void ThenAddParserCorrect6(ParseOptions options)
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .ThenAdd(Char('c'))
            .ThenAdd(Char('d'))
            .ThenAdd(Char('e'))
            .ThenAdd(Char('f'))
            .Parse("abcdefghijklmnop", options)).IsEqualTo(('a', 'b', 'c', 'd', 'e', 'f'));

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void ThenAddParserCorrect7(ParseOptions options)
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .ThenAdd(Char('c'))
            .ThenAdd(Char('d'))
            .ThenAdd(Char('e'))
            .ThenAdd(Char('f'))
            .ThenAdd(Char('g'))
            .Parse("abcdefghijklmnop", options)).IsEqualTo(('a', 'b', 'c', 'd', 'e', 'f', 'g'));

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void ThenAddParserCorrect8(ParseOptions options)
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .ThenAdd(Char('c'))
            .ThenAdd(Char('d'))
            .ThenAdd(Char('e'))
            .ThenAdd(Char('f'))
            .ThenAdd(Char('g'))
            .ThenAdd(Char('h'))
            .Parse("abcdefghijklmnop", options)).IsEqualTo(('a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'));

    /// <summary>
    /// Checks that parsing transformations works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void TransformParserCorrect1(ParseOptions options)
        => AssertThat(Char('a')
            .Transform(a => $"{a}")
            .Parse("abcdefghijklmnop", options)).IsEqualTo("a");

    /// <summary>
    /// Checks that parsing transformations works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void TransformParserCorrect2(ParseOptions options)
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .Transform((a, b) => $"{a}{b}")
            .Parse("abcdefghijklmnop", options)).IsEqualTo("ab");

    /// <summary>
    /// Checks that parsing transformations works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void TransformParserCorrect3(ParseOptions options)
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .ThenAdd(Char('c'))
            .Transform((a, b, c) => $"{a}{b}{c}")
            .Parse("abcdefghijklmnop", options)).IsEqualTo("abc");

    /// <summary>
    /// Checks that parsing transformations works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void TransformParserCorrect4(ParseOptions options)
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .ThenAdd(Char('c'))
            .ThenAdd(Char('d'))
            .Transform((a, b, c, d) => $"{a}{b}{c}{d}")
            .Parse("abcdefghijklmnop", options)).IsEqualTo("abcd");

    /// <summary>
    /// Checks that parsing transformations works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void TransformParserCorrect5(ParseOptions options)
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .ThenAdd(Char('c'))
            .ThenAdd(Char('d'))
            .ThenAdd(Char('e'))
            .Transform((a, b, c, d, e) => $"{a}{b}{c}{d}{e}")
            .Parse("abcdefghijklmnop", options)).IsEqualTo("abcde");

    /// <summary>
    /// Checks that parsing transformations works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void TransformParserCorrect6(ParseOptions options)
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .ThenAdd(Char('c'))
            .ThenAdd(Char('d'))
            .ThenAdd(Char('e'))
            .ThenAdd(Char('f'))
            .Transform((a, b, c, d, e, f) => $"{a}{b}{c}{d}{e}{f}")
            .Parse("abcdefghijklmnop", options)).IsEqualTo("abcdef");

    /// <summary>
    /// Checks that parsing transformations works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void TransformParserCorrect7(ParseOptions options)
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .ThenAdd(Char('c'))
            .ThenAdd(Char('d'))
            .ThenAdd(Char('e'))
            .ThenAdd(Char('f'))
            .ThenAdd(Char('g'))
            .Transform((a, b, c, d, e, f, g) => $"{a}{b}{c}{d}{e}{f}{g}")
            .Parse("abcdefghijklmnop", options)).IsEqualTo("abcdefg");

    /// <summary>
    /// Checks that parsing transformations works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void TransformParserCorrect8(ParseOptions options)
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .ThenAdd(Char('c'))
            .ThenAdd(Char('d'))
            .ThenAdd(Char('e'))
            .ThenAdd(Char('f'))
            .ThenAdd(Char('g'))
            .ThenAdd(Char('h'))
            .Transform((a, b, c, d, e, f, g, h) => $"{a}{b}{c}{d}{e}{f}{g}{h}")
            .Parse("abcdefghijklmnop", options)).IsEqualTo("abcdefgh");

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void ThenAddParserIncorrectLeft(ParseOptions options)
        => AssertThat(() => Char('x').ThenAdd(Char('y')).Parse("yz", options)).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void ThenAddParserIncorrectRight(ParseOptions options)
        => AssertThat(() => Char('x').ThenAdd(Char('y')).Parse("xz", options)).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that peek parsing works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void PeekParserCorrect(ParseOptions options)
        => AssertThat(Peek(Char('x')).ThenAdd(Char('x')).Parse("xyz", options)).IsEqualTo(('x', 'x'));

    /// <summary>
    /// Checks that peek parsing works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void PeekParserIncorrect(ParseOptions options)
        => AssertThat(() => Peek(Char('x')).Parse("yz", options)).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that end parsing works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void EndParserCorrect(ParseOptions options)
        => AssertThat(End.Parse(string.Empty, options)).IsEqualTo(string.Empty);

    /// <summary>
    /// Checks that end parsing works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void EndParserIncorrect(ParseOptions options)
        => AssertThat(() => End.Parse("yz", options)).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that end parsing works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void ThenEndParserCorrect(ParseOptions options)
        => AssertThat(Char('a').ThenEnd().Parse("a", options)).IsEqualTo('a');

    /// <summary>
    /// Checks that end parsing works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void ThenEndParserIncorrectLeft(ParseOptions options)
        => AssertThat(() => Char('a').ThenEnd().Parse("yz", options)).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that end parsing works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void ThenEndParserIncorrectRight(ParseOptions options)
        => AssertThat(() => Char('a').ThenEnd().Parse("ayz", options)).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that creation parsing works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void CreateParserCorrect(ParseOptions options)
        => AssertThat(Create("abc").Parse(string.Empty, options)).IsEqualTo("abc");

    /// <summary>
    /// Checks that conditional parsing works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void IfParserCorrectTrue(ParseOptions options)
        => AssertThat(If(Char('x'), Char('y'), Char('z')).Parse("xyz", options)).IsEqualTo('y');

    /// <summary>
    /// Checks that conditional parsing works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void IfParserCorrectFalse(ParseOptions options)
        => AssertThat(If(Char('x'), Char('y'), Char('z')).Parse("zd", options)).IsEqualTo('z');

    /// <summary>
    /// Checks that conditional parsing works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void IfParserIncorrect(ParseOptions options)
        => AssertThat(() => If(Char('x'), Char('y'), Char('z')).Parse("g", options)).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that lazy parsing works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void LazyParserCorrect(ParseOptions options)
        => AssertThat(Lazy(() => Char('x')).Parse("xyz", options)).IsEqualTo('x');

    /// <summary>
    /// Checks that the expected parser works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void ExpectedParserCorrect(ParseOptions options)
    {
        IParser<string> parser = Or(String("x").WithName("booboo"), String("z").WithName("bahbah"));
        IParseResult<string> result = parser.TryParse("y", options);
        AssertThat(result.Errors).HasSize(1);
        AssertThat(result.Errors[0]).IsExactlyInstanceOf<UnexpectedTokenError>();
        AssertThat(((UnexpectedTokenError)result.Errors[0]).Expected).ContainsExactly("bahbah", "booboo");
    }

    /// <summary>
    /// Checks that the expected parser works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void StringComparisonCorrect(ParseOptions options)
    {
        IParser<string> parser = String("test-string", StringComparison.InvariantCultureIgnoreCase);
        IParseResult<string> result = parser.TryParse("TeSt-StRiNg", options);
        AssertThat(result.Success).IsTrue();
        AssertThat(result.Value).IsSameAs("TeSt-StRiNg");
    }

    /// <summary>
    /// Checks that the unary not parser works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void UnaryNotTest(ParseOptions options)
    {
        IParser<string?> parser = Not(String("test")).WithName("test");
        IParseResult<string?> result1 = parser.TryParse("test string", options);
        AssertThat(result1.Success).IsFalse();
        IParseResult<string?> result2 = parser.TryParse("other string", options);
        AssertThat(result2.Success).IsTrue();
    }

    /// <summary>
    /// Checks that the binary not parser works correctly.
    /// </summary>
    [Theory]
    [MemberData(nameof(Options))]
    public static void ExceptTest(ParseOptions options)
    {
        IParser<string?> identifier = Regex("[a-zA-Z_][a-zA-Z0-9_]*").Except(Or(String("if"), String("while"), String("for"))).WithName("a keyword");
        AssertThat(identifier.TryParse("while warpstone parser", options).Success).IsFalse();
        AssertThat(identifier.TryParse("for warpstone parser", options).Success).IsFalse();
        AssertThat(identifier.TryParse("if warpstone parser", options).Success).IsFalse();

        AssertThat(identifier.TryParse("the warpstone parser", options).Success).IsTrue();
        AssertThat(identifier.TryParse("fOr warpstone parser", options).Success).IsTrue();
    }
}
