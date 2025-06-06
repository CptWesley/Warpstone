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
        IParser<string> exp = null!;
        IParser<string> add = Memo(() => exp).ThenSkip(Char('+')).ThenAdd(Memo(() => exp)).Transform((l, r) => $"({l}+{r})");
        exp = add;

        var result = exp.TryParse("1+2+3", options);
        result.Success.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Should().BeOfType<InfiniteRecursionError>();
    }
}
