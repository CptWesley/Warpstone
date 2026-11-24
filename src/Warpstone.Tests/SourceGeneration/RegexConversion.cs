namespace Warpstone.Tests.SourceGeneration;

/// <summary>
/// Simple regular expressions become scanning loops in generated parsers. They must match exactly what the regex engine matches.
/// </summary>
public static class RegexConversion
{
    private const string Alphabet = "aAbcz_09 \"\t\nxyé٣‍-.";

    private static readonly string Keywords = "(?!(?:class|int|if|else|while|return)\\b)[A-Za-z_][A-Za-z0-9_]*";

    private static readonly object[] Converted =
    [
        "[0-9]+",
        "\"[^\"]*\"",
        "class\\b",
        "[a-z]*",
        "a*b",
        "\\d+\\.\\d*",
        "\\w+",
        "\\s*x",
        "[^a-cy]{2,3}y",
        "\\bfoo",
        "x?y",
        "\\p{L}+",
        "(?!ab)[a-z]+",
        "\\u00e9+",
        ".+",
        "[a-z]+\\b",
        "\\w+\\b",
        "(?:ab)c",
        "\\-\\.",
        Keywords,
    ];

    private static readonly object[] NotConverted =
    [
        "ab|cd",
        "a*a",
        "[a-z]*\\b",
        "(ab)+",
        "a+?",
        "^a",
        "(?=a)a",
        "[^a-c]{2,3}y",
    ];

    [TestCaseSource(nameof(Converted))]
    public static void Converted_patterns_match_like_the_regex_engine(string pattern)
    {
        Regex(pattern).GenerateSource("Generated", "GeneratedParser").Should().NotContain("EnumerateMatches", "'{0}' should become a loop", pattern);
        ShouldMatchRegex(pattern);
    }

    [TestCaseSource(nameof(NotConverted))]
    public static void Other_patterns_keep_the_regex_engine(string pattern)
    {
        Regex(pattern).GenerateSource("Generated", "GeneratedParser").Should().Contain("EnumerateMatches", "'{0}' can't become a loop", pattern);
        ShouldMatchRegex(pattern);
    }

    [Test]
    public static void Patterns_starting_with_a_literal_check_it_before_running_the_regex()
    {
        Regex("abc(?=x)").GenerateSource("Generated", "GeneratedParser").Should().Contain("string.CompareOrdinal(input, pos, \"abc\", 0, 3) != 0");
        ShouldMatchRegex("abc(?=x)");
    }

    private static void ShouldMatchRegex(string pattern)
    {
        var random = new Random(pattern.GetHashCode(StringComparison.Ordinal));
        var inputs = new List<string> { string.Empty, "class", "classy", "class1", "int x", "abc", "\"quoted\"", "12.5", "éé", "a‍b", "xy", "aab" };
        for (var i = 0; i < 300; i++)
        {
            inputs.Add(new string(Enumerable.Range(0, random.Next(0, 8)).Select(_ => Alphabet[random.Next(Alphabet.Length)]).ToArray()));
        }

        foreach (var input in inputs)
        {
            GeneratedParser.ShouldMatchWarpstone($"regex {pattern}", Regex(pattern), input);
        }
    }
}
