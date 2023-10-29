using Warpstone.V2.Parsers;

namespace Warpstone.V2.Errors;

public sealed class InfiniteRecursionError : ParseError
{
    public InfiniteRecursionError(IParseInput input, IParser parser, int position, int length)
        : this(input, parser, position, length, $"Infinite recursion occurred while parsing at position {position}.")
    {
    }

    public InfiniteRecursionError(IParseInput input, IParser parser, int position, int length, string? message)
        : this(input, parser, position, length, message, null)
    {
    }

    public InfiniteRecursionError(IParseInput input, IParser parser, int position, int length, string? message, Exception? innerException)
        : base(input, parser, position, length, message, innerException)
    {
    }
}
