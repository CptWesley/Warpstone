using System;
using System.Collections.Generic;
using System.Text;

namespace Warpstone.SyntaxHighlighting
{
    public class RegexRef : RegexNode
    {
        public RegexRef(RegexNode node)
            => Ref = node;

        public RegexNode Ref { get; }

        public override int CountInnerGroups()
            => 0;

        internal override Dictionary<int, Highlight> GetCaptures(int group)
            => new Dictionary<int, Highlight>();

        public override string ToRegex()
            => $"\\g<{Ref.Id}>";

        public override bool References(RegexNode other)
            => Ref == other;
    }
}
