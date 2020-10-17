namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// A Parser applying two parsers and retaining both results.
    /// </summary>
    /// <typeparam name="T1">The result type of the first parser.</typeparam>
    /// <typeparam name="T2">The result type of the second parser.</typeparam>
    /// <seealso cref="Warpstone.Parser{T}" />
    internal class ThenAddParser<T1, T2> : Parser<(T1, T2)>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThenAddParser{T1, T2}"/> class.
        /// </summary>
        /// <param name="first">The first parser that's tried.</param>
        /// <param name="second">The second parser that's applied if the first one fails.</param>
        internal ThenAddParser(Parser<T1> first, Parser<T2> second)
        {
            First = first;
            Second = second;
        }

        /// <summary>
        /// Gets the first parser.
        /// </summary>
        internal Parser<T1> First { get; }

        /// <summary>
        /// Gets the second parser.
        /// </summary>
        internal Parser<T2> Second { get; }

        /// <inheritdoc/>
        internal override IParseResult<(T1, T2)> TryParse(string input, int position)
        {
            IParseResult<T1> firstResult = First.TryParse(input, position);
            if (!firstResult.Success)
            {
                return new ParseResult<(T1, T2)>(position, firstResult.Position, firstResult.Error);
            }

            IParseResult<T2> secondResult = Second.TryParse(input, firstResult.Position);
            if (!secondResult.Success)
            {
                return new ParseResult<(T1, T2)>(position, secondResult.Position, secondResult.Error);
            }

            return new ParseResult<(T1, T2)>((firstResult.Value, secondResult.Value), position, secondResult.Position);
        }
    }
}
