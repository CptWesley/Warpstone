namespace Warpstone.V2;

public abstract class ParseError : Exception, IParseError
{
    protected ParseError(
        IParsingInput input,
        IParser parser,
        int position,
        int length)
        : this(input, parser, position, length, null, null)
    {
    }

    protected ParseError(
        IParsingInput input,
        IParser parser,
        int position,
        int length,
        string? message)
        : this(input, parser, position, length, message, null)
    {
    }

    protected ParseError(
        IParsingInput input,
        IParser parser,
        int position,
        int length,
        string? message,
        Exception? innerException)
        : base(message, innerException)
    {
        Input = input;
        Parser = parser;
        Position = position;
        Length = length;
    }

    public IParsingInput Input { get; }

    public IParser Parser { get; }

    public int Position { get; }

    public int Length { get; }
}
