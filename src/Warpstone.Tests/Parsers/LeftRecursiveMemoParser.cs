namespace Warpstone.Tests.Parsers;

public static class LeftRecursiveMemoParser
{
    public static readonly IEnumerable<object[]> Options = new ParseOptions[][]
    {
        [
            ParseOptions.Default with
            {
                ExecutionMode = ParserExecutionMode.Auto,
                EnableAutomaticLeftRecursion = false,
                EnableAutomaticMemoization = false,
            }
        ],
        [
            ParseOptions.Default with
            {
                ExecutionMode = ParserExecutionMode.Recursive,
                EnableAutomaticLeftRecursion = false,
                EnableAutomaticMemoization = false,
            }
        ],
        [
            ParseOptions.Default with
            {
                ExecutionMode = ParserExecutionMode.Iterative,
                EnableAutomaticLeftRecursion = false,
                EnableAutomaticMemoization = false,
            }
        ],
    };

    [Theory]
    [MemberData(nameof(Options))]
    public static void DetectInfiniteRecursion(ParseOptions options)
    {
        IParser<string> num = Memo(Regex(@"[0-9]+"));
        IParser<string> exp = null!;
        IParser<string> add = Lazy(() => exp).ThenSkip(Char('+')).ThenAdd(num).Transform((l, r) => $"({l}+{r})");
        exp = LeftRecursive(Or(add, num));

        var result = exp.TryParse("1+2+3", options);
        result.Success.Should().BeTrue();
        result.Value.Should().Be("((1+2)+3)");
    }
}
