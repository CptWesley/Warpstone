namespace Warpstone;

public interface IReadOnlyParseStack : IReadOnlyList<object>
{
    public object? Last { get; }
}
