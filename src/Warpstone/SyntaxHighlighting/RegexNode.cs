using System;
using System.Collections.Generic;
using System.Text;

namespace Warpstone.SyntaxHighlighting
{
    public abstract class RegexNode
    {
        public string Id => "c" + GetHashCode();
        
        public abstract string ToRegex();

        public override string ToString()
            => ToRegex();

        internal abstract Dictionary<int, Highlight> GetCaptures(int group);

        public Dictionary<int, Highlight> GetCaptures()
            => GetCaptures(1);

        public abstract int CountInnerGroups();

        public abstract bool References(RegexNode other);

        public bool Recurses()
            => References(this);

        public RegexRef ToRef()
            => new RegexRef(this);
    }
}
