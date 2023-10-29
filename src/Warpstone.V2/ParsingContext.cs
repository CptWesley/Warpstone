namespace Warpstone.V2;

public static class ParsingContext
{
    public static IParsingContext<T> Create<T>(IParsingInput input, IParser<T> parser)
        => ParsingContext<T>.Create(input, parser);

    public static IParsingContext<T> Create<T>(string input, IParser<T> parser)
        => Create(new ParsingInput(input), parser);
}

public sealed class ParsingContext<T> : IParsingContext<T>, IActiveParsingContext
{
    private readonly Stack<Job> stack = new();
    private readonly MemoTable memo = new();

    private ParsingContext(IParsingInput input, IParser<T> parser)
    {
        Input = input;
        Parser = parser;
        Push(parser, 0, 0);
    }

    public IMemoTable MemoTable => memo;

    public bool Done => memo[0, Parser] is not null;

    public IParsingInput Input { get; }

    IReadOnlyMemoTable IReadOnlyParsingContext.MemoTable => MemoTable;

    public IParser<T> Parser { get; }

    public IParseResult<T> Result
        => memo[0, Parser] is IParseResult<T> result
        ? result
        : throw new InvalidOperationException();

    IParser IReadOnlyParsingContext.Parser => Parser;

    IParseResult IReadOnlyParsingContext.Result => Result;

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

    public static IParsingContext<T> Create(IParsingInput input, IParser<T> parser)
        => new ParsingContext<T>(input, parser);

    private readonly record struct Job(IParser Parser, int Position, int Phase);
}
