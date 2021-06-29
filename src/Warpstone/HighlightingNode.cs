using System.Collections.Generic;

namespace Warpstone
{
    /// <summary>
    /// Entries for building syntax highlighting graphs.
    /// </summary>
    public class HighlightingNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightingNode"/> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="highlight">The syntax highlighting type.</param>
        /// <param name="children">The children.</param>
        public HighlightingNode(string pattern, Highlight highlight, IEnumerable<object> children)
            => (Pattern, Highlight, Children) = (pattern, highlight, children);

        /// <summary>
        /// Gets the pattern.
        /// </summary>
        public string Pattern { get; }

        /// <summary>
        /// Gets the syntax highlighting type.
        /// </summary>
        public Highlight Highlight { get; }

        /// <summary>
        /// Gets the children.
        /// </summary>
        public IEnumerable<object> Children { get; }
    }
}
