namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// A parser which doesn't allow backtracking out of the given inner parser.
    /// </summary>
    /// <typeparam name="T">The result type of the nested parser.</typeparam>
    internal class CutParser<T> : Parser<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CutParser{T} "/> class.
        /// </summary>
        /// <param name="parser">The parser that produces the value this parser returns.</param>
        internal CutParser(IParser<T> parser)
        {
            Parser = parser;
        }

        /// <summary>
        /// Gets the parser that produces the value this parser returns.
        /// </summary>
        internal IParser<T> Parser { get; }

        /// <inheritdoc/>
        public override IParseResult<T> TryParse(string input, int position)
        {
            IParseResult<T> innerResult = Parser.TryParse(input, position);

            if (innerResult.Success)
            {
                return new ParseResult<T>(this, innerResult.Value!, input, position, innerResult.Position.End, new[] { innerResult });
            }

            IParseError error = innerResult.Error!.DisallowBacktracking();
            return new ParseResult<T>(this, input, position, position, error, new[] { innerResult });
        }

        /// <inheritdoc/>
        public override string ToString(int depth)
        {
            if (depth < 0)
            {
                return "...";
            }

            return $"Cut({Parser.ToString(depth - 1)})";
        }
    }
}
