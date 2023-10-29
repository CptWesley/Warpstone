namespace Warpstone.V2;

public static class ParseContextExtensions
{
    public static IParseResult StepUntilCompletion(this IParseContext context, CancellationToken cancellationToken = default)
    {
        while (!context.Done)
        {
            cancellationToken.ThrowIfCancellationRequested();
            context.Step();
        }

        return context.Result;
    }

    public static Task<IParseResult> StepUntilCompletionAsync(this IParseContext context, CancellationToken cancellationToken = default)
        => Task.Run(() => context.StepUntilCompletion(cancellationToken), cancellationToken);

    public static IParseResult<T> StepUntilCompletion<T>(this IParseContext<T> context, CancellationToken cancellationToken = default)
    {
        (context as IParseContext).StepUntilCompletion(cancellationToken);
        return context.Result;
    }

    public static Task<IParseResult<T>> StepUntilCompletionAsync<T>(this IParseContext<T> context, CancellationToken cancellationToken = default)
        => Task.Run(() => context.StepUntilCompletion(cancellationToken), cancellationToken);

    public static void Push(this IActiveParseContext context, IParser parser, int position)
        => context.Push(parser, position, 0);
}