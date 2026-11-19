namespace Warpstone.Tests.Parsers;

public static class RegexParser
{
    // A regex that starts with literal text is first checked against that text; these pin that the check never rejects a match.
    [TestCase("ab*c", "ac")]
    [TestCase("ab+c", "abbc")]
    [TestCase("ab?", "a")]
    [TestCase("ab{0,1}c", "ac")]
    [TestCase("ab*?c", "ac")]
    [TestCase("if|while", "while")]
    [TestCase("(?i)abc", "ABC")]
    [TestCase(@"if\b", "if")]
    [TestCase("a_1", "a_1")]
    public static void Matches_patterns_that_start_with_literal_text(string pattern, string input)
    {
        foreach (var options in TestOptions.All)
        {
            Regex(pattern).Parse(input, options).Should().Be(input);
        }
    }

    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public static void Ignore_case_option_matches_other_case(ParseOptions options)
        => Regex("abc", RegexOptions.IgnoreCase).Parse("ABC", options).Should().Be("ABC");

    [TestCase(@"if\b", "iff")]
    [TestCase("abc", "ab")]
    [TestCase("abc", "abd")]
    [TestCase("abc", "")]
    public static void Rejects_input_without_the_literal_text(string pattern, string input)
    {
        foreach (var options in TestOptions.All)
        {
            Regex(pattern).TryParse(input, options).Success.Should().BeFalse();
        }
    }

    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public static void Returns_each_matched_text(ParseOptions options)
        => Many(Regex("[a-z]+"), Char(' ')).Parse("foo foo bar foo", options).Should().Equal("foo", "foo", "bar", "foo");
}
