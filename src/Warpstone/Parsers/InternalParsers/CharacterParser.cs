using System;
using System.Collections.Generic;
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
        public override IParseResult<char> TryParse(string input, int position)
        {
            if (position >= input.Length || input[position] != Character)
            {
                return new ParseResult<char>(position, position, new UnexpectedTokenError(new SourcePosition(position, position), new string[] { $"'{Character}'" }, GetFound(input, position)));
            }

            return new ParseResult<char>(Character, position, position + 1);
        }

        /// <inheritdoc/>
        public override void FillSyntaxHighlightingGraph(Dictionary<object, HighlightingNode> graph)
        {
            if (graph.ContainsKey(this))
            {
                return;
            }

            graph.Add(this, new HighlightingNode(Regex.Escape(Character.ToString()), Highlight.None, Array.Empty<object>()));
        }

        public override string ToRegex2(Dictionary<object, string> names)
            => Regex.Escape(Character.ToString());
    }
}