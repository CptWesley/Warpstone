namespace Warpstone.Tests.Expressions;

public static class StackDepth
{
    private static readonly IParser<string> A = String("a");
    private static readonly IParser<string> LeftRecursive = Or(Lazy(() => LeftRecursive!).ThenAdd(A).Transform((x, y) => x + y), A, End);
    private static readonly IParser<string> RightRecursive = Or(A.ThenAdd(Lazy(() => RightRecursive!)).Transform((x, y) => x + y), End);

    private static readonly int[] Counts = [0, 1, 10, 100, 1_000, 10_000, 50_000];

    // Recursive mode cannot parse deep inputs (in Debug, 1 000 right-recursive levels already come close to a 1 MB stack);
    // those are checked in Recursive_mode_throws_before_overflowing.
    public static readonly IEnumerable<object[]> Cases =
    [
        .. TestOptions.All.SelectMany(o => Counts
            .Where(c => o.ExecutionMode != ParserExecutionMode.Recursive || c <= 100)
            .Select(c => new object[] { o, c })),
    ];

    public static readonly IEnumerable<ParseOptions> RecursiveOptions
        = TestOptions.All.Where(o => o.ExecutionMode == ParserExecutionMode.Recursive);

    // Right recursion nests one call per character; left recursion grows in a loop and does not.
    [TestCaseSource(nameof(RecursiveOptions))]
    public static void Recursive_mode_throws_before_overflowing(ParseOptions options)
    {
        var input = new string('a', 1_000_000);
        var parse = () => RightRecursive.TryParse(input, options);
        parse.Should().Throw<InsufficientExecutionStackException>();
    }

    [TestCaseSource(nameof(Cases))]
    public static void Left_recursive(ParseOptions options, int count)
    {
        var input = new string('a', count);
        var output = LeftRecursive.TryParse(input, options);
        output.Errors.Should().BeEmpty();
        output.Success.Should().BeTrue();
        output.Value.Should().Be(input);
    }

    [TestCaseSource(nameof(Cases))]
    public static void Right_recursive(ParseOptions options, int count)
    {
        var input = new string('a', count);
        var output = RightRecursive.TryParse(input, options);
        output.Errors.Should().BeEmpty();
        output.Success.Should().BeTrue();
        output.Value.Should().Be(input);
    }
}
