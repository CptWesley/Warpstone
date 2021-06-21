namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// A parser which parses a given parser, if the specified condition holds.
    /// </summary>
    /// <typeparam name="T1">The result type of the condition that should fail.</typeparam>
    /// <typeparam name="T2">The result type of the nested parser.</typeparam>
    internal class BinaryNotParser<T1, T2> : Parser<T2>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryNotParser{T1, T2} "/> class.
        /// </summary>
        /// <param name="condition">The condition that must hold for in order to continue parsing.</param>
        /// <param name="parser">The parser that produces the value this parser returns.</param>
        internal BinaryNotParser(IParser<T1> condition, IParser<T2> parser)
        {
            Condition = condition;
            Parser = parser;
        }

        /// <summary>
        /// Gets the condition that must hold for in order to continue parsing.
        /// </summary>
        internal IParser<T1> Condition { get; }

        /// <summary>
        /// Gets the parser that produces the value this parser returns.
        /// </summary>
        internal IParser<T2> Parser { get; }

        /// <inheritdoc/>
        public override IParseResult<T2> TryParse(string input, int position)
        {
            IParseResult<T1> conditionResult = Condition.TryParse(input, position);

            if (conditionResult.Success)
            {
                return new ParseResult<T2>(position, position, new UnexpectedTokenError(new SourcePosition(position, position), new string[] { "<not>" }, GetFound(input, position)));
            }

            return Parser.TryParse(input, position);
        }
    }
}
