namespace Warpstone;

/// <summary>
/// Interface for all parser implementations.
/// </summary>
public interface IParser
{
    /// <summary>
    /// Gets the result type of the parser.
    /// </summary>
    public Type ResultType { get; }
}

/// <summary>
/// Interface for all typed parser implementations.
/// </summary>
/// <typeparam name="T">The result type being parsed.</typeparam>
#pragma warning disable S2326 // Unused type parameters should be removed
public interface IParser<out T> : IParser;
#pragma warning restore S2326 // Unused type parameters should be removed
