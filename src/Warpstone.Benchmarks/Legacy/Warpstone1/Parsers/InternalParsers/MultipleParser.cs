#nullable enable
#pragma warning disable

using System.Collections.Generic;

namespace Legacy.Warpstone1.Parsers.InternalParsers
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
        public override IParseResult<IList<T1>> TryParse(string input, int position, bool collectTraces)
        {
            IParseError? error = null;
            int errorStartPos = -1;
            int errorEndPos = -1;
            IList<T1> elements = new List<T1>();
            List<IParseResult> trace = new List<IParseResult>();
            int newPosition = position;
            for (ulong i = 0; i < Min; i++)
            {
                IParseResult<T1> result = Parser.TryParse(input, newPosition, collectTraces);
                trace.Add(result);
                if (!result.Success)
                {
                    return new ParseResult<IList<T1>>(this, input, newPosition, result.Position.End, result.Error, collectTraces ? trace : EmptyResults);
                }

                newPosition = result.Position.End;
                elements.Add(result.Value!);

                if (i < Min - 1)
                {
                    IParseResult<T2> delimiterResult = DelimiterParser.TryParse(input, newPosition, collectTraces);
                    trace.Add(delimiterResult);
                    if (!delimiterResult.Success)
                    {
                        return new ParseResult<IList<T1>>(this, input, newPosition, result.Position.End, delimiterResult.Error, collectTraces ? trace : EmptyResults);
                    }

                    newPosition = delimiterResult.Position.End;
                }
            }

            for (ulong i = 0; i < Max - Min; i++)
            {
                int tempPos = newPosition;
                IParseResult<T2>? delimiterResult = null;
                if (i != 0 || Min > 0)
                {
                    delimiterResult = DelimiterParser.TryParse(input, tempPos, collectTraces);
                    if (!delimiterResult.Success)
                    {
                        error = delimiterResult.Error;
                        errorStartPos = delimiterResult.Position.Start;
                        errorEndPos = delimiterResult.Position.End;
                        break;
                    }

                    tempPos = delimiterResult.Position.End;
                }

                IParseResult<T1> result = Parser.TryParse(input, tempPos, collectTraces);
                if (!result.Success)
                {
                    error = result.Error;
                    errorStartPos = result.Position.Start;
                    errorEndPos = result.Position.End;
                    break;
                }

                newPosition = result.Position.End;
                elements.Add(result.Value!);

                if (delimiterResult != null)
                {
                    trace.Add(delimiterResult);
                }

                trace.Add(result);
            }

            IParseResult<T3> terminatorResult = TerminatorParser.TryParse(input, newPosition, collectTraces);
            trace.Add(terminatorResult);
            if (!terminatorResult.Success)
            {
                if (error == null)
                {
                    return new ParseResult<IList<T1>>(this, input, terminatorResult.Position.Start, terminatorResult.Position.End, terminatorResult.Error, collectTraces ? trace : EmptyResults);
                }

                return new ParseResult<IList<T1>>(this, input, errorStartPos, errorEndPos, error, collectTraces ? trace : EmptyResults);
            }

            return new ParseResult<IList<T1>>(this, elements, input, position, terminatorResult.Position.End, collectTraces ? trace : EmptyResults);
        }

        /// <inheritdoc/>
        public override string ToString(int depth)
        {
            if (depth < 0)
            {
                return "...";
            }

            return $"Multiple({Parser.ToString(depth - 1)}, {DelimiterParser.ToString(depth - 1)}, {TerminatorParser.ToString(depth - 1)})";
        }
    }
}
