using System;
using System.Collections.Generic;
using System.Text;
using Warpstone.Internal;

namespace Warpstone.SyntaxHighlighting
{
    public class RegexOr : RegexNode
    {
        public RegexOr()
        {
        }

        public RegexOr(RegexNode left, RegexNode right)
            => (Left, Right) = (left, right);

        public RegexNode Left { get; set; }

        public RegexNode Right { get; set; }

        public override int CountInnerGroups()
            => Left.CountInnerGroups() + Right.CountInnerGroups() + 1;

        internal override Dictionary<int, Highlight> GetCaptures(int group)
        {
            return Left.GetCaptures(group + 1).Merge(Right.GetCaptures(group + 1 + Left.CountInnerGroups()));
        }

        public override string ToRegex()
        {
            string result = $"{Left.ToRegex()}|{Right.ToRegex()}";

            if (Recurses())
            {
                return $"(?<{Id}>{result})";
            }

            return $"({result})";
        }

        public override bool References(RegexNode other)
            => Left.References(other) || Right.References(other);
    }
}
