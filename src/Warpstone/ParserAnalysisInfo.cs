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
