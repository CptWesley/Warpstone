using System;
using System.Text.RegularExpressions;

namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// Parser which parser a given regular expression pattern.
    /// </summary>
    /// <seealso cref="Parser{T}" />
    internal class RegexParser : Parser<string>
    {
        private readonly Regex regex;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexParser"/> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="compiled">Indicates that the regex engine should be compiled.</param>
        internal RegexParser(string pattern, bool compiled)
        {
            Pattern = pattern;
            RegexOptions options = RegexOptions.ExplicitCapture;

            if (compiled)
            {
                options |= RegexOptions.Compiled;
            }

            regex = new Regex(@"\G" + pattern, options);
        }

        /// <summary>
        /// Gets the regular expression.
        /// </summary>
        internal string Pattern { get; }

        /// <inheritdoc/>
        public override IParseResult<string> TryParse(string input, int position)
        {
            Match match = regex.Match(input, position);

            if (!match.Success)
            {
                return new ParseResult<string>(this, input, position, position, new UnexpectedTokenError(new SourcePosition(input, position, position), new string[] { $"'{Pattern}'" }, GetFound(input, position)), Array.Empty<IParseResult<object>>());
            }

            return new ParseResult<string>(this, match.Value, input, match.Index, match.Index + match.Length, Array.Empty<IParseResult>());
        }

        /// <inheritdoc/>
        public override string ToString(int depth)
        {
            if (depth < 0)
            {
                return "...";
            }

            return $"Regex(\"{Regex.Escape(Pattern)}\")";
        }
    }
}
