namespace Warpstone.Errors;

/// <summary>
/// Represents a parse error.
/// </summary>
public interface IParseError
{
    /// <summary>
    /// The parsing context in which this error was found.
    /// </summary>
    public IReadOnlyParseContext Context { get; }

    /// <summary>
    /// The parser that caused this error.
    /// </summary>
    public IParser Parser { get; }

    /// <summary>
    /// The position in the <see cref="Context"/> where this error occurred.
    /// </summary>
    public int Position { get; }

    /// <summary>
    /// The number of characters in the input that caused this error.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Creates a new, retargeted error.
    /// </summary>
    /// <param name="parser">The new parser.</param>
    /// <returns>The newly created <see cref="IParseError"/>.</returns>
    public IParseError Retarget(IParser parser);
}
