using System.Collections.Generic;

namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// Executes the wrapped parser multiple times and collects the results.
    /// </summary>
    /// <typeparam name="T1">The result type of the wrapped parser.</typeparam>
    /// <typeparam name="T2">The result type of the delimiter parser.</typeparam>
    internal class MultipleParser<T1, T2> : Parser<IList<T1>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleParser{T1, T2}"/> class.
        /// </summary>
        /// <param name="parser">The wrapped parser.</param>
        /// <param name="delimiter">The delimiter parser.</param>
        /// <param name="min">The minimum number of parsed items.</param>
        /// <param name="max">The maximum number of parsed items.</param>
        internal MultipleParser(IParser<T1> parser, IParser<T2> delimiter, ulong min, ulong max)
        {
            Parser = parser;
            DelimiterParser = delimiter;
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Gets the wrapped parser.
        /// </summary>
        internal IParser<T1> Parser { get; }

        /// <summary>
        /// Gets the delimiter parser.
        /// </summary>
        internal IParser<T2> DelimiterParser { get; }

        /// <summary>
        /// Gets the minimum number of parsed items.
        /// </summary>
        internal ulong Min { get; }

        /// <summary>
        /// Gets the maximum number of parsed items.
        /// </summary>
        internal ulong Max { get; }

        /// <inheritdoc/>
        public override IParseResult<IList<T1>> TryParse(string input, int position)
        {
            IList<T1> elements = new List<T1>();
            int newPosition = position;
            for (ulong i = 0; i < Min; i++)
            {
                IParseResult<T1> result = Parser.TryParse(input, newPosition);
                if (!result.Success)
                {
                    return new ParseResult<IList<T1>>(newPosition, result.Position, result.Error);
                }

                newPosition = result.Position;
                elements.Add(result.Value);

                if (i < Min - 1)
                {
                    IParseResult<T2> delimiterResult = DelimiterParser.TryParse(input, newPosition);
                    if (!delimiterResult.Success)
                    {
                        return new ParseResult<IList<T1>>(newPosition, result.Position, delimiterResult.Error);
                    }

                    newPosition = delimiterResult.Position;
                }
            }

            for (ulong i = 0; i < Max - Min; i++)
            {
                int tempPos = newPosition;
                if (i != 0 || Min > 0)
                {
                    IParseResult<T2> delimiterResult = DelimiterParser.TryParse(input, tempPos);
                    if (!delimiterResult.Success)
                    {
                        break;
                    }

                    tempPos = delimiterResult.Position;
                }

                IParseResult<T1> result = Parser.TryParse(input, tempPos);
                if (!result.Success)
                {
                    break;
                }

                newPosition = result.Position;
                elements.Add(result.Value);
            }

            return new ParseResult<IList<T1>>(elements, position, newPosition);
        }
    }
}