#pragma warning disable QW0011
#pragma warning disable QW0012

using System.Collections.Generic;

namespace Warpstone
{
    /// <summary>
    /// Holds the results of the parser analysis.
    /// </summary>
    public sealed class ParserAnalysisInfo : IParserAnalysisInfo
    {
        private readonly Dictionary<IParser, int> occurrenceCounts = new();
        private readonly Dictionary<IParser, int> recursiveOccurrenceCounts = new();

        /// <summary>
        /// For every parser visited so far, the number of parsers on the longest path down from it.
        /// Lets the analysis walk a shared parser once instead of once per path that reaches it.
        /// </summary>
        internal Dictionary<IParser, int> Heights { get; } = new();

        /// <inheritdoc />
        public int MaximumNestedParserDepth { get; set; }

        /// <inheritdoc />
        public bool HasRecursiveParsers { get; set; }

        /// <inheritdoc />
        public IDictionary<IParser, int> OccurrenceCounts => occurrenceCounts;

        /// <inheritdoc />
        public IDictionary<IParser, int> RecursiveOccurrenceCounts => recursiveOccurrenceCounts;

        /// <inheritdoc />
        IReadOnlyDictionary<IParser, int> IReadOnlyParserAnalysisInfo.OccurrenceCounts => occurrenceCounts;

        /// <inheritdoc />
        IReadOnlyDictionary<IParser, int> IReadOnlyParserAnalysisInfo.RecursiveOccurrenceCounts => recursiveOccurrenceCounts;
    }
}
