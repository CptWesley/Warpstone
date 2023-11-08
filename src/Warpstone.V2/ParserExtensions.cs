namespace Warpstone.V2;

public static class ParserExtensions
{
    public static IParseResult Mismatch(this IParser parser, int position, params IParseError[] errors)
        => parser.Mismatch(position, errors);

    public static IParseResult<T> Mismatch<T>(this IParser<T> parser, int position, params IParseError[] errors)
        => parser.Mismatch(position, errors);

    public static IParseContext<T> CreateContext<T>(this IParser<T> parser, IParseInput input)
        => ParseContext.Create(input, parser);

    public static IParseContext<T> CreateContext<T>(this IParser<T> parser, string input)
        => ParseContext.Create(input, parser);

    public static IParseResult<T> Parse<T>(this IParser<T> parser, IParseInput input)
        => ParseContext.Create(input, parser).RunToEnd();

    public static IParseResult<T> Parse<T>(this IParser<T> parser, string input)
        => ParseContext.Create(input, parser).RunToEnd();
}