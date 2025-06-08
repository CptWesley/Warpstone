#pragma warning disable QW0011
#pragma warning disable QW0012

namespace Warpstone;

/// <summary>
/// Holds the results of the parser analysis.
/// </summary>
public sealed class ParserAnalysisInfo : IParserAnalysisInfo
{
    private readonly Dictionary<IParser, int> occurrenceCounts = new();
    private readonly Dictionary<IParser, int> leftRecursiveOccurrenceCounts = new();

    /// <inheritdoc />
    public int MaximumNestedParserDepth { get; set; }

    /// <inheritdoc />
    public bool HasRecursiveParsers { get; set; }

    /// <inheritdoc />
    public IDictionary<IParser, int> OccurrenceCounts => occurrenceCounts;

    /// <inheritdoc />
    public IDictionary<IParser, int> LeftRecursiveOccurrenceCounts => leftRecursiveOccurrenceCounts;

    /// <inheritdoc />
    IReadOnlyDictionary<IParser, int> IReadOnlyParserAnalysisInfo.OccurrenceCounts => occurrenceCounts;

    /// <inheritdoc />
    IReadOnlyDictionary<IParser, int> IReadOnlyParserAnalysisInfo.LeftRecursiveOccurrenceCounts => leftRecursiveOccurrenceCounts;
}
