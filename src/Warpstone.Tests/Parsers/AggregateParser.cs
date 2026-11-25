namespace Warpstone.Tests.Parsers;

/// <summary>
/// <c>Aggregate</c> (<c>Many</c>, <c>Multiple</c>) counts and delimiters, the same in every execution mode.
/// </summary>
public static class AggregateParser
{
    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public static void Respects_min_and_max_count(ParseOptions options)
    {
        var parser = Multiple(Char('x'), Char(','), 2, 3).ThenAdd(Regex(".*"));

        parser.TryParse("x", options).Success.Should().BeFalse();
        parser.TryParse("x,y", options).Success.Should().BeFalse();
        parser.Parse("x,x", options).Should().BeEquivalentTo((new[] { 'x', 'x' }, string.Empty));
        parser.Parse("x,x,", options).Should().BeEquivalentTo((new[] { 'x', 'x' }, ","));
        parser.Parse("x,x,x,x", options).Should().BeEquivalentTo((new[] { 'x', 'x', 'x' }, ",x"));
    }

    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public static void Zero_count_parses_nothing(ParseOptions options)
        => Multiple(Char('x'), 0).ThenAdd(Regex(".*")).Parse("xx", options).Should().BeEquivalentTo((Array.Empty<char>(), "xx"));

    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public static void Nested_repetitions_keep_their_own_state(ParseOptions options)
    {
        var parser = Many(OneOrMore(Char('x'), Char(',')), Char(';')).Transform(rows => string.Join("|", rows.Select(r => r.Count)));

        parser.Parse("x,x;x;x,x,x", options).Should().Be("2|1|3");
        parser.Parse("x;x,x", options).Should().Be("1|2");
    }

    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public static void Nested_repetitions_of_the_same_type_keep_their_own_elements(ParseOptions options)
    {
        // Repetitions of one element type share a scratch stack during a parse.
        IParser<string> item = null!;
        item = Or(
            Char('[').Then(Many(Lazy(() => item))).ThenSkip(Char(']')).Transform(xs => "[" + string.Join(",", xs) + "]"),
            Regex("[a-z]"));

        Many(item).Parse("a[b[c]d]e", options).Should().Equal("a", "[b,[c],d]", "e");
        Many(item).Parse("[[]]", options).Should().Equal("[[]]");
    }

    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public static void Failed_repetition_drops_its_elements(ParseOptions options)
    {
        // The inner repetition collects 'a' and 'b', then fails for want of a third letter. The outer one must not see them.
        var letter = Regex("[a-z]");
        var parser = Many(Or(Multiple(letter, 3, 3).Transform(xs => string.Concat(xs)), letter));

        parser.Parse("ab", options).Should().Equal("a", "b");
        parser.Parse("abcd", options).Should().Equal("abc", "d");
        Multiple(letter, Char(','), 2, 3).TryParse("a,", options).Success.Should().BeFalse();
    }
}
