using Warpstone.V2.Errors;
using Warpstone.V2.Parsers;

namespace Warpstone.V2;

public static class ParseResult
{
    public static IParseResult<T> CreateSuccess<T>(IParser<T> parser, int position, int length, T value)
        => ParseResult<T>.CreateSuccess(parser, position, length, value);

    public static IParseResult<T> CreateFailure<T>(IParser<T> parser, int position, IEnumerable<IParseError> errors)
        => ParseResult<T>.CreateFailure(parser, position, errors);
}

public sealed class ParseResult<T> : IParseResult<T>
{
    private readonly T? value;

    private ParseResult(IParser<T> parser, int position, int length, bool success, T? value, IReadOnlyList<IParseError> errors)
    {
        Parser = parser;
        Position = position;
        Length = length;
        Success = success;
        this.value = value;
        Errors = errors;
    }

    public IParser<T> Parser { get; }

    public T Value
    {
        get
        {
            if (!this.Success)
            {
                throw new InvalidOperationException();
            }

            return value!;
        }
    }

    public int Position { get; }

    public int NextPosition => Position + Length;

    public bool Success { get; }

    public int Length { get; }

    public IReadOnlyList<IParseError> Errors { get; }

    IParser IParseResult.Parser => Parser;

    object? IParseResult.Value => Value;

    public static IParseResult<T> CreateSuccess(IParser<T> parser, int position, int length, T value)
        => new ParseResult<T>(parser, position, length, true, value, Array.Empty<IParseError>());

    public static IParseResult<T> CreateFailure(IParser<T> parser, int position, IEnumerable<IParseError> errors)
        => new ParseResult<T>(parser, position, 0, false, default, errors.ToArray());
}
