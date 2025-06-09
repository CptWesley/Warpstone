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
    public void PerformAnalysisStep(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
    {
        var updatedTrace = new IParser[trace.Count + 1];
        CopyTo(trace, updatedTrace);
        updatedTrace[updatedTrace.Length - 1] = this;

        var oldLength = info.MaximumNestedParserDepth;
        if (updatedTrace.Length > oldLength)
        {
            info.MaximumNestedParserDepth = updatedTrace.Length;
        }

        var oldCount = info.OccurrenceCounts.TryGetValue(this, out var cnt) ? cnt : 0;
        var newCount = oldCount + 1;
        info.OccurrenceCounts[this] = newCount;

        if (trace.Contains(this))
        {
            info.HasRecursiveParsers = true;
            return;
        }

        throw new NotImplementedException();
    }

    private static void CopyTo(IReadOnlyList<IParser> trace, IParser[] updated)
    {
        if (trace is IParser[] arr)
        {
            Array.Copy(arr, updated, arr.Length);
        }
        else if (trace is ICollection<IParser> list)
        {
            list.CopyTo(updated, 0);
        }
        else
        {
            for (var i = 0; i < trace.Count; i++)
            {
                updated[i] = trace[i];
            }
        }
    }

    /// <inheritdoc />
    public IParserImplementation<T> CreateUninitializedImplementation()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    IParserImplementation IParser.CreateUninitializedImplementation()
        => CreateUninitializedImplementation();
}
