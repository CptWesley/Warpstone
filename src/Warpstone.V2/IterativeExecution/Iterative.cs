namespace Warpstone.V2.IterativeExecution;

public static class Iterative
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static IterativeStep Done(object? value)
        => new()
        {
            Type = IterativeStepType.Done,
            Value = value,

            First = null,
            More = null,
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static IterativeStep More(Func<IterativeStep> first, Func<object?, IterativeStep> more)
        => new()
        {
            Type = IterativeStepType.More,
            Value = null,

            First = first,
            More = more,
        };

    public static object? Run(this IterativeExecutor iterator)
    {
        while (!iterator.Done)
        {
            iterator.Step();
        }

        return iterator.Result;
    }

    public static object? Run(this IterativeExecutor iterator, CancellationToken cancellationToken)
    {
        while (!iterator.Done)
        {
            cancellationToken.ThrowIfCancellationRequested();
            iterator.Step();
        }

        return iterator.Result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static object? Run(this in IterativeStep iteration)
        => IterativeExecutor.Create(iteration).Run();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static object? Run(this in IterativeStep iteration, CancellationToken cancellationToken)
        => IterativeExecutor.Create(iteration).Run(cancellationToken);
}
