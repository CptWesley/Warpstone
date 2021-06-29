using System.Collections.Generic;

namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// A parser which annotates a part of the grammar for syntax highlighting.
    /// </summary>
    /// <typeparam name="T">The result type of the parser.</typeparam>
    /// <seealso cref="Parser{T}" />
    internal class HighlightParser<T> : Parser<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightParser{T}"/> class.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="highlight">The syntax highlighting type.</param>
        internal HighlightParser(IParser<T> parser, Highlight highlight)
        {
            Parser = parser;
            Highlight = highlight;
        }

        /// <summary>
        /// Gets the parser.
        /// </summary>
        internal IParser<T> Parser { get; }

        /// <summary>
        /// Gets the syntax highlighting type.
        /// </summary>
        internal Highlight Highlight { get; }

        /// <inheritdoc/>
        public override IParseResult<T> TryParse(string input, int position)
            => Parser.TryParse(input, position);

        /// <inheritdoc/>
        public override void FillSyntaxHighlightingGraph(Dictionary<object, HighlightingNode> graph)
        {
            if (graph.ContainsKey(this))
            {
                return;
            }

            graph.Add(this, new HighlightingNode(string.Empty, Highlight, new object[] { Parser }));
            Parser.FillSyntaxHighlightingGraph(graph);
        }

        public override string ToRegex2(Dictionary<object, string> names)
            => Parser.ToRegex(names);
    }
}
