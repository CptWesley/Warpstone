using System.Collections.Generic;

namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// Executes the wrapped parser multiple times and collects the results.
    /// </summary>
    /// <typeparam name="T1">The result type of the wrapped parser.</typeparam>
    /// <typeparam name="T2">The result type of the delimiter parser.</typeparam>
    /// <typeparam name="T3">The result type of the terminator parser.</typeparam>
    internal class MultipleParser<T1, T2, T3> : Parser<IList<T1>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleParser{T1, T2, T3}"/> class.
        /// </summary>
        /// <param name="parser">The wrapped parser.</param>
        /// <param name="delimiter">The delimiter parser.</param>
        /// <param name="min">The minimum number of parsed items.</param>
        /// <param name="max">The maximum number of parsed items.</param>
        /// <param name="terminator">The terminator parser.</param>
        internal MultipleParser(IParser<T1> parser, IParser<T2> delimiter, IParser<T3> terminator, ulong min, ulong max)
        {
            Parser = parser;
            DelimiterParser = delimiter;
            Min = min;
            Max = max;
            TerminatorParser = terminator;
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
        /// Gets the terminator parser.
        /// </summary>
        internal IParser<T3> TerminatorParser { get; }

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
            IParseError? error = null;
            int errorStartPos = -1;
            int errorEndPos = -1;
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
                elements.Add(result.Value!);

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
                        error = delimiterResult.Error;
                        errorStartPos = delimiterResult.StartPosition;
                        errorEndPos = delimiterResult.Position;
                        break;
                    }

                    tempPos = delimiterResult.Position;
                }

                IParseResult<T1> result = Parser.TryParse(input, tempPos);
                if (!result.Success)
                {
                    error = result.Error;
                    errorStartPos = result.StartPosition;
                    errorEndPos = result.Position;
                    break;
                }

                newPosition = result.Position;
                elements.Add(result.Value!);
            }

            IParseResult<T3> terminatorResult = TerminatorParser.TryParse(input, newPosition);
            if (!terminatorResult.Success)
            {
                if (error == null)
                {
                    return new ParseResult<IList<T1>>(terminatorResult.StartPosition, terminatorResult.Position, terminatorResult.Error);
                }

                return new ParseResult<IList<T1>>(errorStartPos, errorEndPos, error);
            }

            return new ParseResult<IList<T1>>(elements, position, terminatorResult.Position);
        }
    }
}