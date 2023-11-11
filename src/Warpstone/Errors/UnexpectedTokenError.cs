namespace Warpstone.Errors;

/// <summary>
/// Represents an error that occurs during parsing when an expected token is not found.
/// </summary>
public sealed class UnexpectedTokenError : ParseError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnexpectedTokenError"/> class.
    /// </summary>
    /// <param name="context">The context of the error.</param>
    /// <param name="parser">The parser.</param>
    /// <param name="position">The position in the input string.</param>
    /// <param name="length">The length of the error.</param>
    /// <param name="expected">The expected input.</param>
    public UnexpectedTokenError(
        IReadOnlyParseContext context,
        IParser parser,
        int position,
        int length,
        string expected)
        : this(
              context,
              parser,
              position,
              length,
              expected,
              $"Expected token {expected}, but found token {Find(context.Input.Content, position)}. At position {position}.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnexpectedTokenError"/> class.
    /// </summary>
    /// <param name="context">The context of the error.</param>
    /// <param name="parser">The parser.</param>
    /// <param name="position">The position in the input string.</param>
    /// <param name="length">The length of the error.</param>
    /// <param name="expected">The expected input.</param>
    /// <param name="message">The custom message.</param>
    public UnexpectedTokenError(
        IReadOnlyParseContext context,
        IParser parser,
        int position,
        int length,
        string expected,
        string? message)
        : this(context, parser, position, length, expected, message, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnexpectedTokenError"/> class.
    /// </summary>
    /// <param name="context">The context of the error.</param>
    /// <param name="parser">The parser.</param>
    /// <param name="position">The position in the input string.</param>
    /// <param name="length">The length of the error.</param>
    /// <param name="expected">The expected input.</param>
    /// <param name="message">The custom message.</param>
    /// <param name="innerException">The inner exception that caused this exception.</param>
    public UnexpectedTokenError(
        IReadOnlyParseContext context,
        IParser parser,
        int position,
        int length,
        string expected,
        string? message,
        Exception? innerException)
        : base(context, parser, position, length, message, innerException)
    {
        Expected = expected;
    }

    /// <summary>
    /// Gets the expected string.
    /// </summary>
    public string Expected { get; }

    /// <summary>
    /// Gets the found string.
    /// </summary>
    public string Found => Find(Context.Input.Content, Position);

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
