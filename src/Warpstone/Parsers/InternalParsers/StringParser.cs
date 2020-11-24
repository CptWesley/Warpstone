namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// Parser which parser a given string.
    /// </summary>
    /// <seealso cref="Warpstone.Parser{T}" />
    internal class StringParser : Parser<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringParser"/> class.
        /// </summary>
        /// <param name="str">The string to look for.</param>
        internal StringParser(string str)
            => String = str;

        /// <summary>
        /// Gets the regular expression.
        /// </summary>
        internal string String { get; }

        /// <inheritdoc/>
        public override IParseResult<string> TryParse(string input, int position)
        {
            if (!StringAtIndex(input, position))
            {
                return new ParseResult<string>(position, position, new UnexpectedTokenError(new string[] { $"'{String}'" }, GetFound(input, position)));
            }

            return new ParseResult<string>(String, position, position + String.Length);
        }

        private bool StringAtIndex(string input, int position)
        {
            if (position + String.Length > input.Length)
            {
                return false;
            }

            for (int i = 0; i < String.Length; i++)
            {
                if (String[i] != input[position + i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
