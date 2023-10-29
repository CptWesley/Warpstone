namespace Warpstone.V2;

public static class ParsingContextExtensions
{
    public static IParseResult StepUntilCompletion(this IParsingContext context, CancellationToken cancellationToken = default)
    {
        while (!context.Done)
        {
            cancellationToken.ThrowIfCancellationRequested();
            context.Step();
        }

        return context.Result;
    }

    public static Task<IParseResult> StepUntilCompletionAsync(this IParsingContext context, CancellationToken cancellationToken = default)
        => Task.Run(() => context.StepUntilCompletion(cancellationToken), cancellationToken);

    public static IParseResult<T> StepUntilCompletion<T>(this IParsingContext<T> context, CancellationToken cancellationToken = default)
    {
        (context as IParsingContext).StepUntilCompletion(cancellationToken);
        return context.Result;
    }

    public static Task<IParseResult<T>> StepUntilCompletionAsync<T>(this IParsingContext<T> context, CancellationToken cancellationToken = default)
        => Task.Run(() => context.StepUntilCompletion(cancellationToken), cancellationToken);

    public static void Push(this IActiveParsingContext context, IParser parser, int position)
        => context.Push(parser, position, 0);
}