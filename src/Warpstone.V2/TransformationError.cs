namespace Warpstone.V2;

public sealed class TransformationError : ParseError
{
    public TransformationError(IParsingInput input, IParser parser, int position, int length)
        : base(input, parser, position, length)
    {
    }

    public TransformationError(IParsingInput input, IParser parser, int position, int length, string? message)
        : base(input, parser, position, length, message)
    {
    }

    public TransformationError(IParsingInput input, IParser parser, int position, int length, string? message, Exception? innerException)
        : base(input, parser, position, length, message, innerException)
    {
    }
}