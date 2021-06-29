using System;
using System.Collections.Generic;
using System.Text;

namespace Warpstone.SyntaxHighlighting
{
    public class RegexLeaf : RegexNode
    {
        public RegexLeaf(string pattern, Highlight highlight)
            => (Pattern, Highlight) = (pattern, highlight);

        public Highlight Highlight { get; }

        public string Pattern { get; }

        public override int CountInnerGroups()
        {
            int result = 0;

            if (Highlight != Highlight.None)
            {
                result++;
            }

            return result;
        }

        internal override Dictionary<int, Highlight> GetCaptures(int group)
        {
            Dictionary<int, Highlight> result = new Dictionary<int, Highlight>();

            if (Highlight != Highlight.None)
            {
                result.Add(group, Highlight);
            }

            return result;
        }

        public override string ToRegex()
            => Highlight == Highlight.None ? Pattern : $"({Pattern})";

        public override bool References(RegexNode other)
            => false;
    }
}
