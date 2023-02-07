﻿namespace Warpstone.ParseState;

/// <summary>
/// Provides writeable access into the current state of a <see cref="IParseUnit"/> instance.
/// </summary>
public interface IParseState : IReadOnlyParseState
{
    /// <summary>
    /// Gets the used <see cref="IMemoTable"/> instance.
    /// </summary>
    public new IMemoTable MemoTable { get; }

    /*
    /// <summary>
    /// Gets or sets the current position in the parse unit.
    /// </summary>
    public new int Position { get; set; }
    */
}