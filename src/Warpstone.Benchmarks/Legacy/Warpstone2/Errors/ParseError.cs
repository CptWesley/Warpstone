#nullable enable

using Legacy.Warpstone2.Parsers;

namespace Legacy.Warpstone2.Errors;

/// <summary>
/// Base implementation for various different errors that occur during parsing.
/// </summary>
public abstract class ParseError : Exception, IParseError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParseError"/> class.
    /// </summary>
    /// <param name="context">The context of the error.</param>
    /// <param name="parser">The parser.</param>
    /// <param name="position">The position in the input string.</param>
    /// <param name="length">The length of the error.</param>
    protected ParseError(
        IReadOnlyParseContext context,
        IParser parser,
        int position,
        int length)
        : this(context, parser, position, length, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParseError"/> class.
    /// </summary>
    /// <param name="context">The context of the error.</param>
    /// <param name="parser">The parser.</param>
    /// <param name="position">The position in the input string.</param>
    /// <param name="length">The length of the error.</param>
    /// <param name="message">The custom message.</param>
    protected ParseError(
        IReadOnlyParseContext context,
        IParser parser,
        int position,
        int length,
        string? message)
        : this(context, parser, position, length, message, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParseError"/> class.
    /// </summary>
    /// <param name="context">The context of the error.</param>
    /// <param name="parser">The parser.</param>
    /// <param name="position">The position in the input string.</param>
    /// <param name="length">The length of the error.</param>
    /// <param name="message">The custom message.</param>
    /// <param name="innerException">The inner exception that caused this exception.</param>
    protected ParseError(
        IReadOnlyParseContext context,
        IParser parser,
        int position,
        int length,
        string? message,
        Exception? innerException)
        : base(message, innerException)
    {
        Context = context;
        Parser = parser;
        Position = position;
        Length = length;
    }

    /// <inheritdoc />
    public IReadOnlyParseContext Context { get; }

    /// <inheritdoc />
    public IParser Parser { get; }

    /// <inheritdoc />
    public int Position { get; }

    /// <inheritdoc />
    public int Length { get; }

    /// <inheritdoc />
    public ParseInputPosition InputStartPosition => Context.Input.GetPosition(Position);

    /// <inheritdoc />
    public ParseInputPosition InputEndPosition => Context.Input.GetPosition(Position + Length - 1);

    /// <inheritdoc />
    public abstract IParseError Retarget(IParser parser);
}
