using System;

namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// Parser that doesn't take any arguments and always fails.
    /// </summary>
    /// <typeparam name="T">The result type of the parser.</typeparam>
    /// <seealso cref="Parser{T}" />
    internal class FailureParser<T> : Parser<T>
    {
        /// <inheritdoc/>
        public override IParseResult<T> TryParse(string input, int position)
            => new ParseResult<T>(this, position, position, new UnexpectedTokenError(new SourcePosition(position, position), Array.Empty<string>(), string.Empty), Array.Empty<IParseResult>());
    }
}
