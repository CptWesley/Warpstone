using System;
using System.Collections.Generic;

namespace Warpstone.Internal.ParserExpressions
{
    /// <summary>
    /// Describes which parsers a parser may invoke at its own start position. Used to find left recursion.
    /// </summary>
    internal interface ILeftEdgeParser
    {
        /// <summary>
        /// Adds the parsers this parser may invoke before consuming any input to <paramref name="leftEdge"/>.
        /// </summary>
        /// <param name="isNullable">Tells whether a parser can succeed without consuming input.</param>
        /// <param name="leftEdge">The collection to add the parsers to.</param>
        /// <returns><see langword="true"/> if this parser can succeed without consuming input.</returns>
        public bool AnalyzeLeftEdge(Func<IParser, bool> isNullable, ICollection<IParser> leftEdge);
    }
}
