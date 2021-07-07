namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// A parser which parses a given parser, if the specified condition holds.
    /// The output of this parser should not be directly consumed.
    /// </summary>
    /// <typeparam name="T">The result type of the nested parser.</typeparam>
    internal class NotParser<T> : Parser<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotParser{T} "/> class.
        /// </summary>
        /// <param name="parser">The parser that produces the value this parser returns.</param>
        internal NotParser(IParser<T> parser)
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
            IParseResult<T> conditionResult = Parser.TryParse(input, position);

            if (conditionResult.Success)
            {
                return new ParseResult<T>(this, input, position, position, new UnexpectedTokenError(new SourcePosition(input, position, position), new string[] { "<not>" }, GetFound(input, position)), new[] { conditionResult });
            }

            return new ParseResult<T>(this, default, input, position, position, new[] { conditionResult });
        }
    }
}
