using System;

namespace Legacy.Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// A parser that is lazily instantiated.
    /// </summary>
    /// <typeparam name="T">Result type of the given parser.</typeparam>
    /// <seealso cref="Parser{T}" />
    internal class LazyParser<T> : Parser<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LazyParser{T}"/> class.
        /// </summary>
        /// <param name="parser">The parser.</param>
        internal LazyParser(Func<IParser<T>> parser)
            => Parser = new Lazy<IParser<T>>(parser);

        /// <summary>
        /// Gets the parser.
        /// </summary>
        internal Lazy<IParser<T>> Parser { get; }

        /// <inheritdoc/>
        public override IParseResult<T> TryParse(string input, int position, bool collectTraces)
        {
            IParseResult<T> result = Parser.Value.TryParse(input, position, collectTraces);
            if (result.Success)
            {
                return new ParseResult<T>(this, result.Value, input, result.Position.Start, result.Position.End, collectTraces ? new[] { result } : EmptyResults);
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

            return $"Lazy({Parser.Value.ToString(depth - 1)})";
        }
    }
}
