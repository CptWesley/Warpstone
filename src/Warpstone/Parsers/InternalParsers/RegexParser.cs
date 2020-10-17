using System.Text.RegularExpressions;

namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// Parser which parser a given regular expression pattern.
    /// </summary>
    /// <seealso cref="Warpstone.Parser{T}" />
    internal class RegexParser : Parser<string>
    {
        private readonly Regex regex;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexParser"/> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        internal RegexParser(string pattern)
        {
            Pattern = pattern;
            regex = new Regex(pattern, RegexOptions.ExplicitCapture);
        }

        /// <summary>
        /// Gets the regular expression.
        /// </summary>
        internal string Pattern { get; }

        /// <inheritdoc/>
        internal override IParseResult<string> TryParse(string input, int position)
        {
            Match match = regex.Match(input, position);

            if (!match.Success || match.Index != position)
            {
                return new ParseResult<string>(position, position, new UnexpectedTokenError(new string[] { $"'{Pattern}'" }, GetFound(input, position)));
            }

            return new ParseResult<string>(match.Value, match.Index, match.Index + match.Length);
        }
    }
}
