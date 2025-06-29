using System;

namespace Warpstone.Internal.ParserExpressions
{
    /// <summary>
    /// Marker interface for lazy parsers.
    /// </summary>
    internal interface ILazyParser : IParser
    {
        /// <summary>
        /// The lazily executed parser.
        /// </summary>
        public Lazy<IParser> Parser { get; }
    }

    /// <summary>
    /// Marker interface for lazy parsers.
    /// </summary>
    /// <typeparam name="T">The return type of the wrapped parser.</typeparam>
    internal interface ILazyParser<T> : ILazyParser
    {
        /// <inheritdoc cref="ILazyParser.Parser" />
        public new Lazy<IParser<T>> Parser { get; }
    }
}
