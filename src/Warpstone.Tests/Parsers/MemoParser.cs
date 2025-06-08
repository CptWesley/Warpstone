namespace Warpstone.Tests.Parsers;

public static class MemoParser
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
        IParser<string> add = null!;
        add = Memo(Lazy(() => add).ThenSkip(Char('+')).ThenAdd(num).Transform((l, r) => $"({l}+{r})"));

        var result = add.TryParse("1+2+3", options);
        result.Success.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Should().BeOfType<InfiniteRecursionError>();
    }
}
