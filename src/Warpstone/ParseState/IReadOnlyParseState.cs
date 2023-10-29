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

    /// <summary>
    /// Gets the parse unit associated with this state.
    /// </summary>
    public IParseUnit Unit { get; }

    /// <summary>
    /// Gets the used <see cref="IReadOnlyParseQueue"/> instance.
    /// </summary>
    public IReadOnlyParseQueue Queue { get; }
}
