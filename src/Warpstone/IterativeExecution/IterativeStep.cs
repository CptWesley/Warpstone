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
public readonly record struct IterativeStep
{
    /// <summary>
    /// The type of iterative step.
    /// </summary>
    public required IterativeStepType Type { get; init; }

    /// <summary>
    /// The result value (if any).
    /// </summary>
    public required object? Value { get; init; }

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
