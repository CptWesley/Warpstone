namespace Warpstone;

/// <summary>
/// Indicates the desired execution mode of the parser.
/// </summary>
public enum ParserExecutionMode
{
    /// <summary>
    /// Automatically determines the execution mode.
    /// This will prefer <see cref="Recursive"/> mode
    /// if no recursive grammar were detected, otherwise
    /// resorts to the safer (but slower) <see cref="Iterative"/>
    /// mode.
    /// </summary>
    Auto = 0,

    /// <summary>
    /// Forces the execution mode to run in iterative mode,
    /// which guarantees that no <see cref="StackOverflowException"/>
    /// will occur due to deep recursive grammar, at the cost of some performance.
    /// </summary>
    Iterative = 1,

    /// <summary>
    /// Forces the execution mode to run in recursive mode,
    /// which might result in a <see cref="StackOverflowException"/>
    /// if the grammar is recursively too deep.
    /// </summary>
    Recursive = 2,
}
