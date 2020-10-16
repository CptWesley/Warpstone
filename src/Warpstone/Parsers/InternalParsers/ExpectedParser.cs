namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// A parser which replaced the expected string with a given string.
    /// </summary>
    /// <typeparam name="T">The result type of the parser.</typeparam>
    /// <seealso cref="Parser{T}" />
    internal class ExpectedParser<T> : Parser<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectedParser{T}"/> class.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="name">The name.</param>
        internal ExpectedParser(Parser<T> parser, string name)
        {
            Parser = parser;
            Name = name;
        }

        /// <summary>
        /// Gets the parser.
        /// </summary>
        internal Parser<T> Parser { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        internal string Name { get; }

        /// <inheritdoc/>
        internal override ParseResult<T> TryParse(string input, int position)
        {
            ParseResult<T> result = Parser.TryParse(input, position);
            if (result.Success)
            {
                return result;
            }

            return new ParseResult<T>(position, result.Position, new UnexpectedTokenError(new string[] { Name }, GetFound(input, position)));
        }
    }
}
