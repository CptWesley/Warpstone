using System.Text.RegularExpressions;

namespace Warpstone.InternalParsers
{
    /// <summary>
    /// Parser which parser a given regular expression pattern.
    /// </summary>
    /// <seealso cref="Warpstone.Parser{T}" />
    internal class RegexParser : Parser<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegexParser"/> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        internal RegexParser(string pattern)
            => Regex = new Regex(pattern);

        /// <summary>
        /// Gets the regular expression.
        /// </summary>
        internal Regex Regex { get; }

        /// <inheritdoc/>
        internal override ParseResult<string> Parse(string input, int position)
        {
            Match match = Regex.Match(input, position);

            if (!match.Success || match.Index != position)
            {
                return new ParseResult<string>();
            }

            return new ParseResult<string>(match.Value, match.Index, match.Index + match.Length);
        }
    }
}
