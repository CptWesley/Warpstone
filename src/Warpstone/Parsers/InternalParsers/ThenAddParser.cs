namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// A Parser applying two parsers and retaining both results.
    /// </summary>
    /// <typeparam name="T1">The result type of the first parser.</typeparam>
    /// <typeparam name="T2">The result type of the second parser.</typeparam>
    /// <seealso cref="Parser{T}" />
    internal class ThenAddParser<T1, T2> : Parser<(T1 First, T2 Second)>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThenAddParser{T1, T2}"/> class.
        /// </summary>
        /// <param name="first">The first parser that's tried.</param>
        /// <param name="second">The second parser that's applied if the first one fails.</param>
        internal ThenAddParser(IParser<T1> first, IParser<T2> second)
        {
            First = first;
            Second = second;
        }

        /// <summary>
        /// Gets the first parser.
        /// </summary>
        internal IParser<T1> First { get; }

        /// <summary>
        /// Gets the second parser.
        /// </summary>
        internal IParser<T2> Second { get; }

        /// <inheritdoc/>
        public override IParseResult<(T1 First, T2 Second)> TryParse(string input, int position)
        {
            IParseResult<T1> firstResult = First.TryParse(input, position);
            if (!firstResult.Success)
            {
                return new ParseResult<(T1, T2)>(this, input, firstResult.Position.Start, firstResult.Position.End, firstResult.Error, new[] { firstResult });
            }

            IParseResult<T2> secondResult = Second.TryParse(input, firstResult.Position.End);
            if (!secondResult.Success)
            {
                return new ParseResult<(T1, T2)>(this, input, secondResult.Position.Start, secondResult.Position.End, secondResult.Error, new IParseResult[] { firstResult, secondResult });
            }

            return new ParseResult<(T1, T2)>(this, (firstResult.Value!, secondResult.Value!), input, position, secondResult.Position.End, new IParseResult[] { firstResult, secondResult });
        }
    }
}
