namespace Warpstone;

public interface IParseContext : IReadOnlyParseContext
{
    public bool Step();

    public IParseResult RunToEnd(CancellationToken cancellationToken);
}

public interface IParseContext<T> : IParseContext, IReadOnlyParseContext<T>
{
    public new IParseResult<T> RunToEnd(CancellationToken cancellationToken);
}
