namespace Warpstone.Tests.Expressions;

public static class AutomaticMemoization
{
    public static readonly ParseOptions[] Options =
    [
        .. new[] { ParserExecutionMode.Auto, ParserExecutionMode.Recursive, ParserExecutionMode.Iterative }
            .Select(mode => ParseOptions.Default with { ExecutionMode = mode, EnableAutomaticMemoization = true }),
    ];

    [TestCaseSource(nameof(Options))]
    public static void Memoizes_parsers_retried_after_backtracking(ParseOptions options)
    {
        // Every alternative starts with the same term, so without memoization each nesting level triples the work.
        IParser<int> expr = null!;
        var term = Or(Char('(').Then(Lazy(() => expr)).ThenSkip(Char(')')), Char('1').Transform(_ => 1));
        expr = Or(
            term.ThenSkip(Char('+')).ThenAdd(Lazy(() => expr)).Transform((a, b) => a + b),
            term.ThenSkip(Char('-')).ThenAdd(Lazy(() => expr)).Transform((a, b) => a - b),
            term);

        var input = new string('(', 30) + "1" + new string(')', 30);
        var context = ParseContext.Create(ParseInput.CreateFromMemory(input), expr.ThenEnd(), options);

        context.RunToEnd(default).Success.Should().BeTrue();
        context.MemoTable.Count.Should().BePositive();
    }

    [TestCaseSource(nameof(Options))]
    public static void Skips_retried_parsers_that_are_cheaper_to_run_again(ParseOptions options)
    {
        // Both alternatives retry the token at the same position, but a token and its whitespace cost less than a memo lookup.
        var token = Char('a').ThenSkip(OptionalWhitespaces);
        var parser = Or(token.ThenSkip(Char('+')), token.ThenSkip(Char('-')));

        var context = ParseContext.Create(ParseInput.CreateFromMemory("a -"), parser.ThenEnd(), options);

        context.RunToEnd(default).Success.Should().BeTrue();
        context.MemoTable.Count.Should().Be(0);
    }

    [TestCaseSource(nameof(Options))]
    public static void Skips_shared_parsers_that_are_never_retried(ParseOptions options)
    {
        // Whitespace and the recursive value are shared, but no alternative retries them at the same position.
        IParser<int> value = null!;
        var ws = Regex(@"\s*");
        var number = Regex("[0-9]+").Transform(_ => 1);
        var array = Char('[').ThenSkip(ws).Then(Many(Lazy(() => value), Char(','))).ThenSkip(ws).ThenSkip(Char(']')).Transform(x => x.Count);
        value = ws.Then(Or(array, number)).ThenSkip(ws);

        var context = ParseContext.Create(ParseInput.CreateFromMemory("[ 1, [2, 3], [] ]"), value.ThenEnd(), options);

        context.RunToEnd(default).Success.Should().BeTrue();
        context.MemoTable.Count.Should().Be(0);
    }
}
