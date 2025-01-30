namespace Warpstone;

/// <summary>
/// Represents the context necessary for parsing recursively.
/// </summary>
public interface IRecursiveParseContext
{
    /// <summary>
    /// The input string.
    /// </summary>
    public string Input { get; }

    /// <summary>
    /// The memo table.
    /// </summary>
    public IMemoTable MemoTable { get; }
}
