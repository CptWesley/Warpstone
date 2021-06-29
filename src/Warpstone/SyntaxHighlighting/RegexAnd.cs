using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Warpstone.Internal;

namespace Warpstone.SyntaxHighlighting
{
    public class RegexAnd : RegexNode
    {
        public RegexAnd()
        {
        }

        public RegexAnd(RegexNode left, RegexNode right)
            => (Left, Right) = (left, right);

        public RegexNode Left { get; set; }

        public RegexNode Right { get; set; }

        public override int CountInnerGroups()
        {
            int result = Left.CountInnerGroups() + Right.CountInnerGroups();

            if (Recurses())
            {
                result++;
            }

            return result;
        }

        internal override Dictionary<int, Highlight> GetCaptures(int group)
        {
            int start = group;

            if (Recurses())
            {
                start++;
            }

            return Left.GetCaptures(start).Merge(Right.GetCaptures(start + Left.CountInnerGroups()));
        }

        public override string ToRegex()
        {
            string result = $"{Left.ToRegex()}{Right.ToRegex()}";

            if (Recurses())
            {
                result = $"(?<{Id}>{result})";
            }

            return result;
        }

        public override bool References(RegexNode other)
            => Left.References(other) || Right.References(other);
    }
}
