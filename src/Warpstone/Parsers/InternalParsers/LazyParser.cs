using System;

namespace Warpstone.Parsers.InternalParsers
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
        public override IParseResult<T> TryParse(string input, int position)
        {
            IParseResult<T> result = Parser.Value.TryParse(input, position);
            if (result.Success)
            {
                return new ParseResult<T>(this, result.Value, result.StartPosition, result.Position, new[] { result });
            }

            return new ParseResult<T>(this, result.StartPosition, result.Position, result.Error, new[] { result });
        }
    }
}
