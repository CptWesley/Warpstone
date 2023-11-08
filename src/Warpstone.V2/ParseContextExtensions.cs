namespace Warpstone.V2;

public static class ParseContextExtensions
{
    public static IParseResult<T> RunToEnd<T>(this IParseContext<T> context)
        => context.RunToEnd(default);

    public static IParseResult RunToEnd(this IParseContext context)
        => context.RunToEnd(default);
}