namespace Warpstone.Tests;

/// <summary>Every execution mode, with and without automatic memoization, with automatic growing on.</summary>
public static class TestOptions
{
    public static readonly ParseOptions[] All =
    [
        .. new[] { ParserExecutionMode.Auto, ParserExecutionMode.Recursive, ParserExecutionMode.Iterative }
            .SelectMany(mode => new[] { true, false }.Select(memo => ParseOptions.Default with
            {
                ExecutionMode = mode,
                EnableAutomaticGrowingRecursion = true,
                EnableAutomaticMemoization = memo,
            })),
    ];
}
