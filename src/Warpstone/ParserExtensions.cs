namespace Warpstone;

/// <summary>
/// Provides extension methods for <see cref="IParser"/> and
/// <see cref="IParser{T}"/> instances.
/// </summary>
public static class ParserExtensions
{
    /// <inheritdoc cref="IParser.Mismatch(IReadOnlyParseContext, int, IEnumerable{IParseError})" />
    [MethodImpl(InlinedOptimized)]
    public static IParseResult Mismatch(this IParser parser, IReadOnlyParseContext context, int position, params IParseError[] errors)
        => parser.Mismatch(context, position, errors);

    /// <summary>
    /// Create a parse result for errors encountered in the input.
    /// </summary>
    /// <param name="parser">The parser.</param>
    /// <param name="context">The parsing context.</param>
    /// <param name="position">The position in the input.</param>
    /// <param name="length">The length of the failed match.</param>
    /// <param name="expected">The expected input.</param>
    /// <returns>The newly created <see cref="IParseResult"/>.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IParseResult Mismatch(
        this IParser parser,
        IReadOnlyParseContext context,
        int position,
        int length,
        string expected)
        => parser.Mismatch(context, position, new UnexpectedTokenError(context, parser, position, length, expected));

    /// <inheritdoc cref="IParser{T}.Mismatch(IReadOnlyParseContext, int, IEnumerable{IParseError})" />
    [MethodImpl(InlinedOptimized)]
    public static IParseResult<T> Mismatch<T>(this IParser<T> parser, IReadOnlyParseContext context, int position, params IParseError[] errors)
        => parser.Mismatch(context, position, errors);

    /// <summary>
    /// Create a parse result for errors encountered in the input.
    /// </summary>
    /// <param name="parser">The parser.</param>
    /// <param name="context">The parsing context.</param>
    /// <param name="position">The position in the input.</param>
    /// <param name="length">The length of the failed match.</param>
    /// <param name="expected">The expected input.</param>
    /// <typeparam name="T">The result type of the parser.</typeparam>
    /// <returns>The newly created <see cref="IParseResult"/>.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IParseResult<T> Mismatch<T>(
        this IParser<T> parser,
        IReadOnlyParseContext context,
        int position,
        int length,
        string expected)
        => parser.Mismatch(context, position, new UnexpectedTokenError(context, parser, position, length, expected));

    /// <summary>
    /// Create a parse result for a successful match in the input.
    /// </summary>
    /// <param name="parser">The parser.</param>
    /// <param name="context">The parsing context.</param>
    /// <param name="position">The position in the input.</param>
    /// <param name="length">The length of the match.</param>
    /// <param name="value">The value produced by the parser.</param>
    /// <typeparam name="T">The result type of the parser.</typeparam>
    /// <returns>The newly created <see cref="IParseResult{T}"/>.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IParseResult<T> Match<T>(this IParser<T> parser, IReadOnlyParseContext context, int position, int length, T value)
        => ParseResult.CreateMatch(context, parser, position, length, value);

    /// <summary>
    /// Creates a new <see cref="IParseContext"/> from the given <paramref name="input"/>
    /// and the initial <paramref name="parser"/>.
    /// </summary>
    /// <param name="parser">The parser that starts the parsing.</param>
    /// <param name="input">The input to be parsed.</param>
    /// <returns>The newly created <see cref="IParseContext"/> instance.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IParseContext CreateContext(this IParser parser, IParseInput input)
        => ParseContext.Create(input, parser);

    /// <summary>
    /// Creates a new <see cref="IParseContext{T}"/> from the given <paramref name="input"/>
    /// and the initial <paramref name="parser"/>.
    /// </summary>
    /// <param name="parser">The parser that starts the parsing.</param>
    /// <param name="input">The input to be parsed.</param>
    /// <typeparam name="T">The output type of the initial parser.</typeparam>
    /// <returns>The newly created <see cref="IParseContext{T}"/> instance.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IParseContext<T> CreateContext<T>(this IParser<T> parser, IParseInput input)
        => ParseContext.Create(input, parser);

    /// <summary>
    /// Creates a new <see cref="IParseContext"/> from the given <paramref name="input"/>
    /// and the initial <paramref name="parser"/>.
    /// </summary>
    /// <param name="parser">The parser that starts the parsing.</param>
    /// <param name="input">The input to be parsed.</param>
    /// <returns>The newly created <see cref="IParseContext"/> instance.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IParseContext CreateContext(this IParser parser, string input)
        => ParseContext.Create(input, parser);

    /// <summary>
    /// Creates a new <see cref="IParseContext{T}"/> from the given <paramref name="input"/>
    /// and the initial <paramref name="parser"/>.
    /// </summary>
    /// <param name="parser">The parser that starts the parsing.</param>
    /// <param name="input">The input to be parsed.</param>
    /// <typeparam name="T">The output type of the initial parser.</typeparam>
    /// <returns>The newly created <see cref="IParseContext{T}"/> instance.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IParseContext<T> CreateContext<T>(this IParser<T> parser, string input)
        => ParseContext.Create(input, parser);

    /// <summary>
    /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>.
    /// </summary>
    /// <param name="parser">The parser.</param>
    /// <param name="input">The input.</param>
    /// <returns>The parse result.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IParseResult TryParse(this IParser parser, IParseInput input)
        => parser.CreateContext(input).RunToEnd();

    /// <summary>
    /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="parser">The parser.</param>
    /// <param name="input">The input.</param>
    /// <returns>The parse result.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IParseResult<T> TryParse<T>(this IParser<T> parser, IParseInput input)
        => parser.CreateContext(input).RunToEnd();

    /// <summary>
    /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>.
    /// </summary>
    /// <param name="parser">The parser.</param>
    /// <param name="input">The input.</param>
    /// <returns>The parse result.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IParseResult TryParse(this IParser parser, string input)
        => parser.CreateContext(input).RunToEnd();

    /// <summary>
    /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="parser">The parser.</param>
    /// <param name="input">The input.</param>
    /// <returns>The parse result.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IParseResult<T> TryParse<T>(this IParser<T> parser, string input)
        => parser.CreateContext(input).RunToEnd();

    /// <summary>
    /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>.
    /// </summary>
    /// <param name="parser">The parser.</param>
    /// <param name="input">The input.</param>
    /// <returns>The parse result.</returns>
    [MethodImpl(InlinedOptimized)]
    public static object? Parse(this IParser parser, IParseInput input)
        => parser.TryParse(input).Value;

    /// <summary>
    /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="parser">The parser.</param>
    /// <param name="input">The input.</param>
    /// <returns>The parse result.</returns>
    [MethodImpl(InlinedOptimized)]
    public static T Parse<T>(this IParser<T> parser, IParseInput input)
        => parser.TryParse(input).Value;

    /// <summary>
    /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>.
    /// </summary>
    /// <param name="parser">The parser.</param>
    /// <param name="input">The input.</param>
    /// <returns>The parse result.</returns>
    [MethodImpl(InlinedOptimized)]
    public static object? Parse(this IParser parser, string input)
        => parser.TryParse(input).Value;

    /// <summary>
    /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="parser">The parser.</param>
    /// <param name="input">The input.</param>
    /// <returns>The parse result.</returns>
    [MethodImpl(InlinedOptimized)]
    public static T Parse<T>(this IParser<T> parser, string input)
        => parser.TryParse(input).Value;
}
