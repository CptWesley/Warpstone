#nullable enable

using Legacy.Warpstone2.Internal;

namespace Legacy.Warpstone2.IterativeExecution;

/// <summary>
/// Helper that allows for iteratively executing complex computations.
/// </summary>
public sealed class IterativeExecutor
{
    private readonly Stack<IIterativeStep> stack = new();
    private object? last = null;

    private IterativeExecutor(in IIterativeStep step)
    {
        stack.Push(step);
    }

    /// <summary>
    /// Creates a new <see cref="IterativeExecutor"/> class
    /// from the given <paramref name="step"/>.
    /// </summary>
    /// <param name="step">The iterative step that starts the execution.</param>
    /// <returns>The newly created <see cref="IterativeExecutor"/> instance.</returns>
    [MethodImpl(InternalMethodImplOptions.InlinedOptimized)]
    public static IterativeExecutor Create(in IIterativeStep step)
        => new IterativeExecutor(step);

    /// <summary>
    /// Indicates whether or not the execution has finished.
    /// </summary>
    public bool Done => stack.Count == 0;

    /// <summary>
    /// Gets the last returned value in the iterative execution.
    /// When the execution has finished, this is the final result
    /// value.
    /// </summary>
    public object? Result => last;

    /// <summary>
    /// Performs a single step in the iterative execution.
    /// </summary>
    public void Step()
    {
        var job = stack.Pop();
        job.Do(stack, ref last);
    }
}
