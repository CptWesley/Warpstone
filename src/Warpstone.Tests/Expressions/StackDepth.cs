namespace Warpstone.Tests.Expressions;

public static class StackDepth
{
    private static readonly IParser<string> A = String("a");
    private static readonly IParser<string> LeftRecursive = Or(Lazy(() => LeftRecursive!).ThenAdd(A).Transform((x, y) => x + y), A, End);
    private static readonly IParser<string> RightRecursive = Or(A.ThenAdd(Lazy(() => RightRecursive!)).Transform((x, y) => x + y), End);

    private static readonly int[] Counts = [0, 1, 10, 100, 1_000, 10_000, 50_000];

    // Recursive mode has no stack guard: deep inputs overflow the stack by design, so it only gets the shallow counts.
    public static readonly IEnumerable<object[]> Cases =
    [
        .. TestOptions.All.SelectMany(o => Counts
            .Where(c => o.ExecutionMode != ParserExecutionMode.Recursive || c <= 1_000)
            .Select(c => new object[] { o, c })),
    ];

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
