namespace Warpstone.V2;

public static class ParserExtensions
{
    public static IParseResult<T> Succeed<T>(this IParser<T> parser, int position, int length, T value)
        => ParseResult<T>.CreateSuccess(parser, position, length, value);

    public static IParseResult<T> Fail<T>(this IParser<T> parser, int position, IEnumerable<IParseError> errors)
        => ParseResult<T>.CreateFailure(parser, position, errors);

    public static IParseResult<T> Fail<T>(this IParser<T> parser, int position, params IParseError[] errors)
        => parser.Fail(position, errors as IEnumerable<IParseError>);

    public static IParseContext<T> CreateContext<T>(this IParser<T> parser, IParseInput input)
        => ParseContext.Create(input, parser);

    public static IParseContext<T> CreateContext<T>(this IParser<T> parser, string input)
        => ParseContext.Create(input, parser);

    public static IParseResult<T> Parse<T>(this IParser<T> parser, IParseInput input)
        => parser.CreateContext(input).StepUntilCompletion();

    public static IParseResult<T> Parse<T>(this IParser<T> parser, string input)
        => parser.Parse(new ParseInput(input));

    public static Task<IParseResult<T>> ParseAsync<T>(this IParser<T> parser, IParseInput input, CancellationToken cancellationToken = default)
        => parser.CreateContext(input).StepUntilCompletionAsync(cancellationToken);

    public static Task<IParseResult<T>> ParseAsync<T>(this IParser<T> parser, string input, CancellationToken cancellationToken = default)
        => parser.ParseAsync(new ParseInput(input), cancellationToken);
}