﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// Parser which parser a given string.
    /// </summary>
    /// <seealso cref="Parser{T}" />
    internal class StringParser : Parser<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringParser"/> class.
        /// </summary>
        /// <param name="str">The string to look for.</param>
        /// <param name="stringComparison">The string comparison method to use.</param>
        internal StringParser(string str, StringComparison? stringComparison = null)
        {
            String = str;
            StringComparison = stringComparison;
        }

        /// <summary>
        /// Gets the regular expression.
        /// </summary>
        internal string String { get; }

        /// <summary>
        /// Gets the string comparison method.
        /// </summary>
        internal StringComparison? StringComparison { get; }

        /// <inheritdoc/>
        public override IParseResult<string> TryParse(string input, int position)
        {
            if (!StringAtIndex(input, position))
            {
                return new ParseResult<string>(position, position, new UnexpectedTokenError(new SourcePosition(position, position), new string[] { $"'{String}'" }, GetFound(input, position)));
            }

            return new ParseResult<string>(String, position, position + String.Length);
        }

        /// <inheritdoc/>
        public override void FillSyntaxHighlightingGraph(Dictionary<object, HighlightingNode> graph)
        {
            if (graph.ContainsKey(this))
            {
                return;
            }

            graph.Add(this, new HighlightingNode(Regex.Escape(String), Highlight.None, Array.Empty<object>()));
        }

        public override string ToRegex2(Dictionary<object, string> names)
            => Regex.Escape(String);

        private bool StringAtIndex(string input, int position)
        {
            if (position + String.Length > input.Length)
            {
                return false;
            }

            if (StringComparison.HasValue)
            {
                return String.Equals(input.Substring(position, String.Length), StringComparison.Value);
            }
            else
            {
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
}
