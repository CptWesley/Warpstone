namespace Legacy.Warpstone2.IterativeExecution;

/// <summary>
/// Represents an iterative step that can be executed by
/// a <see cref="IterativeExecutor"/>.
/// </summary>
/// <remarks>
/// Avoid creating instances manually.
/// To ensure correct behaviour use the methods provided
/// in the <see cref="Iterative"/> helper class.
/// </remarks>
public interface IIterativeStep
{
    /// <summary>
    /// Execute the iterative step.
    /// </summary>
    /// <param name="stack">The stack.</param>
    /// <param name="last">The previous result.</param>
    public void Do(Stack<IIterativeStep> stack, ref object? last);
}

/// <summary>
/// Represents an iterative step that indicates the end of an execution.
/// </summary>
public sealed class IterativeDone : IIterativeStep
{
    /// <summary>
    /// The result value (if any).
    /// </summary>
    public required object? Value { get; init; }

    /// <inheritdoc />
    public void Do(Stack<IIterativeStep> stack, ref object? last)
    {
        last = Value;
    }
}

/// <summary>
/// Represents an interative step that requires some other work to be done first.
/// </summary>
public abstract class IterativeMore : IIterativeStep
{
    /// <summary>
    /// The function that is executed first (if any).
    /// </summary>
    /// <returns>The step that performs the initial function.</returns>
    public abstract IIterativeStep DoFirst();

    /// <summary>
    /// The continuation function that is executed after <see cref="DoFirst"/>
    /// (if any).
    /// </summary>
    /// <param name="prev">The result of <see cref="DoFirst"/>.</param>
    /// <returns>The step that performs the continuation.</returns>
    public abstract IIterativeStep DoMore(object? prev);

    /// <inheritdoc />
    public void Do(Stack<IIterativeStep> stack, ref object? last)
    {
        stack.Push(new IterativeContinue(this));
        stack.Push(DoFirst());
    }
}

/// <summary>
/// Represents an interative step that requires some other work to be done first.
/// </summary>
public sealed class IterativeMoreAdHoc : IterativeMore
{
    /// <summary>
    /// The function that is executed first (if any).
    /// </summary>
    public required Func<IIterativeStep> First { get; init; }

    /// <summary>
    /// The continuation function that is executed after <see cref="First"/>
    /// (if any).
    /// </summary>
    public required Func<object?, IIterativeStep> More { get; init; }

    /// <inheritdoc />
    public override IIterativeStep DoFirst()
        => First();

    /// <inheritdoc />
    public override IIterativeStep DoMore(object? prev)
        => More(prev);
}

/// <summary>
/// Represents an interative step that handles the result of a previous execution.
/// </summary>
public sealed class IterativeContinue(IterativeMore more) : IIterativeStep
{
    /// <inheritdoc />
    public void Do(Stack<IIterativeStep> stack, ref object? last)
    {
        stack.Push(more.DoMore(last));
    }
}
