using Warpstone.Internal.ParserImplementations;

namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Represents a parser that always fails.
/// </summary>
/// <typeparam name="T">The result type of the parser.</typeparam>
internal sealed class FailParser<T> : ParserBase<T>
{
    /// <summary>
    /// The singleton instance of the parser.
    /// </summary>
    public static readonly FailParser<T> Instance = new();

    private FailParser()
    {
    }

    /// <inheritdoc />
    public override IParserImplementation<T> CreateUninitializedImplementation()
        => FailParserImpl<T>.Instance;

    /// <inheritdoc />
    protected override void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
    {
        // Do nothing.
    }
}
