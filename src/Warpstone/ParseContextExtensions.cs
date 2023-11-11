namespace Warpstone;

public static class ParseContextExtensions
{
    [MethodImpl(InlinedOptimized)]
    public static IParseResult<T> RunToEnd<T>(this IParseContext<T> context)
        => context.RunToEnd(default);

    [MethodImpl(InlinedOptimized)]
    public static IParseResult RunToEnd(this IParseContext context)
        => context.RunToEnd(default);
}
