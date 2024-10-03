namespace Warpstone.Errors;

/// <summary>
/// Represents an error that occurs during parsing when a transformation throws an exception.
/// </summary>
public sealed class TransformationError : ParseError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TransformationError"/> class.
    /// </summary>
    /// <param name="context">The context of the error.</param>
    /// <param name="parser">The parser.</param>
    /// <param name="position">The position in the input string.</param>
    /// <param name="length">The length of the error.</param>
    public TransformationError(IReadOnlyParseContext context, IParser parser, int position, int length)
        : base(context, parser, position, length)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TransformationError"/> class.
    /// </summary>
    /// <param name="context">The context of the error.</param>
    /// <param name="parser">The parser.</param>
    /// <param name="position">The position in the input string.</param>
    /// <param name="length">The length of the error.</param>
    /// <param name="message">The custom message.</param>
    public TransformationError(IReadOnlyParseContext context, IParser parser, int position, int length, string? message)
        : base(context, parser, position, length, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TransformationError"/> class.
    /// </summary>
    /// <param name="context">The context of the error.</param>
    /// <param name="parser">The parser.</param>
    /// <param name="position">The position in the input string.</param>
    /// <param name="length">The length of the error.</param>
    /// <param name="message">The custom message.</param>
    /// <param name="innerException">The inner exception that caused this exception.</param>
    public TransformationError(IReadOnlyParseContext context, IParser parser, int position, int length, string? message, Exception? innerException)
        : base(context, parser, position, length, message, innerException)
    {
    }

    /// <inheritdoc />
    public override IParseError Retarget(IParser parser)
        => new TransformationError(Context, parser, Position, Length, Message, InnerException);
}
