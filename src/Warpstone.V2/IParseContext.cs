namespace Warpstone.V2;

public interface IParseContext : IReadOnlyParseContext
{
    public bool Step();
}

public interface IParseContext<T> : IParseContext, IReadOnlyParseContext<T>
{
}
