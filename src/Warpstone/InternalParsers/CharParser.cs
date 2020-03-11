namespace Warpstone.InternalParsers
{
    /// <summary>
    /// A parser for parsing single characters.
    /// </summary>
    internal class CharParser : Parser<char>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharParser"/> class.
        /// </summary>
        /// <param name="c">The character expecting in the parser.</param>
        internal CharParser(char c)
            => Character = c;

        /// <summary>
        /// Gets the expected character.
        /// </summary>
        internal char Character { get; }

        /// <inheritdoc/>
        internal override ParseResult<char> Parse(string input, int position)
        {
            if (position < input.Length && input[position] == Character)
            {
                return new ParseResult<char>(input[position], position, position + 1);
            }

            return new ParseResult<char>();
        }
    }
}