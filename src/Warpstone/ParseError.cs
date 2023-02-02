namespace Warpstone;

/// <summary>
/// Represents parse errors.
/// </summary>
/// <seealso cref="IParseError" />
public abstract class ParseError : IParseError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParseError"/> class.
    /// </summary>
    /// <param name="position">The position.</param>
    public ParseError(SourcePosition position)
        => Position = position;

    /// <inheritdoc/>
    public SourcePosition Position { get; }

    /// <inheritdoc/>
    public string GetMessage()
        => $"{GetSimpleMessage()} At {Position}.";

    /// <summary>
    /// Gets the simple message without positional information.
    /// </summary>
    /// <returns>The message.</returns>
    protected abstract string GetSimpleMessage();
}
