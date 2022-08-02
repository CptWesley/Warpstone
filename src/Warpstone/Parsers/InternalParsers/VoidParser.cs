namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// Parser that doesn't take any arguments and always succeeds.
    /// </summary>
    /// <typeparam name="T">The result type of the parser.</typeparam>
    /// <seealso cref="Parser{T}" />
    internal class VoidParser<T> : Parser<T>
    {
        /// <inheritdoc/>
        public override IParseResult<T> TryParse(string input, int position, bool collectTraces)
            => new ParseResult<T>(this, default, input, position, position, EmptyResults);

        /// <inheritdoc/>
        public override string ToString(int depth)
        {
            if (depth < 0)
            {
                return "...";
            }

            return $"Void()";
        }
    }
}
