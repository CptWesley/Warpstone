namespace Warpstone;

/// <summary>
/// Interface for parse errors.
/// </summary>
public interface IParseError
{
    /// <summary>
    /// Gets the position.
    /// </summary>
    SourcePosition Position { get; }

    /// <summary>
    /// Turns the error into a readable message.
    /// </summary>
    /// <returns>A string representing the error.</returns>
    string GetMessage();
}
