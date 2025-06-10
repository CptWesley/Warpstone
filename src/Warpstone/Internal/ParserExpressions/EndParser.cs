namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Parser used for detecting the end of the input stream.
/// </summary>
internal sealed class EndParser : ParserBase<string>
{
    private static readonly int baseHash = typeof(EndParser).GetHashCode() * 31;

    /// <summary>
    /// Singleton instance of the parser.
    /// </summary>
    public static readonly EndParser Instance = new();

    private EndParser()
    {
    }

    /// <inheritdoc />
    public override IParserImplementation<string> CreateUninitializedImplementation()
        => EndParserImpl.Instance;

    /// <inheritdoc />
    protected override void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
    {
        // Do nothing.
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is EndParser;

    /// <inheritdoc />
    public override int GetHashCode()
        => baseHash;
}
