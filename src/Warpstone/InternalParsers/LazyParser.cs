using System;

namespace Warpstone.InternalParsers
{
    /// <summary>
    /// A parser that is lazily instantiated.
    /// </summary>
    /// <typeparam name="T">Result type of the given parser.</typeparam>
    /// <seealso cref="Warpstone.Parser{T}" />
    internal class LazyParser<T> : Parser<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LazyParser{T}"/> class.
        /// </summary>
        /// <param name="parser">The parser.</param>
        internal LazyParser(Func<Parser<T>> parser)
            => Parser = new Lazy<Parser<T>>(parser);

        /// <summary>
        /// Gets the parser.
        /// </summary>
        internal Lazy<Parser<T>> Parser { get; }

        /// <inheritdoc/>
        internal override ParseResult<T> Parse(string input, int position)
            => Parser.Value.Parse(input, position);
    }
}
