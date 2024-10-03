using System;

namespace Legacy.Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// Parser that doesn't take any arguments and always fails.
    /// </summary>
    /// <typeparam name="T">The result type of the parser.</typeparam>
    /// <seealso cref="Parser{T}" />
    internal class FailureParser<T> : Parser<T>
    {
        /// <inheritdoc/>
        public override IParseResult<T> TryParse(string input, int position, bool collectTraces)
            => new ParseResult<T>(this, input, position, position, new UnexpectedTokenError(new SourcePosition(input, position, position), Array.Empty<string>(), string.Empty), EmptyResults);

        /// <inheritdoc/>
        public override string ToString(int depth)
        {
            if (depth < 0)
            {
                return "...";
            }

            return $"Failure()";
        }
    }
}
