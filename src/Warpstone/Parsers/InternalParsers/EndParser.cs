using System;
using System.Collections.Generic;

namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// Parser that checks for the end of the input stream.
    /// </summary>
    /// <seealso cref="Parser{T}" />
    internal class EndParser : Parser<object>
    {
        /// <inheritdoc/>
        public override IParseResult<object> TryParse(string input, int position)
        {
            if (position == input.Length)
            {
                return new ParseResult<object>(default, position, position);
            }

            return new ParseResult<object>(position, position, new UnexpectedTokenError(new SourcePosition(position, position), new string[] { string.Empty }, GetFound(input, position)));
        }

        /// <inheritdoc/>
        public override void FillSyntaxHighlightingGraph(Dictionary<object, HighlightingNode> graph)
        {
            if (graph.ContainsKey(this))
            {
                return;
            }

            graph.Add(this, new HighlightingNode(string.Empty, Highlight.None, Array.Empty<object>()));
        }

        public override string ToRegex2(Dictionary<object, string> names)
            => "";
    }
}
