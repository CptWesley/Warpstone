using System.Collections.Generic;

namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// Executes the wrapped parser multiple times and collects the results.
    /// </summary>
    /// <typeparam name="T1">The result type of the wrapped parser.</typeparam>
    /// <typeparam name="T2">The result type of the delimiter parser.</typeparam>
    internal class OneOrMoreParser<T1, T2> : Parser<IEnumerable<T1>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneOrMoreParser{T1, T2}"/> class.
        /// </summary>
        /// <param name="parser">The wrapped parser.</param>
        /// <param name="delimiter">The delimiter parser.</param>
        internal OneOrMoreParser(IParser<T1> parser, IParser<T2> delimiter)
        {
            Parser = parser;
            DelimiterParser = delimiter;
        }

        /// <summary>
        /// Gets the wrapped parser.
        /// </summary>
        internal IParser<T1> Parser { get; }

        /// <summary>
        /// Gets the delimiter parser.
        /// </summary>
        internal IParser<T2> DelimiterParser { get; }

        /// <inheritdoc/>
        public override IParseResult<IEnumerable<T1>> TryParse(string input, int position)
        {
            List<T1> elements = new List<T1>();
            IParseResult<T1> result = Parser.TryParse(input, position);
            if (!result.Success)
            {
                return new ParseResult<IEnumerable<T1>>(position, result.Position, result.Error);
            }

            elements.Add(result.Value);
            int newPosition = result.Position;

            while (true)
            {
                IParseResult<T2> delimiterResult = DelimiterParser.TryParse(input, newPosition);
                if (!delimiterResult.Success)
                {
                    break;
                }

                result = Parser.TryParse(input, delimiterResult.Position);
                if (!result.Success)
                {
                    break;
                }

                newPosition = result.Position;
                elements.Add(result.Value);
            }

            return new ParseResult<IEnumerable<T1>>(elements, position, newPosition);
        }
    }
}