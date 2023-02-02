using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Warpstone;

/// <summary>
/// Extension methods for <see cref="IParser{TOutput}"/>.
/// These extensions are necessary, because <see cref="Task"/> does not support covariance,
/// while the <see cref="IParser{TOutput}"/> interface does.
/// </summary>
public static class ParserExtensions
{
    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="parser">The parser to use for parsing.</param>
    /// <param name="input">The input.</param>
    /// <param name="startPosition">The index of the first character to be parsed.</param>
    /// <param name="maxLength">The maximum length of input to be parsed.</param>
    /// <param name="cancellationToken">The cancellation token used for cancelling the parsing task.</param>
    /// <typeparam name="TOutput">The output type of the parser.</typeparam>
    /// <returns>The result of running the parser.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParseResult<TOutput> TryParse<TOutput>(this IParser<TOutput> parser, string input, int startPosition, int maxLength, CancellationToken cancellationToken)
    {
        ParseUnit<TOutput> unit = new ParseUnit<TOutput>(input, startPosition, maxLength, parser);
        unit.Parse(cancellationToken);
        return unit.Result;
    }

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="parser">The parser to use for parsing.</param>
    /// <param name="input">The input.</param>
    /// <param name="startPosition">The index of the first character to be parsed.</param>
    /// <param name="maxLength">The maximum length of input to be parsed.</param>
    /// <typeparam name="TOutput">The output type of the parser.</typeparam>
    /// <returns>The result of running the parser.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParseResult<TOutput> TryParse<TOutput>(this IParser<TOutput> parser, string input, int startPosition, int maxLength)
        => parser.TryParse(input, startPosition, maxLength, CancellationToken.None);

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="parser">The parser to use for parsing.</param>
    /// <param name="input">The input.</param>
    /// <param name="startPosition">The index of the first character to be parsed.</param>
    /// <param name="maxLength">The maximum length of input to be parsed.</param>
    /// <param name="cancellationToken">The cancellation token used for cancelling the parsing task.</param>
    /// <typeparam name="TOutput">The output type of the parser.</typeparam>
    /// <returns>The parsed result.</returns>
    /// <exception cref="ParseException">Thrown when the parser fails.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TOutput Parse<TOutput>(this IParser<TOutput> parser, string input, int startPosition, int maxLength, CancellationToken cancellationToken)
    {
        IParseResult<TOutput> result = parser.TryParse(input, startPosition, maxLength, cancellationToken);
        if (result.Success)
        {
            return result.Value!;
        }

        throw new ParseException(result.Error!.GetMessage());
    }

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="parser">The parser to use for parsing.</param>
    /// <param name="input">The input.</param>
    /// <param name="startPosition">The index of the first character to be parsed.</param>
    /// <param name="maxLength">The maximum length of input to be parsed.</param>
    /// <typeparam name="TOutput">The output type of the parser.</typeparam>
    /// <returns>The parsed result.</returns>
    /// <exception cref="ParseException">Thrown when the parser fails.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TOutput Parse<TOutput>(this IParser<TOutput> parser, string input, int startPosition, int maxLength)
        => parser.Parse(input, startPosition, maxLength, CancellationToken.None);

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="parser">The parser to use for parsing.</param>
    /// <param name="input">The input.</param>
    /// <param name="startPosition">The index of the first character to be parsed.</param>
    /// <param name="cancellationToken">The cancellation token used for cancelling the parsing task.</param>
    /// <typeparam name="TOutput">The output type of the parser.</typeparam>
    /// <returns>The result of running the parser.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParseResult<TOutput> TryParse<TOutput>(this IParser<TOutput> parser, string input, int startPosition, CancellationToken cancellationToken)
        => parser.TryParse(input, startPosition, input.Length - startPosition, cancellationToken);

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="parser">The parser to use for parsing.</param>
    /// <param name="input">The input.</param>
    /// <param name="startPosition">The index of the first character to be parsed.</param>
    /// <typeparam name="TOutput">The output type of the parser.</typeparam>
    /// <returns>The result of running the parser.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParseResult<TOutput> TryParse<TOutput>(this IParser<TOutput> parser, string input, int startPosition)
        => parser.TryParse(input, startPosition, CancellationToken.None);

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="parser">The parser to use for parsing.</param>
    /// <param name="input">The input.</param>
    /// <param name="startPosition">The index of the first character to be parsed.</param>
    /// <param name="cancellationToken">The cancellation token used for cancelling the parsing task.</param>
    /// <typeparam name="TOutput">The output type of the parser.</typeparam>
    /// <returns>The parsed result.</returns>
    /// <exception cref="ParseException">Thrown when the parser fails.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TOutput Parse<TOutput>(this IParser<TOutput> parser, string input, int startPosition, CancellationToken cancellationToken)
        => parser.Parse(input, startPosition, input.Length - startPosition, cancellationToken);

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="parser">The parser to use for parsing.</param>
    /// <param name="input">The input.</param>
    /// <param name="startPosition">The index of the first character to be parsed.</param>
    /// <typeparam name="TOutput">The output type of the parser.</typeparam>
    /// <returns>The parsed result.</returns>
    /// <exception cref="ParseException">Thrown when the parser fails.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TOutput Parse<TOutput>(this IParser<TOutput> parser, string input, int startPosition)
        => parser.Parse(input, startPosition, CancellationToken.None);

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="parser">The parser to use for parsing.</param>
    /// <param name="input">The input.</param>
    /// <param name="cancellationToken">The cancellation token used for cancelling the parsing task.</param>
    /// <typeparam name="TOutput">The output type of the parser.</typeparam>
    /// <returns>The result of running the parser.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParseResult<TOutput> TryParse<TOutput>(this IParser<TOutput> parser, string input, CancellationToken cancellationToken)
        => parser.TryParse(input, 0, cancellationToken);

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="parser">The parser to use for parsing.</param>
    /// <param name="input">The input.</param>
    /// <typeparam name="TOutput">The output type of the parser.</typeparam>
    /// <returns>The result of running the parser.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IParseResult<TOutput> TryParse<TOutput>(this IParser<TOutput> parser, string input)
        => parser.TryParse(input, CancellationToken.None);

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="parser">The parser to use for parsing.</param>
    /// <param name="input">The input.</param>
    /// <param name="cancellationToken">The cancellation token used for cancelling the parsing task.</param>
    /// <typeparam name="TOutput">The output type of the parser.</typeparam>
    /// <returns>The parsed result.</returns>
    /// <exception cref="ParseException">Thrown when the parser fails.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TOutput Parse<TOutput>(this IParser<TOutput> parser, string input, CancellationToken cancellationToken)
        => parser.Parse(input, 0, cancellationToken);

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="parser">The parser to use for parsing.</param>
    /// <param name="input">The input.</param>
    /// <typeparam name="TOutput">The output type of the parser.</typeparam>
    /// <returns>The parsed result.</returns>
    /// <exception cref="ParseException">Thrown when the parser fails.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TOutput Parse<TOutput>(this IParser<TOutput> parser, string input)
        => parser.Parse(input, CancellationToken.None);

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="parser">The parser to use for parsing.</param>
    /// <param name="input">The input.</param>
    /// <param name="startPosition">The index of the first character to be parsed.</param>
    /// <param name="maxLength">The maximum length of input to be parsed.</param>
    /// <param name="cancellationToken">The cancellation token used for cancelling the parsing task.</param>
    /// <typeparam name="TOutput">The output type of the parser.</typeparam>
    /// <returns>The result of running the parser.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<IParseResult<TOutput>> TryParseAsync<TOutput>(this IParser<TOutput> parser, string input, int startPosition, int maxLength, CancellationToken cancellationToken)
    {
        ParseUnit<TOutput> unit = new ParseUnit<TOutput>(input, startPosition, maxLength, parser);
        await unit.ParseAsync(cancellationToken).ConfigureAwait(false);
        return unit.Result;
    }

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="parser">The parser to use for parsing.</param>
    /// <param name="input">The input.</param>
    /// <param name="startPosition">The index of the first character to be parsed.</param>
    /// <param name="maxLength">The maximum length of input to be parsed.</param>
    /// <typeparam name="TOutput">The output type of the parser.</typeparam>
    /// <returns>The result of running the parser.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<IParseResult<TOutput>> TryParseAsync<TOutput>(this IParser<TOutput> parser, string input, int startPosition, int maxLength)
        => parser.TryParseAsync(input, startPosition, maxLength, CancellationToken.None);

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="parser">The parser to use for parsing.</param>
    /// <param name="input">The input.</param>
    /// <param name="startPosition">The index of the first character to be parsed.</param>
    /// <param name="maxLength">The maximum length of input to be parsed.</param>
    /// <param name="cancellationToken">The cancellation token used for cancelling the parsing task.</param>
    /// <typeparam name="TOutput">The output type of the parser.</typeparam>
    /// <returns>The parsed result.</returns>
    /// <exception cref="ParseException">Thrown when the parser fails.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<TOutput> ParseAsync<TOutput>(this IParser<TOutput> parser, string input, int startPosition, int maxLength, CancellationToken cancellationToken)
    {
        IParseResult<TOutput> result = await parser.TryParseAsync(input, startPosition, maxLength, cancellationToken);
        if (result.Success)
        {
            return result.Value!;
        }

        throw new ParseException(result.Error!.GetMessage());
    }

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="parser">The parser to use for parsing.</param>
    /// <param name="input">The input.</param>
    /// <param name="startPosition">The index of the first character to be parsed.</param>
    /// <param name="maxLength">The maximum length of input to be parsed.</param>
    /// <typeparam name="TOutput">The output type of the parser.</typeparam>
    /// <returns>The parsed result.</returns>
    /// <exception cref="ParseException">Thrown when the parser fails.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<TOutput> ParseAsync<TOutput>(this IParser<TOutput> parser, string input, int startPosition, int maxLength)
        => parser.ParseAsync(input, startPosition, maxLength, CancellationToken.None);

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="parser">The parser to use for parsing.</param>
    /// <param name="input">The input.</param>
    /// <param name="startPosition">The index of the first character to be parsed.</param>
    /// <param name="cancellationToken">The cancellation token used for cancelling the parsing task.</param>
    /// <typeparam name="TOutput">The output type of the parser.</typeparam>
    /// <returns>The result of running the parser.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<IParseResult<TOutput>> TryParseAsync<TOutput>(this IParser<TOutput> parser, string input, int startPosition, CancellationToken cancellationToken)
        => parser.TryParseAsync(input, startPosition, input.Length - startPosition, cancellationToken);

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="parser">The parser to use for parsing.</param>
    /// <param name="input">The input.</param>
    /// <param name="startPosition">The index of the first character to be parsed.</param>
    /// <typeparam name="TOutput">The output type of the parser.</typeparam>
    /// <returns>The result of running the parser.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<IParseResult<TOutput>> TryParseAsync<TOutput>(this IParser<TOutput> parser, string input, int startPosition)
        => parser.TryParseAsync(input, startPosition, CancellationToken.None);

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="parser">The parser to use for parsing.</param>
    /// <param name="input">The input.</param>
    /// <param name="startPosition">The index of the first character to be parsed.</param>
    /// <param name="cancellationToken">The cancellation token used for cancelling the parsing task.</param>
    /// <typeparam name="TOutput">The output type of the parser.</typeparam>
    /// <returns>The parsed result.</returns>
    /// <exception cref="ParseException">Thrown when the parser fails.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<TOutput> ParseAsync<TOutput>(this IParser<TOutput> parser, string input, int startPosition, CancellationToken cancellationToken)
        => parser.ParseAsync(input, startPosition, input.Length - startPosition, cancellationToken);

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="parser">The parser to use for parsing.</param>
    /// <param name="input">The input.</param>
    /// <param name="startPosition">The index of the first character to be parsed.</param>
    /// <typeparam name="TOutput">The output type of the parser.</typeparam>
    /// <returns>The parsed result.</returns>
    /// <exception cref="ParseException">Thrown when the parser fails.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<TOutput> ParseAsync<TOutput>(this IParser<TOutput> parser, string input, int startPosition)
        => parser.ParseAsync(input, startPosition, CancellationToken.None);

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="parser">The parser to use for parsing.</param>
    /// <param name="input">The input.</param>
    /// <param name="cancellationToken">The cancellation token used for cancelling the parsing task.</param>
    /// <typeparam name="TOutput">The output type of the parser.</typeparam>
    /// <returns>The result of running the parser.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<IParseResult<TOutput>> TryParseAsync<TOutput>(this IParser<TOutput> parser, string input, CancellationToken cancellationToken)
        => parser.TryParseAsync(input, 0, input.Length, cancellationToken);

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="parser">The parser to use for parsing.</param>
    /// <param name="input">The input.</param>
    /// <typeparam name="TOutput">The output type of the parser.</typeparam>
    /// <returns>The result of running the parser.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<IParseResult<TOutput>> TryParseAsync<TOutput>(this IParser<TOutput> parser, string input)
        => parser.TryParseAsync(input, CancellationToken.None);

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="parser">The parser to use for parsing.</param>
    /// <param name="input">The input.</param>
    /// <param name="cancellationToken">The cancellation token used for cancelling the parsing task.</param>
    /// <typeparam name="TOutput">The output type of the parser.</typeparam>
    /// <returns>The parsed result.</returns>
    /// <exception cref="ParseException">Thrown when the parser fails.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<TOutput> ParseAsync<TOutput>(this IParser<TOutput> parser, string input, CancellationToken cancellationToken)
        => parser.ParseAsync(input, 0, input.Length, cancellationToken);

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="parser">The parser to use for parsing.</param>
    /// <param name="input">The input.</param>
    /// <typeparam name="TOutput">The output type of the parser.</typeparam>
    /// <returns>The parsed result.</returns>
    /// <exception cref="ParseException">Thrown when the parser fails.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<TOutput> ParseAsync<TOutput>(this IParser<TOutput> parser, string input)
        => parser.ParseAsync(input, CancellationToken.None);
}
