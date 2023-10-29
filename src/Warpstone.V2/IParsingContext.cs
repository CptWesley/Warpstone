namespace Warpstone.V2;

public interface IParsingContext : IReadOnlyParsingContext
{
    public bool Step();
}

public interface IParsingContext<T> : IParsingContext, IReadOnlyParsingContext<T>
{
}
