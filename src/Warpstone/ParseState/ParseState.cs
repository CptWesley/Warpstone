namespace Warpstone.ParseState;

/// <summary>
/// Provides an implementation for the <see cref="IParseState"/> interface.
/// </summary>
internal class ParseState : IParseState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParseState"/> class.
    /// </summary>
    /// <param name="unit">The parse unit associated with this state.</param>
    public ParseState(IParseUnit unit)
    {
        Unit = unit;
    }

    /// <inheritdoc/>
    public IMemoTable MemoTable { get; } = new MemoTable();

    /// <inheritdoc/>
    IReadOnlyMemoTable IReadOnlyParseState.MemoTable => MemoTable;

    /// <inheritdoc/>
    public int Position { get; set; }

    /*
    /// <inheritdoc/>
    int IReadOnlyParseState.Position => Position;
    */

    /// <inheritdoc/>
    public IParseUnit Unit { get; }

    /*
    /// <inheritdoc/>
    public int RemainingCharacters => Unit.StartingPosition + Unit.MaxLength - Position;
    */
}
