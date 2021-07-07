namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// Parser that attempts a parse, but does not record it.
    /// </summary>
    /// <typeparam name="T">The result type of the parser.</typeparam>
    /// <seealso cref="Parser{T}" />
    internal class PeekParser<T> : Parser<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PeekParser{T}"/> class.
        /// </summary>
        /// <param name="parser">The parser.</param>
        internal PeekParser(IParser<T> parser)
            => Parser = parser;

        /// <summary>
        /// Gets the parser.
        /// </summary>
        internal IParser<T> Parser { get; }

        /// <inheritdoc/>
        public override IParseResult<T> TryParse(string input, int position)
        {
            IParseResult<T> parseResult = Parser.TryParse(input, position);

            if (parseResult.Success)
            {
                return new ParseResult<T>(this, parseResult.Value, input, position, position, new[] { parseResult });
            }

            return new ParseResult<T>(this, input, position, parseResult.Position.End, parseResult.Error, new[] { parseResult });
        }

        /// <inheritdoc/>
        public override string ToString(int depth)
        {
            if (depth < 0)
            {
                return "...";
            }

            return $"Peek({Parser.ToString(depth - 1)})";
        }
    }
}
