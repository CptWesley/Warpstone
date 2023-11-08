namespace Warpstone.V2;

public static class ParseContextExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static IParseResult<T> RunToEnd<T>(this IParseContext<T> context)
        => context.RunToEnd(default);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static IParseResult RunToEnd(this IParseContext context)
        => context.RunToEnd(default);
}