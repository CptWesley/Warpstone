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

    // More distinct words than the intern table has slots, so slots collide and get overwritten; plus words too long to intern.
    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public static void Returns_matched_text_when_interned_matches_collide(ParseOptions options)
    {
        var words = Enumerable.Range(0, 2000).Select(i => "w" + i).Append(new string('l', 40)).ToArray();
        var input = string.Join(' ', words.SelectMany(w => new[] { w, w }));

        Many(Regex("[a-z0-9]+"), Char(' ')).Parse(input, options).Should().Equal(words.SelectMany(w => new[] { w, w }));
    }

    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public static void Repeated_short_matches_share_one_string(ParseOptions options)
    {
        var result = Many(Regex("[a-z]+"), Char(' ')).Parse("foo bar bar", options);

#if NET9_0_OR_GREATER
        result[2].Should().BeSameAs(result[1]);
#else
        result[2].Should().Be(result[1]);
#endif
    }
}
