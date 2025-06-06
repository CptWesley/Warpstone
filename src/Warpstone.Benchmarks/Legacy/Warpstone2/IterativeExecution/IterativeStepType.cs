namespace Legacy.Warpstone2.IterativeExecution;

/// <summary>
/// Used to indicate to the <see cref="IterativeExecutor"/>
/// how a <see cref="IIterativeStep"/> should be handled.
/// </summary>
public enum IterativeStepType
{
    /// <summary>
    /// The iterative step type is not defined.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The iterative step represents a return statement.
    /// </summary>
    Done = 1,

    /// <summary>
    /// The iterative step represents a function call followed by a continuation.
    /// </summary>
    More = 2,

    /// <summary>
    /// The iterative step represents a continuation handler.
    /// </summary>
    Continue = 3,
}
