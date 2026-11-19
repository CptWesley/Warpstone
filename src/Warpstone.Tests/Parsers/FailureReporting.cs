namespace Warpstone.Tests.Parsers;

/// <summary>
/// A failed parse reports what was expected at the furthest position any parser reached.
/// </summary>
public static class FailureReporting
{
    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public static void Merges_everything_expected_at_the_furthest_position(ParseOptions options)
    {
        var parser = Many(Char('x')).ThenSkip(Char(';'));

        var error = SingleError(parser.TryParse("xxy", options));

        error.Position.Should().Be(2);
        error.Expected.Should().BeEquivalentTo("'x'", "';'");
    }

    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public static void Name_replaces_failures_at_its_start(ParseOptions options)
    {
        var parser = Char('a').Then(Char('b')).WithName("ab");

        var error = SingleError(parser.TryParse("x", options));

        error.Position.Should().Be(0);
        error.Expected.Should().BeEquivalentTo("ab");
    }

    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public static void Name_keeps_failures_that_got_further(ParseOptions options)
    {
        var parser = Char('a').Then(Char('b')).WithName("ab");

        var error = SingleError(parser.TryParse("ac", options));

        error.Position.Should().Be(1);
        error.Expected.Should().BeEquivalentTo("'b'");
    }

    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public static void Failures_inside_a_negative_lookahead_are_not_errors(ParseOptions options)
    {
        var parser = Not(Char('x')).Then(Char('y'));

        var error = SingleError(parser.TryParse("z", options));

        error.Position.Should().Be(0);
        error.Expected.Should().BeEquivalentTo("'y'");
    }

    private static UnexpectedTokenError SingleError<T>(IParseResult<T> result)
    {
        result.Success.Should().BeFalse();
        return result.Errors.Should().ContainSingle().Which.Should().BeOfType<UnexpectedTokenError>().Subject;
    }
}
