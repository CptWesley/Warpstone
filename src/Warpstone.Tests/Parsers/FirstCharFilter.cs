using Warpstone.Errors;

namespace Warpstone.Tests.Parsers;

/// <summary>
/// On string input, <c>Or</c> skips alternatives that can't start with the next character and reports their failures without running them.
/// Reader input never skips, so both must give the same results and the same errors, in the same order.
/// </summary>
public static class FirstCharFilter
{
    private static readonly string[] Inputs =
        ["", "if", "iff", "42", "\"s\"", "(x)", "(y)", "+=", "+", "AB", "ab", "-", "ü", "é9", " ", "x y", "ax", "bx", "(", "]", "z"];

    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public static void Mixed_alternatives_behave_as_without_the_filter(ParseOptions options)
    {
        var grammar = Or(
            Regex(@"if\b"),
            Regex("[0-9]+"),
            Or(Regex("\"[^\"]*\""), Char('(').Then(String("x")).ThenSkip(Char(')'))),
            String("+=").WithName("plus-assign"),
            Regex(@"(?!if\b)[A-Za-z_][A-Za-z0-9_]*"),
            String("ab", ignoreCase: true),
            String("-"),
            Regex(@"\w+"),
            String("+").Transform(x => x + "!"))
            .ThenEnd();

        foreach (var input in Inputs)
        {
            Describe(grammar.TryParse(input, options)).Should().Be(Describe(grammar.TryParse(Reader(input), options)), input);
        }
    }

    [TestCase("[a-c]x")]
    [TestCase(@"\(")]
    [TestCase(@"(?!b)[a-z]")]
    [TestCase("(?=a)[ab]")]
    [TestCase("a*b")]
    [TestCase("a{0,1}b")]
    [TestCase("a+b")]
    [TestCase("[^a]")]
    [TestCase("a|b")]
    [TestCase("[a-]")]
    [TestCase("[-a]")]
    [TestCase(@"[\]a]")]
    [TestCase(" x")]
    [TestCase("é")]
    [TestCase(@"\w")]
    public static void Regex_first_characters_behave_as_without_the_filter(string pattern)
    {
        var grammar = Or(Regex(pattern), String("zzz"), Char('-').Transform(c => c.ToString())).ThenEnd();

        foreach (var options in TestOptions.All)
        {
            foreach (var input in Inputs.Concat(["a", "b", "c", "ab", "bb", " x", "é", "_"]))
            {
                Describe(grammar.TryParse(input, options)).Should().Be(Describe(grammar.TryParse(Reader(input), options)), $"{pattern} on '{input}'");
            }
        }
    }

    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public static void Reports_skipped_alternatives_in_order(ParseOptions options)
    {
        var parser = Or(String("a"), String("bc"), String("d").WithName("D"), String("bx"));

        var error = parser.TryParse("by", options).Errors.Should().ContainSingle().Which.Should().BeOfType<UnexpectedTokenError>().Subject;

        error.Expected.Should().Equal("\"a\"", "\"bc\"", "D", "\"bx\"");
    }

    private static IParseInput Reader(string input)
        => ParseInput.CreateFromMemory(new StringReader(input));

    private static string Describe(IParseResult result)
        => result.Success
        ? $"ok {result.Length} {result.Value}"
        : string.Join(" | ", result.Errors.Select(e => e is UnexpectedTokenError u
            ? $"{e.GetType().Name}@{e.Position}: {string.Join(", ", u.Expected)} ({u.InnerErrors.Count})"
            : $"{e.GetType().Name}@{e.Position}"));
}
