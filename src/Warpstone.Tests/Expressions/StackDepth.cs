namespace Warpstone.Tests.Expressions;

[TestFixture(true)]
[TestFixture(false)]
public sealed class StackDepth(bool memo)
{
    private readonly ParseOptions options = ParseOptions.Default with { EnableAutomaticMemoization = memo };

    private static readonly IParser<string> A = String("a");
    private static readonly IParser<string> LeftRecursive = Or(Lazy(() => LeftRecursive!).ThenAdd(A).Transform((x, y) => x + y), A, End);
    private static readonly IParser<string> RightRecursive = Or(A.ThenAdd(Lazy(() => RightRecursive!)).Transform((x, y) => x + y), End);

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(100)]
    [TestCase(1_000)]
    [TestCase(10_000)]
    [TestCase(50_000)]
    public void Left_recursive(int count)
    {
        var input = new string('a', count);
        var output = LeftRecursive.TryParse(input, options);
        output.Errors.Should().BeEmpty();
        output.Success.Should().BeTrue();
        output.Value.Should().Be(input);
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(100)]
    [TestCase(1_000)]
    [TestCase(10_000)]
    [TestCase(50_000)]
    public void Right_recursive(int count)
    {
        var input = new string('a', count);
        var output = RightRecursive.TryParse(input, options);
        output.Errors.Should().BeEmpty();
        output.Success.Should().BeTrue();
        output.Value.Should().Be(input);
    }
}
