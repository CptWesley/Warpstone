using System.Collections.Concurrent;

namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Provides a base implementation for parsers.
/// </summary>
/// <typeparam name="T">The result type of the parser.</typeparam>
internal abstract class ParserBase<T> : IParser<T>
{
    private readonly Lazy<IReadOnlyParserAnalysisInfo> lazyAnalsysis;
    private readonly ConcurrentDictionary<ParseOptions, ParseOptions> simplifiedOptions = new();
    private readonly ConcurrentDictionary<ParseOptions, IParserImplementation<T>> implementations = new();

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
        var simplified = GetSimplifiedOptions(options);
        return implementations.GetOrAdd(simplified, o =>
        {
            var analysis = Analyze();
            var lookup = new Dictionary<IParser, IParserImplementation>();
            var lazy = new List<ILazyParser>();
            var initializeable = new List<(IParser Parser, IParserImplementation Implementation)>();

            foreach (var entry in analysis.OccurrenceCounts)
            {
                var parser = entry.Key;

                if (parser is ILazyParser lp)
                {
                    lazy.Add(lp);
                    continue;
                }

                var impl = parser.CreateUninitializedImplementation();
                initializeable.Add((parser, impl));
                lookup[parser] = impl;
            }

            foreach (var parser in lazy)
            {
                IParser target = parser;
                var seen = new HashSet<IParser>();

                while (target is ILazyParser lazyTarget)
                {
                    if (!seen.Add(lazyTarget))
                    {
                        throw new ArgumentException("Provided parser contains unsolvable recursive lazy parsers.");
                    }

                    target = lazyTarget.Parser.Value;
                }

                lookup[parser] = lookup[target];
            }

            foreach (var entry in initializeable)
            {
                entry.Implementation.Initialize(entry.Parser, lookup);
            }

            var untypedResult = lookup[this];
            var typedResult = (IParserImplementation<T>)untypedResult;
            return typedResult;
        });
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

        var oldCount = info.OccurrenceCounts.TryGetValue(this, out var oldCountValue) ? oldCountValue : 0;
        var newCount = oldCount + 1;
        info.OccurrenceCounts[this] = newCount;

        if (trace.Contains(this))
        {
            var oldRecCount = info.OccurrenceCounts.TryGetValue(this, out var oldRecCountValue) ? oldRecCountValue : 0;
            var newRecCount = oldRecCount + 1;
            info.RecursiveOccurrenceCounts[this] = newRecCount;

            info.HasRecursiveParsers = true;
            return;
        }

        PerformAnalysisStepInternal(info, updatedTrace);
    }

    /// <inheritdoc cref="PerformAnalysisStep(IParserAnalysisInfo, IReadOnlyList{IParser})" />
    protected abstract void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace);

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
    public abstract IParserImplementation<T> CreateUninitializedImplementation();

    /// <inheritdoc />
    IParserImplementation IParser.CreateUninitializedImplementation()
        => CreateUninitializedImplementation();

    private ParseOptions GetSimplifiedOptions(ParseOptions options)
        => simplifiedOptions.GetOrAdd(options, o =>
        {
            var analysis = Analyze();

            if (o.EnableAutomaticGrowingRecursion && !analysis.HasRecursiveParsers)
            {
                o = o with
                {
                    EnableAutomaticGrowingRecursion = false,
                };
            }

            if (o.EnableAutomaticMemoization && !analysis.OccurrenceCounts.Any(static entry => entry.Value > 1))
            {
                o = o with
                {
                    EnableAutomaticMemoization = false,
                };
            }

            return o;
        });
}
