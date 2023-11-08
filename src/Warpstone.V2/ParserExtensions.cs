using Warpstone.V2.Alt;
using Warpstone.V2.Errors;
using Warpstone.V2.Parsers;

namespace Warpstone.V2;

public static class ParserExtensions
{
    //public static IParseResult<T> Match<T>(this IParser<T> parser, int position, int length, T value)
    //    => ParseResult.CreateMatch(parser, position, length, value);

    //public static IParseResult<T> Mismatch<T>(this IParser<T> parser, int position, IEnumerable<IParseError> errors)
    //    => ParseResult.CreateMismatch(parser, position, errors);

    public static IParseResult Mismatch(this IParser parser, int position, params IParseError[] errors)
        => parser.Mismatch(position, errors);

    public static IParseResult<T> Mismatch<T>(this IParser<T> parser, int position, params IParseError[] errors)
        => parser.Mismatch(position, errors);

    public static IParseContext<T> CreateContext<T>(this IParser<T> parser, IParseInput input)
        => ParseContext.Create(input, parser);

    public static IParseContext<T> CreateContext<T>(this IParser<T> parser, string input)
        => ParseContext.Create(input, parser);

    public static IParseResult<T> Parse<T>(this IParser<T> parser, IParseInput input)
        => parser.CreateContext(input).StepUntilCompletion();

    /*
    public static IParseResult<T> Parse<T>(this IParser<T> parser, string input)
        => parser.Parse(new ParseInput(input));
    */

    public static IParseResult<T> Parse<T>(this IParser<T> parser, string input)
        => new AltParseContext<T>(input, parser).RunToEnd();

    public static Task<IParseResult<T>> ParseAsync<T>(this IParser<T> parser, IParseInput input, CancellationToken cancellationToken = default)
        => parser.CreateContext(input).StepUntilCompletionAsync(cancellationToken);

    public static Task<IParseResult<T>> ParseAsync<T>(this IParser<T> parser, string input, CancellationToken cancellationToken = default)
        => parser.ParseAsync(new ParseInput(input), cancellationToken);
}