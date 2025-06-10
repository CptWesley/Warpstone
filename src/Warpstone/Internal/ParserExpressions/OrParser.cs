using Warpstone.Internal.ParserImplementations;

namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Represents a parser that parses either the provided first or second option.
/// </summary>
/// <typeparam name="T">The result type of the parsers.</typeparam>
internal sealed class OrParser<T> : ParserBase<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrParser{T}"/> class.
    /// </summary>
    /// <param name="first">The first parser to try.</param>
    /// <param name="second">The second parser to try.</param>
    public OrParser(IParser<T> first, IParser<T> second)
    {
        First = first;
        Second = second;
    }

    /// <summary>
    /// The first parser to try.
    /// </summary>
    public IParser<T> First { get; }

    /// <summary>
    /// The second parser to try.
    /// </summary>
    public IParser<T> Second { get; }

    /// <inheritdoc />
    public override IParserImplementation<T> CreateUninitializedImplementation()
        => new OrParserImpl<T>();

    /// <inheritdoc />
    protected override void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
    {
        First.PerformAnalysisStep(info, trace);
        Second.PerformAnalysisStep(info, trace);
    }
}
