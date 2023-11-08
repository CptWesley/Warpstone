namespace Warpstone.V2;

public static class ParserExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static IParseResult Mismatch(this IParser parser, int position, params IParseError[] errors)
        => parser.Mismatch(position, errors);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static IParseResult<T> Mismatch<T>(this IParser<T> parser, int position, params IParseError[] errors)
        => parser.Mismatch(position, errors);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static IParseContext<T> CreateContext<T>(this IParser<T> parser, IParseInput input)
        => ParseContext.Create(input, parser);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static IParseContext<T> CreateContext<T>(this IParser<T> parser, string input)
        => ParseContext.Create(input, parser);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static IParseResult<T> Parse<T>(this IParser<T> parser, IParseInput input)
        => parser.CreateContext(input).RunToEnd();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static IParseResult<T> Parse<T>(this IParser<T> parser, string input)
        => parser.CreateContext(input).RunToEnd();
}