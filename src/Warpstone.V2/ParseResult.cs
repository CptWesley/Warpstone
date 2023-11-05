using Warpstone.V2.Errors;
using Warpstone.V2.Parsers;

namespace Warpstone.V2;

public static class ParseResult
{
    public static IParseResult<T> CreateMatch<T>(IParser<T> parser, int position, int length, T value)
        => ParseResult<T>.CreateMatch(parser, position, length, value);

    public static IParseResult<T> CreateMismatch<T>(IParser<T> parser, int position, IEnumerable<IParseError> errors)
        => ParseResult<T>.CreateMismatch(parser, position, errors);

    public static IParseResult<T> CreateFail<T>(IParser<T> parser, int position, IParseInput input)
        => ParseResult<T>.CreateFail(parser, position, input);
}

public sealed class ParseResult<T> : IParseResult<T>
{
    private readonly T? value;

    private ParseResult(IParser<T> parser, int position, int length, ParseStatus status, T? value, IReadOnlyList<IParseError> errors)
    {
        Parser = parser;
        Position = position;
        Length = length;
        Status = status;
        this.value = value;
        Errors = errors;
    }

    public IParser<T> Parser { get; }

    public T Value
    {
        get
        {
            if (Status != ParseStatus.Match)
            {
                throw new InvalidOperationException();
            }

            return value!;
        }
    }

    public int Position { get; }

    public int NextPosition => Position + Length;

    public ParseStatus Status { get; }

    public int Length { get; }

    public IReadOnlyList<IParseError> Errors { get; }

    IParser IParseResult.Parser => Parser;

    object? IParseResult.Value => Value;

    public static IParseResult<T> CreateMatch(IParser<T> parser, int position, int length, T value)
        => new ParseResult<T>(parser, position, length, ParseStatus.Match, value, Array.Empty<IParseError>());

    public static IParseResult<T> CreateMismatch(IParser<T> parser, int position, IEnumerable<IParseError> errors)
        => new ParseResult<T>(parser, position, 0, ParseStatus.Mismatch, default, errors.ToArray());

    public static IParseResult<T> CreateFail(IParser<T> parser, int position, IParseInput input)
        => new ParseResult<T>(parser, position, 0, ParseStatus.Fail, default, new[] { new InfiniteRecursionError(input, parser, position, 0) });

    public IParseResult<T> AsMismatch()
    {
        if (Status == ParseStatus.Mismatch)
        {
            return this;
        }

        return new ParseResult<T>(Parser, Position, Length, ParseStatus.Mismatch, value, Errors);
    }

    IParseResult IParseResult.AsMismatch()
        => AsMismatch();

    public override string ToString()
        => Status switch
        {
            ParseStatus.Match => Value?.ToString() ?? string.Empty,
            ParseStatus.Fail => "Fail",
            _ => $"Error([{string.Join(", ", Errors)}])",
        };
}
