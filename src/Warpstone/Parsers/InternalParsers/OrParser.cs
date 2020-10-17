using System;
using System.Collections.Generic;
using System.Linq;

namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// Parser that first applies the left parser and if it fails applies the right parser.
    /// </summary>
    /// <typeparam name="T">The parse result type of the parsers.</typeparam>
    /// <seealso cref="Warpstone.Parser{T}" />
    internal class OrParser<T> : Parser<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrParser{T}"/> class.
        /// </summary>
        /// <param name="first">The first parser that's tried.</param>
        /// <param name="second">The second parser that's applied if the first one fails.</param>
        internal OrParser(Parser<T> first, Parser<T> second)
        {
            First = first;
            Second = second;
        }

        /// <summary>
        /// Gets the first parser.
        /// </summary>
        internal Parser<T> First { get; }

        /// <summary>
        /// Gets the second parser.
        /// </summary>
        internal Parser<T> Second { get; }

        /// <inheritdoc/>
        internal override IParseResult<T> TryParse(string input, int position)
        {
            IParseResult<T> firstResult = First.TryParse(input, position);
            if (firstResult.Success)
            {
                return firstResult;
            }

            IParseResult<T> secondResult = Second.TryParse(input, position);
            if (secondResult.Success)
            {
                return secondResult;
            }

            IEnumerable<string> newExpected = Array.Empty<string>();

            if (firstResult.Error is UnexpectedTokenError t1)
            {
                newExpected = newExpected.Concat(t1.Expected);
            }

            if (secondResult.Error is UnexpectedTokenError t2)
            {
                newExpected = newExpected.Concat(t2.Expected);
            }

            return new ParseResult<T>(position, secondResult.Position, new UnexpectedTokenError(newExpected, GetFound(input, position)));
        }
    }
}
