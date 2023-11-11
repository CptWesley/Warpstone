namespace Warpstone.IterativeExecution;

public sealed class IterativeExecutor
{
    private readonly Stack<IterativeStep> stack = new();
    private object? last = null;

    private IterativeExecutor(in IterativeStep step)
    {
        stack.Push(step);
    }

    [MethodImpl(InlinedOptimized)]
    public static IterativeExecutor Create(in IterativeStep step)
        => new IterativeExecutor(step);

    public bool Done => stack.Count == 0;

    public object? Result => last;

    public void Step()
    {
        var job = stack.Pop();

        if (job.Type == IterativeStepType.Done)
        {
            last = job.Value;
        }
        else if (job.Type == IterativeStepType.More)
        {
            stack.Push(new()
            {
                Type = IterativeStepType.Continue,
                More = job.More,

                First = null,
                Value = null,
            });
            stack.Push(job.First!());
        }
        else if (job.Type == IterativeStepType.Continue)
        {
            stack.Push(job.More!(last));
        }
    }
}
