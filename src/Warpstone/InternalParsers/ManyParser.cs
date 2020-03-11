using System.Collections.Generic;

namespace Warpstone.InternalParsers
{
    /// <summary>
    /// Executes the wrapped parser multiple times and collects the results.
    /// </summary>
    /// <typeparam name="T">The result type of the wrapped parser.</typeparam>
    internal class ManyParser<T> : Parser<IEnumerable<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManyParser{T}"/> class.
        /// </summary>
        /// <param name="parser">The wrapped parser.</param>
        internal ManyParser(Parser<T> parser)
            => Parser = parser;

        /// <summary>
        /// Gets the wrapped parser.
        /// </summary>
        internal Parser<T> Parser { get; }

        /// <inheritdoc/>
        internal override ParseResult<IEnumerable<T>> Parse(string input, int position)
        {
            List<T> elements = new List<T>();
            int newPosition = position;
            ParseResult<T> result;
            while (true)
            {
                result = Parser.Parse(input, newPosition);
                if (!result.Success)
                {
                    break;
                }

                newPosition = result.Position;
                elements.Add(result.Value);
            }

            return new ParseResult<IEnumerable<T>>(elements, position, newPosition);
        }
    }
}