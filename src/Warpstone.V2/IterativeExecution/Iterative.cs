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
    public static IterativeStep Done(Func<IterativeStep> first)
        => More(first, Done);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static IterativeStep Done()
        => Done(null as object);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static IterativeStep More(Func<IterativeStep> first, Func<object?, IterativeStep> more)
        => new()
        {
            Type = IterativeStepType.More,
            Value = null,

            First = first,
            More = more,
        };
}
