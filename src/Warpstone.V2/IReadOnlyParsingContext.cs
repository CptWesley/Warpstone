namespace Warpstone.V2;

public interface IReadOnlyParsingContext
{
    public IParser Parser { get; }

    public IParsingInput Input { get; }

    public IReadOnlyMemoTable MemoTable { get; }

    public bool Done { get; }

    public IParseResult Result { get; }
}

public interface IReadOnlyParsingContext<T> : IReadOnlyParsingContext
{
    public new IParser<T> Parser { get; }

    public new IParseResult<T> Result { get; }
}
