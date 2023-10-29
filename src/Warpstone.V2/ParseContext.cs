namespace Warpstone.V2;

public static class ParseContext
{
    public static IParseContext<T> Create<T>(IParseInput input, IParser<T> parser)
        => ParseContext<T>.Create(input, parser);

    public static IParseContext<T> Create<T>(string input, IParser<T> parser)
        => Create(new ParseInput(input), parser);
}

public sealed class ParseContext<T> : IParseContext<T>, IActiveParseContext
{
    private readonly Stack<Job> stack = new();
    private readonly MemoTable memo = new();

    private ParseContext(IParseInput input, IParser<T> parser)
    {
        Input = input;
        Parser = parser;
        Push(parser, 0, 0);
    }

    public IMemoTable MemoTable => memo;

    public bool Done => memo[0, Parser] is not null;

    public IParseInput Input { get; }

    IReadOnlyMemoTable IReadOnlyParseContext.MemoTable => MemoTable;

    public IParser<T> Parser { get; }

    public IParseResult<T> Result
        => memo[0, Parser] is IParseResult<T> result
        ? result
        : throw new InvalidOperationException();

    IParser IReadOnlyParseContext.Parser => Parser;

    IParseResult IReadOnlyParseContext.Result => Result;

    public void Push(IParser parser, int position, int phase)
        => stack.Push(new(parser, position, phase));

    public bool Step()
    {
        if (Done)
        {
            return false;
        }

        var job = stack.Pop();

        if (MemoTable[job.Position, job.Parser] is { })
        {
            return true;
        }

        job.Parser.Step(this, job.Position, job.Phase);
        return true;
    }

    public static IParseContext<T> Create(IParseInput input, IParser<T> parser)
        => new ParseContext<T>(input, parser);

    private readonly record struct Job(IParser Parser, int Position, int Phase);
}
