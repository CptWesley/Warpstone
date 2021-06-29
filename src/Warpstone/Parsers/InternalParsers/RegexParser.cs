using System;
using System.Collections.Generic;
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
                return new ParseResult<string>(position, position, new UnexpectedTokenError(new SourcePosition(position, position), new string[] { $"'{Pattern}'" }, GetFound(input, position)));
            }

            return new ParseResult<string>(match.Value, match.Index, match.Index + match.Length);
        }

        /// <inheritdoc/>
        public override void FillSyntaxHighlightingGraph(Dictionary<object, HighlightingNode> graph)
        {
            if (graph.ContainsKey(this))
            {
                return;
            }

            graph.Add(this, new HighlightingNode(Pattern, Highlight.None, Array.Empty<object>()));
        }

        public override string ToRegex2(Dictionary<object, string> names)
            => Pattern;
    }
}
