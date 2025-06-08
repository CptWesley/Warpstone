namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Provides a base implementation for parsers.
/// </summary>
/// <typeparam name="T">The result type of the parser.</typeparam>
internal abstract class ParserBase<T> : IParser<T>
{
    private readonly Lazy<IReadOnlyParserAnalysisInfo> lazyAnalsysis;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParserBase{T}"/> class.
    /// </summary>
    protected ParserBase()
    {
        lazyAnalsysis = new Lazy<IReadOnlyParserAnalysisInfo>(() =>
        {
            var info = new ParserAnalysisInfo();
            var trace = Array.Empty<IParser>();
            PerformAnalysisStep(info, trace);
            return info;
        });
    }

    /// <inheritdoc />
    public Type ResultType => typeof(T);

    /// <inheritdoc />
    public IParserImplementation<T> GetImplementation(ParseOptions options)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    IParserImplementation IParser.GetImplementation(ParseOptions options)
        => GetImplementation(options);

    /// <inheritdoc />
    public IReadOnlyParserAnalysisInfo Analyze()
        => lazyAnalsysis.Value;

    /// <inheritdoc />
    public abstract void PerformAnalysisStep(IParserAnalysisInfo info, IReadOnlyList<IParser> trace);
}
