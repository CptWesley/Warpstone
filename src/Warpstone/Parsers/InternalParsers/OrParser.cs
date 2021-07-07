using System;
using System.Collections.Generic;
using System.Linq;

namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// Parser that first applies the left parser and if it fails applies the right parser.
    /// </summary>
    /// <typeparam name="T">The parse result type of the parsers.</typeparam>
    /// <seealso cref="Parser{T}" />
    internal class OrParser<T> : Parser<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrParser{T}"/> class.
        /// </summary>
        /// <param name="first">The first parser that's tried.</param>
        /// <param name="second">The second parser that's applied if the first one fails.</param>
        internal OrParser(IParser<T> first, IParser<T> second)
        {
            First = first;
            Second = second;
        }

        /// <summary>
        /// Gets the first parser.
        /// </summary>
        internal IParser<T> First { get; }

        /// <summary>
        /// Gets the second parser.
        /// </summary>
        internal IParser<T> Second { get; }

        /// <inheritdoc/>
        public override IParseResult<T> TryParse(string input, int position)
        {
            IParseResult<T> firstResult = First.TryParse(input, position);
            if (firstResult.Success)
            {
                return new ParseResult<T>(this, firstResult.Value!, input, position, firstResult.Position.End, new[] { firstResult });
            }

            IParseResult<T> secondResult = Second.TryParse(input, position);
            if (secondResult.Success)
            {
                return new ParseResult<T>(this, secondResult.Value!, input, position, secondResult.Position.End, new[] { firstResult, secondResult });
            }

            IEnumerable<string> newExpected = Array.Empty<string>();
            if (firstResult.Error is UnexpectedTokenError t1 && firstResult.Position.End >= secondResult.Position.End)
            {
                newExpected = newExpected.Concat(t1.Expected);
            }

            if (secondResult.Error is UnexpectedTokenError t2 && firstResult.Position.End <= secondResult.Position.End)
            {
                newExpected = newExpected.Concat(t2.Expected);
            }

            if (firstResult.Position.End > secondResult.Position.End)
            {
                return new ParseResult<T>(this, input, position, firstResult.Position.End, new UnexpectedTokenError(firstResult.Error!.Position, newExpected, GetFound(input, firstResult.Error.Position.Start)), new[] { firstResult, secondResult });
            }

            return new ParseResult<T>(this, input, position, secondResult.Position.End, new UnexpectedTokenError(secondResult.Error!.Position, newExpected, GetFound(input, secondResult.Error.Position.Start)), new[] { firstResult, secondResult });
        }

        /// <inheritdoc/>
        public override string ToString(int depth)
        {
            if (depth < 0)
            {
                return "...";
            }

            return $"Or({First.ToString(depth - 1)}, {Second.ToString(depth - 1)})";
        }
    }
}
