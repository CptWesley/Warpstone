using System.Collections.Generic;

namespace Warpstone
{
    /// <summary>
    /// Provides a read-only interface of the parser analysis results.
    /// </summary>
    public interface IReadOnlyParserAnalysisInfo
    {
        /// <summary>
        /// Indicates the maximum nested parser depth.
        /// </summary>
        public int MaximumNestedParserDepth { get; }

        /// <summary>
        /// Indicates whether or not any recursive parsers have been found.
        /// </summary>
        public bool HasRecursiveParsers { get; }

        /// <summary>
        /// Provides the number of occurrences that each parser has in the represented grammar.
        /// </summary>
        public IReadOnlyDictionary<IParser, int> OccurrenceCounts { get; }

        /// <summary>
        /// Provides the number of recursive occurrences that each parser has in the represented grammar.
        /// </summary>
        public IReadOnlyDictionary<IParser, int> RecursiveOccurrenceCounts { get; }
    }
}
