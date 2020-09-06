using System;

namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// Parser that doesn't take any arguments and always fails.
    /// </summary>
    /// <typeparam name="T">The result type of the parser.</typeparam>
    /// <seealso cref="Warpstone.Parser{T}" />
    internal class FailureParser<T> : Parser<T>
    {
        /// <inheritdoc/>
        internal override ParseResult<T> TryParse(string input, int position)
            => new ParseResult<T>(position, position, new ParseError(Array.Empty<string>(), string.Empty));
    }
}
