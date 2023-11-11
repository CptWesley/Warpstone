namespace Warpstone;

public interface IReadOnlyParseContext
{
    public IParser Parser { get; }

    public IParseInput Input { get; }

    public IReadOnlyMemoTable MemoTable { get; }

    public bool Done { get; }

    public IParseResult Result { get; }
}

public interface IReadOnlyParseContext<T> : IReadOnlyParseContext
{
    public new IParser<T> Parser { get; }

    public new IParseResult<T> Result { get; }
}
