using System;
using System.Collections.Generic;

namespace Legacy.Warpstone1.Parsers.InternalParsers
{
    /// <summary>
    /// A parser which replaced the expected string with a given string.
    /// </summary>
    /// <typeparam name="T">The result type of the parser.</typeparam>
    /// <seealso cref="Parser{T}" />
    internal class ExpectedParser<T> : Parser<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectedParser{T}"/> class.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="transform">The transformation applied to the list of names.</param>
        internal ExpectedParser(IParser<T> parser, Func<IEnumerable<string>, IEnumerable<string>> transform)
        {
            Parser = parser;
            Transform = transform;
        }

        /// <summary>
        /// Gets the parser.
        /// </summary>
        internal IParser<T> Parser { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        internal Func<IEnumerable<string>, IEnumerable<string>> Transform { get; }

        /// <inheritdoc/>
        public override IParseResult<T> TryParse(string input, int position, bool collectTraces)
        {
            IParseResult<T> result = Parser.TryParse(input, position, collectTraces);
            if (result.Success)
            {
                return new ParseResult<T>(this, result.Value, input, result.Position.Start, result.Position.End, collectTraces ? new[] { result } : EmptyResults);
            }

            if (result.Error is UnexpectedTokenError ute)
            {
                return new ParseResult<T>(this, input, result.Position.Start, result.Position.End, new UnexpectedTokenError(result.Error!.Position, Transform(ute.Expected), GetFound(input, position)), collectTraces ? new[] { result } : EmptyResults);
            }

            return new ParseResult<T>(this, input, result.Position.Start, result.Position.End, result.Error, collectTraces ? new[] { result } : EmptyResults);
        }

        /// <inheritdoc/>
        public override string ToString(int depth)
        {
            if (depth < 0)
            {
                return "...";
            }

            return $"Expected({Parser.ToString(depth - 1)})";
        }
    }
}
