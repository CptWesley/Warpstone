namespace Warpstone.InternalParsers
{
    /// <summary>
    /// Parser for conditional parsing.
    /// </summary>
    /// <typeparam name="T1">The result type of the condition parser.</typeparam>
    /// <typeparam name="T2">The result type of the then and else branch parsers.</typeparam>
    /// <seealso cref="Warpstone.Parser{T2}" />
    internal class ConditionalParser<T1, T2> : Parser<T2>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalParser{T1, T2}"/> class.
        /// </summary>
        /// <param name="conditionParser">The condition parser.</param>
        /// <param name="thenParser">The then parser.</param>
        /// <param name="elseParser">The else parser.</param>
        internal ConditionalParser(Parser<T1> conditionParser, Parser<T2> thenParser, Parser<T2> elseParser)
        {
            ConditionParser = conditionParser;
            ThenParser = thenParser;
            ElseParser = elseParser;
        }

        /// <summary>
        /// Gets the condition parser.
        /// </summary>
        internal Parser<T1> ConditionParser { get; }

        /// <summary>
        /// Gets the then parser.
        /// </summary>
        internal Parser<T2> ThenParser { get; }

        /// <summary>
        /// Gets the else parser.
        /// </summary>
        internal Parser<T2> ElseParser { get; }

        /// <inheritdoc/>
        internal override ParseResult<T2> Parse(string input, int position)
        {
            ParseResult<T1> conditionResult = ConditionParser.Parse(input, position);

            if (conditionResult.Success)
            {
                return ThenParser.Parse(input, conditionResult.Position);
            }

            return ElseParser.Parse(input, position);
        }
    }
}
