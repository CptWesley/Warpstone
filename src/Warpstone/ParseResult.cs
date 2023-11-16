namespace Warpstone;

/// <summary>
/// Provides helper methods to create new <see cref="IParseResult"/> instances.
/// </summary>
public static class ParseResult
{
    /// <summary>
    /// Creates a new successfully matched parse result.
    /// </summary>
    /// <param name="context">The parsing context in which the match was found.</param>
    /// <param name="parser">The parser which found the match.</param>
    /// <param name="position">The position in the input where the match started.</param>
    /// <param name="length">The number of characters in the input that were matched.</param>
    /// <param name="value">The value that was found.</param>
    /// <typeparam name="T">The result type of the parser.</typeparam>
    /// <returns>The newly created <see cref="IParseResult{T}"/> instance.</returns>
    public static IParseResult<T> CreateMatch<T>(IReadOnlyParseContext context, IParser<T> parser, int position, int length, T value)
        => ParseResult<T>.CreateMatch(context, parser, position, length, value);

    /// <summary>
    /// Creates a new failed parse result, because the input could not be matched.
    /// </summary>
    /// <param name="context">The parsing context in which the match was found.</param>
    /// <param name="parser">The parser which found the match.</param>
    /// <param name="position">The position in the input where the match started.</param>
    /// <param name="errors">The list of errors that explain why the parsing was not successful.</param>
    /// <typeparam name="T">The result type of the parser.</typeparam>
    /// <returns>The newly created <see cref="IParseResult{T}"/> instance.</returns>
    public static IParseResult<T> CreateMismatch<T>(IReadOnlyParseContext context, IParser<T> parser, int position, IEnumerable<IParseError> errors)
        => ParseResult<T>.CreateMismatch(context, parser, position, errors);

    /// <summary>
    /// Creates a new failed parse result, because the parser failed for
    /// reasons related to the parser definition.
    /// </summary>
    /// <param name="context">The parsing context in which the match was found.</param>
    /// <param name="parser">The parser which found the match.</param>
    /// <param name="position">The position in the input where the match started.</param>
    /// <typeparam name="T">The result type of the parser.</typeparam>
    /// <returns>The newly created <see cref="IParseResult{T}"/> instance.</returns>
    public static IParseResult<T> CreateFail<T>(IReadOnlyParseContext context, IParser<T> parser, int position)
        => ParseResult<T>.CreateFail(context, parser, position);
}

/// <summary>
/// Represents the result of parsing.
/// </summary>
/// <typeparam name="T">The type of the values obtained in the results.</typeparam>
public sealed class ParseResult<T> : IParseResult<T>
{
    private readonly T? value;

    private ParseResult(
        IReadOnlyParseContext context,
        IParser<T> parser,
        int position,
        int length,
        ParseStatus status,
        T? value,
        IImmutableList<IParseError> errors)
    {
        Context = context;
        Parser = parser;
        Position = position;
        Length = length;
        Status = status;
        this.value = value;
        Errors = errors;
    }

    /// <inheritdoc />
    public IParser<T> Parser { get; }

    /// <inheritdoc />
    public T Value
    {
        get
        {
            ThrowIfUnsuccessful();
            return value!;
        }
    }

    /// <inheritdoc />
    public int Position { get; }

    /// <inheritdoc />
    public int NextPosition => Position + Length;

    /// <inheritdoc />
    public ParseStatus Status { get; }

    /// <inheritdoc />
    public bool Success => Status == ParseStatus.Match;

    /// <inheritdoc />
    public int Length { get; }

    /// <inheritdoc />
    public IImmutableList<IParseError> Errors { get; }

    /// <inheritdoc />
    IParser IParseResult.Parser => Parser;

    /// <inheritdoc />
    object? IParseResult.Value => Value;

    /// <inheritdoc />
    public IReadOnlyParseContext Context { get; }

    /// <inheritdoc />
    public override string ToString()
        => Status switch
        {
            ParseStatus.Match => Value?.ToString() ?? string.Empty,
            ParseStatus.Fail => "Fail",
            _ => $"Error([{string.Join(", ", Errors)}])",
        };

    /// <inheritdoc />
    public void ThrowIfUnsuccessful()
    {
        if (Success)
        {
            return;
        }

        var throwables = Errors.OfType<Exception>().ToArray();

        throw throwables.Length switch
        {
            0 => new InvalidOperationException("Can not read property Value of non-successful ParseResult."),
            1 => throwables[0],
            _ => new AggregateException(throwables),
        };
    }

    /// <summary>
    /// Creates a new successfully matched parse result.
    /// </summary>
    /// <param name="context">The parsing context in which the match was found.</param>
    /// <param name="parser">The parser which found the match.</param>
    /// <param name="position">The position in the input where the match started.</param>
    /// <param name="length">The number of characters in the input that were matched.</param>
    /// <param name="value">The value that was found.</param>
    /// <returns>The newly created <see cref="IParseResult{T}"/> instance.</returns>
    public static IParseResult<T> CreateMatch(IReadOnlyParseContext context, IParser<T> parser, int position, int length, T value)
        => new ParseResult<T>(context, parser, position, length, ParseStatus.Match, value, ImmutableList<IParseError>.Empty);

    /// <summary>
    /// Creates a new failed parse result, because the input could not be matched.
    /// </summary>
    /// <param name="context">The parsing context in which the match was found.</param>
    /// <param name="parser">The parser which found the match.</param>
    /// <param name="position">The position in the input where the match started.</param>
    /// <param name="errors">The list of errors that explain why the parsing was not successful.</param>
    /// <returns>The newly created <see cref="IParseResult{T}"/> instance.</returns>
    public static IParseResult<T> CreateMismatch(IReadOnlyParseContext context, IParser<T> parser, int position, IEnumerable<IParseError> errors)
        => new ParseResult<T>(context, parser, position, 0, ParseStatus.Mismatch, default, errors.ToImmutableList());

    /// <summary>
    /// Creates a new failed parse result, because the parser failed for
    /// reasons related to the parser definition.
    /// </summary>
    /// <param name="context">The parsing context in which the match was found.</param>
    /// <param name="parser">The parser which found the match.</param>
    /// <param name="position">The position in the input where the match started.</param>
    /// <returns>The newly created <see cref="IParseResult{T}"/> instance.</returns>
    public static IParseResult<T> CreateFail(IReadOnlyParseContext context, IParser<T> parser, int position)
        => new ParseResult<T>(context, parser, position, 0, ParseStatus.Fail, default, ImmutableList<IParseError>.Empty.Add(new InfiniteRecursionError(context, parser, position, 0)));
}
