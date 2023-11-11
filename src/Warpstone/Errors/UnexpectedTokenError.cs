namespace Warpstone.Errors;

public sealed class UnexpectedTokenError : ParseError
{
    public UnexpectedTokenError(
        IParseInput input,
        IParser parser,
        int position,
        int length,
        string expected)
        : this(
              input,
              parser,
              position,
              length,
              expected,
              $"Expected token {expected}, but found token {Find(input.Input, position)}. At position {position}.")
    {
    }

    public UnexpectedTokenError(
        IParseInput input,
        IParser parser,
        int position,
        int length,
        string expected,
        string? message)
        : this(input, parser, position, length, expected, message, null)
    {
    }

    public UnexpectedTokenError(
        IParseInput input,
        IParser parser,
        int position,
        int length,
        string expected,
        string? message,
        Exception? innerException)
        : base(input, parser, position, length, message, innerException)
    {
        Expected = expected;
    }

    public string Expected { get; }

    public string Found => Find(Input.Input, Position);

    private static string Find(string input, int position)
    {
        if (position < 0)
        {
            return "BOF";
        }

        if (position >= input.Length)
        {
            return "EOF";
        }

        return $"'{input[position]}'";
    }
}
