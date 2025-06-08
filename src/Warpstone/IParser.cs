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

    /// <summary>
    /// Gets the parser implementation for the current parser expression with the given <paramref name="options"/>.
    /// </summary>
    /// <param name="options">The options used for finding the correct parser implementation.</param>
    /// <returns>The found parser implementation.</returns>
    public IParserImplementation GetImplementation(ParseOptions options);
}

/// <summary>
/// Interface for all typed parser implementations.
/// </summary>
/// <typeparam name="T">The result type being parsed.</typeparam>
public interface IParser<out T> : IParser
{
    /// <inheritdoc cref="IParser.GetImplementation(ParseOptions)" />
    public new IParserImplementation<T> GetImplementation(ParseOptions options);
}
