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
              GetMessage(context, position, expected))
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
    public string Found => Find(Context, Position);

    private static string Find(IReadOnlyParseContext context, int position)
    {
        if (position < 0)
        {
            return "BOF";
        }

        var input = context.Input.Content;

        if (position >= input.Length)
        {
            return "EOF";
        }

        return $"'{input[position]}'";
    }

    private static string GetMessage(IReadOnlyParseContext context, int position, string expected)
    {
        var found = Find(context, position);
        var sb = new StringBuilder()
            .Append("Expected ")
            .Append(expected)
            .Append(" but found ")
            .Append(found)
            .Append(" at ")
            .Append(context.Input.GetPosition(position).ToString());

        if (context.Input.Source is not FromMemorySource)
        {
            sb.Append(" in ")
                .Append(context.Input.Source.ToString());
        }

        sb.Append('.');
        return sb.ToString();
    }

    /// <inheritdoc />
    public override IParseError Retarget(IParser parser)
        => new UnexpectedTokenError(Context, parser, Position, Length, Expected, Message, InnerException);
}
