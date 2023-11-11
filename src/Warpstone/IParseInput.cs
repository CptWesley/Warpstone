namespace Warpstone;

/// <summary>
/// Interface for inputs used by parsers.
/// </summary>
public interface IParseInput
{
    /// <summary>
    /// The string content of the input.
    /// </summary>
    public string Content { get; }

    /// <summary>
    /// The source of the input.
    /// </summary>
    public IParseInputSource Source { get; }
}
