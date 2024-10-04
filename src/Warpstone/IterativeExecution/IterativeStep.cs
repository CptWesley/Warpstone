namespace Warpstone.IterativeExecution;

/// <summary>
/// Represents an iterative step that can be executed by
/// a <see cref="IterativeExecutor"/>.
/// </summary>
/// <remarks>
/// Avoid creating instances manually.
/// To ensure correct behaviour use the methods provided
/// in the <see cref="Iterative"/> helper class.
/// </remarks>
public interface IterativeStep
{
}

/// <summary>
/// Represents an iterative step that indicates the end of an execution.
/// </summary>
public sealed class IterativeDone : IterativeStep
{
    /// <summary>
    /// The result value (if any).
    /// </summary>
    public required object? Value { get; init; }
}

/// <summary>
/// Represents an interative step that requires some other work to be done first.
/// </summary>
public sealed class IterativeMore : IterativeStep
{
    /// <summary>
    /// The function that is executed first (if any).
    /// </summary>
    public required Func<IterativeStep>? First { get; init; }

    /// <summary>
    /// The continuation function that is executed after <see cref="First"/>
    /// (if any).
    /// </summary>
    public required Func<object?, IterativeStep>? More { get; init; }
}

/// <summary>
/// Represents an interative step that handles the result of a previous execution.
/// </summary>
public sealed class IterativeContinue : IterativeStep
{
    /// <summary>
    /// The continuation function that is executed.
    /// </summary>
    public required Func<object?, IterativeStep>? More { get; init; }
}
