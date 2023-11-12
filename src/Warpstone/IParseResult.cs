namespace Warpstone;

/// <summary>
/// Interface for results of parsing.
/// </summary>
public interface IParseResult
{
    /// <summary>
    /// The starting position of the result.
    /// </summary>
    public int Position { get; }

    /// <summary>
    /// The number of characters in the input of the result.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// The next position in the input.
    /// </summary>
    public int NextPosition { get; }

    /// <summary>
    /// The parser used to obtain this result.
    /// </summary>
    public IParser Parser { get; }

    /// <summary>
    /// The context in which this result was obtained.
    /// </summary>
    public IReadOnlyParseContext Context { get; }

    /// <summary>
    /// Indicates whether the result is an indication of success.
    /// </summary>
    public ParseStatus Status { get; }

    /// <summary>
    /// The obtained value.
    /// Throws an exception if the parsing was not succcesful.
    /// </summary>
    public object? Value { get; }

    /// <summary>
    /// The list of errors (if any).
    /// </summary>
    public IReadOnlyList<IParseError> Errors { get; }

    /// <summary>
    /// Indicates whether or not the parsing was successful.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Throws an exception if the parsing result was not successful.
    /// </summary>
    public void ThrowIfUnsuccessful();
}

/// <summary>
/// Interface for results of parsing.
/// </summary>
/// <typeparam name="T">The type of the values obtained in the results.</typeparam>
public interface IParseResult<out T> : IParseResult
{
    /// <summary>
    /// The parser used to obtain this result.
    /// </summary>
    public new IParser<T> Parser { get; }

    /// <summary>
    /// The obtained value.
    /// Throws an exception if the parsing was not succcesful.
    /// </summary>
    public new T Value { get; }
}
