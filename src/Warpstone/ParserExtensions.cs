using System.Runtime.CompilerServices;

namespace Warpstone
{
    /// <summary>
    /// Provides extension methods for <see cref="IParser"/> and
    /// <see cref="IParser{T}"/> instances.
    /// </summary>
    public static class ParserExtensions
    {
        /// <summary>
        /// Creates a new <see cref="IParseContext"/> from the given <paramref name="input"/>
        /// and the initial <paramref name="parser"/>.
        /// </summary>
        /// <param name="parser">The parser that starts the parsing.</param>
        /// <param name="input">The input to be parsed.</param>
        /// <param name="options">The options to use for parsing.</param>
        /// <returns>The newly created <see cref="IParseContext"/> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParseContext CreateContext(this IParser parser, IParseInput input, ParseOptions options)
            => ParseContext.Create(input, parser, options);

        /// <summary>
        /// Creates a new <see cref="IParseContext{T}"/> from the given <paramref name="input"/>
        /// and the initial <paramref name="parser"/>.
        /// </summary>
        /// <param name="parser">The parser that starts the parsing.</param>
        /// <param name="input">The input to be parsed.</param>
        /// <param name="options">The options to use for parsing.</param>
        /// <typeparam name="T">The output type of the initial parser.</typeparam>
        /// <returns>The newly created <see cref="IParseContext{T}"/> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParseContext<T> CreateContext<T>(this IParser<T> parser, IParseInput input, ParseOptions options)
            => ParseContext.Create(input, parser, options);

        /// <summary>
        /// Creates a new <see cref="IParseContext"/> from the given <paramref name="input"/>
        /// and the initial <paramref name="parser"/>.
        /// </summary>
        /// <param name="parser">The parser that starts the parsing.</param>
        /// <param name="input">The input to be parsed.</param>
        /// <param name="options">The options to use for parsing.</param>
        /// <returns>The newly created <see cref="IParseContext"/> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParseContext CreateContext(this IParser parser, string input, ParseOptions options)
            => ParseContext.Create(input, parser, options);

        /// <summary>
        /// Creates a new <see cref="IParseContext{T}"/> from the given <paramref name="input"/>
        /// and the initial <paramref name="parser"/>.
        /// </summary>
        /// <param name="parser">The parser that starts the parsing.</param>
        /// <param name="input">The input to be parsed.</param>
        /// <param name="options">The options to use for parsing.</param>
        /// <typeparam name="T">The output type of the initial parser.</typeparam>
        /// <returns>The newly created <see cref="IParseContext{T}"/> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParseContext<T> CreateContext<T>(this IParser<T> parser, string input, ParseOptions options)
            => ParseContext.Create(input, parser, options);

        /// <summary>
        /// Creates a new <see cref="IParseContext"/> from the given <paramref name="input"/>
        /// and the initial <paramref name="parser"/>.
        /// </summary>
        /// <param name="parser">The parser that starts the parsing.</param>
        /// <param name="input">The input to be parsed.</param>
        /// <returns>The newly created <see cref="IParseContext"/> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParseContext<T> CreateContext<T>(this IParser<T> parser, IParseInput input)
            => ParseContext.Create(input, parser);

        /// <summary>
        /// Creates a new <see cref="IParseContext"/> from the given <paramref name="input"/>
        /// and the initial <paramref name="parser"/>.
        /// </summary>
        /// <param name="parser">The parser that starts the parsing.</param>
        /// <param name="input">The input to be parsed.</param>
        /// <returns>The newly created <see cref="IParseContext"/> instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParseContext<T> CreateContext<T>(this IParser<T> parser, string input)
            => ParseContext.Create(input, parser);

        /// <summary>
        /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <param name="options">The options to use for parsing.</param>
        /// <returns>The parse result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParseResult TryParse(this IParser parser, IParseInput input, ParseOptions options)
            => parser.CreateContext(input, options).RunToEnd();

        /// <summary>
        /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <param name="options">The options to use for parsing.</param>
        /// <returns>The parse result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParseResult<T> TryParse<T>(this IParser<T> parser, IParseInput input, ParseOptions options)
            => parser.CreateContext(input, options).RunToEnd();

        /// <summary>
        /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <param name="options">The options to use for parsing.</param>
        /// <returns>The parse result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParseResult TryParse(this IParser parser, string input, ParseOptions options)
            => parser.CreateContext(input, options).RunToEnd();

        /// <summary>
        /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <param name="options">The options to use for parsing.</param>
        /// <returns>The parse result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParseResult<T> TryParse<T>(this IParser<T> parser, string input, ParseOptions options)
            => parser.CreateContext(input, options).RunToEnd();

        /// <summary>
        /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <returns>The parse result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParseResult TryParse(this IParser parser, IParseInput input)
            => parser.CreateContext(input).RunToEnd();

        /// <summary>
        /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <returns>The parse result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParseResult<T> TryParse<T>(this IParser<T> parser, IParseInput input)
            => parser.CreateContext(input).RunToEnd();

        /// <summary>
        /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <returns>The parse result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParseResult TryParse(this IParser parser, string input)
            => parser.CreateContext(input).RunToEnd();

        /// <summary>
        /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <returns>The parse result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParseResult<T> TryParse<T>(this IParser<T> parser, string input)
            => parser.CreateContext(input).RunToEnd();

        /// <summary>
        /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <param name="options">The options to use for parsing.</param>
        /// <returns>The parse result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? Parse(this IParser parser, IParseInput input, ParseOptions options)
            => parser.TryParse(input, options).Value;

        /// <summary>
        /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <param name="options">The options to use for parsing.</param>
        /// <returns>The parse result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Parse<T>(this IParser<T> parser, IParseInput input, ParseOptions options)
            => parser.TryParse(input, options).Value;

        /// <summary>
        /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <param name="options">The options to use for parsing.</param>
        /// <returns>The parse result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? Parse(this IParser parser, string input, ParseOptions options)
            => parser.TryParse(input, options).Value;

        /// <summary>
        /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <param name="options">The options to use for parsing.</param>
        /// <returns>The parse result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Parse<T>(this IParser<T> parser, string input, ParseOptions options)
            => parser.TryParse(input, options).Value;

        /// <summary>
        /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <returns>The parse result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? Parse(this IParser parser, IParseInput input)
            => parser.TryParse(input).Value;

        /// <summary>
        /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <returns>The parse result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Parse<T>(this IParser<T> parser, IParseInput input)
            => parser.TryParse(input).Value;

        /// <summary>
        /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <returns>The parse result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? Parse(this IParser parser, string input)
            => parser.TryParse(input).Value;

        /// <summary>
        /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <returns>The parse result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Parse<T>(this IParser<T> parser, string input)
            => parser.TryParse(input).Value;
    }
}
