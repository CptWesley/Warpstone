namespace Warpstone.Tests.Parsers;

/// <summary>
/// <see cref="FailureTracking.OnFailure"/> parses failing input again to report its errors, which are the same as with <see cref="FailureTracking.Always"/>.
/// </summary>
public static class FailureTrackingModes
{
    private static readonly ParseOptions[] Modes =
    [
        .. new[] { ParserExecutionMode.Auto, ParserExecutionMode.Recursive, ParserExecutionMode.Iterative }
            .Select(mode => ParseOptions.Default with { ExecutionMode = mode }),
    ];

    private static readonly IParser<int> Sum = CreateSum();

    private static IEnumerable<object[]> Cases()
        => from options in Modes
           from input in new[] { "1+2", "1+", "+1", "1+x", "99999999999+1", "abc", string.Empty }
           select new object[] { options, input };

    [TestCaseSource(nameof(Cases))]
    public static void Errors_are_the_same_in_both_modes(ParseOptions options, string input)
    {
        var always = Sum.TryParse(input, options with { FailureTracking = FailureTracking.Always });
        var onFailure = Sum.TryParse(input, options with { FailureTracking = FailureTracking.OnFailure });

        onFailure.Success.Should().Be(always.Success);
        onFailure.Errors.Select(Describe).Should().Equal(always.Errors.Select(Describe));
    }

    [TestCaseSource(nameof(Modes))]
    public static void On_failure_tracking_runs_transformations_of_failing_input_twice(ParseOptions options)
    {
        var calls = 0;
        var parser = Char('a').Transform(c => ++calls).ThenEnd();

        parser.TryParse("ab", options with { FailureTracking = FailureTracking.OnFailure }).Success.Should().BeFalse();
        calls.Should().Be(2);

        calls = 0;
        parser.TryParse("ab", options with { FailureTracking = FailureTracking.Always }).Success.Should().BeFalse();
        calls.Should().Be(1);

        calls = 0;
        parser.TryParse("a", options with { FailureTracking = FailureTracking.OnFailure }).Success.Should().BeTrue();
        calls.Should().Be(1);
    }

    [TestCaseSource(nameof(Modes))]
    public static void Only_tracking_contexts_track(ParseOptions options)
    {
        ParseContext.Create("1", Sum, options with { FailureTracking = FailureTracking.OnFailure }).Failures.Enabled.Should().BeFalse();
        ParseContext.Create("1", Sum, options with { FailureTracking = FailureTracking.Always }).Failures.Enabled.Should().BeTrue();
    }

    [TestCaseSource(nameof(Modes))]
    public static void Grammars_with_results_or_binds_always_track(ParseOptions options)
    {
        var onFailure = options with { FailureTracking = FailureTracking.OnFailure };

        ParseContext.Create("a", Char('a').AsResult(), onFailure).Failures.Enabled.Should().BeTrue();
        ParseContext.Create("aa", Char('a').Then(c => Char(c)), onFailure).Failures.Enabled.Should().BeTrue();
    }

    [TestCaseSource(nameof(Modes))]
    public static void Inner_results_keep_their_errors(ParseOptions options)
    {
        var parser = Char('a').ThenAdd(Char('b').AsResult()).ThenSkip(Or(Char('c'), Char('x')));

        var result = parser.TryParse("ax", options with { FailureTracking = FailureTracking.OnFailure });

        result.Success.Should().BeTrue();
        result.Value.Right.Success.Should().BeFalse();
        result.Value.Right.Errors.Should().ContainSingle().Which.Should().BeOfType<UnexpectedTokenError>().Which.Expected.Should().Equal("'b'");
    }

    [TestCaseSource(nameof(Modes))]
    public static async Task Asynchronous_readers_parse_again_from_what_they_read(ParseOptions options)
    {
        var input = "1+2+x";
        var expected = Sum.TryParse(input, options with { FailureTracking = FailureTracking.Always });

        var actual = await Sum.TryParseAsync(ParseInput.CreateFromMemory(new System.IO.StringReader(input)), options with { FailureTracking = FailureTracking.OnFailure });

        actual.Success.Should().BeFalse();
        actual.Errors.Select(Describe).Should().Equal(expected.Errors.Select(Describe));
    }

    private static IParser<int> CreateSum()
    {
        var number = Regex("[0-9]+").Transform(s => int.Parse(s, System.Globalization.CultureInfo.InvariantCulture)).WithName("number");
        IParser<int> sum = null!;
        sum = Or(Lazy(() => sum).ThenSkip(Char('+')).ThenAdd(number).Transform((l, r) => l + r), number);
        return sum.ThenEnd();
    }

    private static string Describe(IParseError error)
        => error switch
        {
            UnexpectedTokenError unexpected => $"{unexpected.Position}: {string.Join(", ", unexpected.Expected)}",
            _ => $"{error.Position}: {error.GetType().Name}",
        };
}
