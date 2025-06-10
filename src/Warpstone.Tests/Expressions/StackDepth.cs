namespace Warpstone.Tests.Expressions;

public static class StackDepth
{
    private static readonly IParser<string> A = String("a");
    private static readonly IParser<string> LeftRecursive = Or(Lazy(() => LeftRecursive!).ThenAdd(A).Transform((x, y) => x + y), A, End);
    private static readonly IParser<string> RightRecursive = Or(A.ThenAdd(Lazy(() => RightRecursive!)).Transform((x, y) => x + y), End);

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1_000)]
    [InlineData(10_000)]
    [InlineData(50_000)]
    [InlineData(100_000)]
    public static void Left_recursive(int count)
    {
        var input = new string('a', count);
        var output = LeftRecursive.TryParse(input);
        output.Errors.Should().BeEmpty();
        output.Success.Should().BeTrue();
        output.Value.Should().Be(input);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1_000)]
    [InlineData(10_000)]
    [InlineData(50_000)]
    [InlineData(100_000)]
    public static void Right_recursive(int count)
    {
        var input = new string('a', count);
        var output = RightRecursive.TryParse(input);
        output.Errors.Should().BeEmpty();
        output.Success.Should().BeTrue();
        output.Value.Should().Be(input);
    }
}
