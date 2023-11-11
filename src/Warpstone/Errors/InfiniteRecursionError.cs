namespace Warpstone.Errors;

/// <summary>
/// Represents a <see cref="ParseError"/> that is thrown when infinite recursion is detected.
/// </summary>
public sealed class InfiniteRecursionError : ParseError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InfiniteRecursionError"/> class.
    /// </summary>
    /// <param name="context">The context of the error.</param>
    /// <param name="parser">The parser.</param>
    /// <param name="position">The position in the input string.</param>
    /// <param name="length">The length of the error.</param>
    public InfiniteRecursionError(IReadOnlyParseContext context, IParser parser, int position, int length)
        : this(context, parser, position, length, $"Infinite recursion occurred while parsing at position {position}.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InfiniteRecursionError"/> class.
    /// </summary>
    /// <param name="context">The context of the error.</param>
    /// <param name="parser">The parser.</param>
    /// <param name="position">The position in the input string.</param>
    /// <param name="length">The length of the error.</param>
    /// <param name="message">The custom message.</param>
    public InfiniteRecursionError(IReadOnlyParseContext context, IParser parser, int position, int length, string? message)
        : this(context, parser, position, length, message, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InfiniteRecursionError"/> class.
    /// </summary>
    /// <param name="context">The context of the error.</param>
    /// <param name="parser">The parser.</param>
    /// <param name="position">The position in the input string.</param>
    /// <param name="length">The length of the error.</param>
    /// <param name="message">The custom message.</param>
    /// <param name="innerException">The inner exception that caused this exception.</param>
    public InfiniteRecursionError(IReadOnlyParseContext context, IParser parser, int position, int length, string? message, Exception? innerException)
        : base(context, parser, position, length, message, innerException)
    {
    }
}
