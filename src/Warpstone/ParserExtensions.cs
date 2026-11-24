using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Warpstone.Internal;
using Warpstone.Internal.SourceGeneration;

namespace Warpstone
{
    /// <summary>
    /// Provides extension methods for <see cref="IParser"/> and
    /// <see cref="IParser{T}"/> instances.
    /// </summary>
    public static class ParserExtensions
    {
        /// <summary>
        /// Generates the C# source of a parser that parses like <paramref name="parser"/>, without depending on Warpstone.
        /// The source declares a <c>static partial class</c> with <c>TryParse</c> and <c>Parse</c> methods taking a <see cref="string"/>.
        /// </summary>
        /// <remarks>
        /// The source of every transformation is copied from the call to <c>Transform</c> or <c>Aggregate</c> that created it,
        /// so transformations can only use their parameters, static members and the namespaces in <paramref name="usings"/>.
        /// The generated parser always runs recursively, so very deep input throws an <see cref="InsufficientExecutionStackException"/>.
        /// Parsers created by <c>Bind</c> (or <c>Then</c>, <c>ThenAdd</c> and <c>ThenSkip</c> taking a function) and <c>AsResult</c> are not supported.
        /// </remarks>
        /// <typeparam name="T">The result type of the parser.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="namespace">The namespace of the generated class.</param>
        /// <param name="className">The name of the generated class.</param>
        /// <param name="usings">The namespaces imported by the generated source.</param>
        /// <returns>The generated source.</returns>
        /// <exception cref="NotSupportedException">Thrown when the parser contains a parser that can't be generated.</exception>
        public static string GenerateSource<T>(this IParser<T> parser, string @namespace, string className, params string[] usings)
            => SourceEmitter.Generate(parser.MustNotBeNull(), @namespace.MustNotBeNull(), className.MustNotBeNull(), usings ?? []);

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

        /// <summary>
        /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>,
        /// awaiting more input instead of blocking when it is read from a <see cref="System.IO.TextReader"/>
        /// or <see cref="System.IO.Stream"/>.
        /// Always runs iteratively, whatever <see cref="ParseOptions.ExecutionMode"/> says,
        /// because only iterative execution can pause while waiting for input.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <param name="options">The options to use for parsing.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The parse result.</returns>
        public static Task<IParseResult<T>> TryParseAsync<T>(this IParser<T> parser, IParseInput input, ParseOptions options, CancellationToken cancellationToken = default)
            => new IterativeParseContext<T>(input, parser, options).RunToEndAsync(cancellationToken);

        /// <summary>
        /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>,
        /// awaiting more input instead of blocking when it is read from a <see cref="System.IO.TextReader"/>
        /// or <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The parse result.</returns>
        public static Task<IParseResult<T>> TryParseAsync<T>(this IParser<T> parser, IParseInput input, CancellationToken cancellationToken = default)
            => parser.TryParseAsync(input, ParseOptions.Default, cancellationToken);

        /// <summary>
        /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>,
        /// awaiting more input instead of blocking when it is read from a <see cref="System.IO.TextReader"/>
        /// or <see cref="System.IO.Stream"/>.
        /// Always runs iteratively, whatever <see cref="ParseOptions.ExecutionMode"/> says.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <param name="options">The options to use for parsing.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The parse result.</returns>
        public static async Task<T> ParseAsync<T>(this IParser<T> parser, IParseInput input, ParseOptions options, CancellationToken cancellationToken = default)
            => (await parser.TryParseAsync(input, options, cancellationToken).ConfigureAwait(false)).Value;

        /// <summary>
        /// Parses the given <paramref name="input"/> with the given <paramref name="parser"/>,
        /// awaiting more input instead of blocking when it is read from a <see cref="System.IO.TextReader"/>
        /// or <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The parse result.</returns>
        public static Task<T> ParseAsync<T>(this IParser<T> parser, IParseInput input, CancellationToken cancellationToken = default)
            => parser.ParseAsync(input, ParseOptions.Default, cancellationToken);
    }
}
