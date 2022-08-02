using System;
using System.Text.RegularExpressions;

namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// Parser which parser a given character.
    /// </summary>
    /// <seealso cref="Parser{T}" />
    internal class CharacterParser : Parser<char>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterParser"/> class.
        /// </summary>
        /// <param name="c">The character to look for.</param>
        internal CharacterParser(char c)
            => Character = c;

        /// <summary>
        /// Gets the regular expression.
        /// </summary>
        internal char Character { get; }

        /// <inheritdoc/>
        public override IParseResult<char> TryParse(string input, int position, bool collectTraces)
        {
            if (position >= input.Length || input[position] != Character)
            {
                return new ParseResult<char>(this, input, position, position, new UnexpectedTokenError(new SourcePosition(input, position, position), new string[] { $"'{Regex.Escape(Character.ToString())}'" }, GetFound(input, position)), Array.Empty<IParseResult>());
            }

            return new ParseResult<char>(this, Character, input, position, position + 1, Array.Empty<IParseResult>());
        }

        /// <inheritdoc/>
        public override string ToString(int depth)
        {
            if (depth < 0)
            {
                return "...";
            }

            return $"Character('{Regex.Escape(Character.ToString())}')";
        }
    }
}