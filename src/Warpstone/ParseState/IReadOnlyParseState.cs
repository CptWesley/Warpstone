namespace Warpstone.ParseState;

/// <summary>
/// Provides read-only access into the current state of a <see cref="IParseUnit"/> instance.
/// </summary>
public interface IReadOnlyParseState
{
    /// <summary>
    /// Gets the used <see cref="IReadOnlyMemoTable"/> instance.
    /// </summary>
    public IReadOnlyMemoTable MemoTable { get; }

    /*
    /// <summary>
    /// Gets the current position in the parse unit.
    /// </summary>
    public int Position { get; }
    */

    /// <summary>
    /// Gets the parse unit associated with this state.
    /// </summary>
    public IParseUnit Unit { get; }

    /*
    /// <summary>
    /// Gets the number of remaining characters to parse.
    /// </summary>
    public int RemainingCharacters { get; }
    */
}
