#pragma warning disable QW0011
#pragma warning disable QW0012

namespace Warpstone;

/// <summary>
/// Provides a writeable interface of the parser analysis results.
/// </summary>
public interface IParserAnalysisInfo : IReadOnlyParserAnalysisInfo
{
    /// <inheritdoc cref="IReadOnlyParserAnalysisInfo.MaximumNestedParserDepth" />
    public new int MaximumNestedParserDepth { get; set; }

    /// <inheritdoc cref="IReadOnlyParserAnalysisInfo.HasRecursiveParsers" />
    public new bool HasRecursiveParsers { get; set; }

    /// <inheritdoc cref="IReadOnlyParserAnalysisInfo.OccurrenceCounts" />
    public new IDictionary<IParser, int> OccurrenceCounts { get; }

    /// <inheritdoc cref="IReadOnlyParserAnalysisInfo.RecursiveOccurrenceCounts" />
    public new IDictionary<IParser, int> RecursiveOccurrenceCounts { get; }
}
