namespace Warpstone;

/// <summary>
/// Provides helper methods to create <see cref="IParseContext{T}"/> instances.
/// </summary>
public static class ParseContext
{
    private static readonly MethodInfo createMethodInfo
        = typeof(ParseContext)
            .GetRuntimeMethods()
            .First(m
                => m.Name == nameof(Create)
                && m.IsGenericMethodDefinition
                && m.GetParameters().Length == 2
                && m.GetParameters()[0].ParameterType == typeof(IParseInput));

    /// <summary>
    /// Creates a new <see cref="IParseContext{T}"/> from the given <paramref name="input"/>
    /// and the initial <paramref name="parser"/>.
    /// </summary>
    /// <param name="input">The input to be parsed.</param>
    /// <param name="parser">The parser that starts the parsing.</param>
    /// <typeparam name="T">The output type of the initial parser.</typeparam>
    /// <returns>The newly created <see cref="IParseContext{T}"/> instance.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IParseContext<T> Create<T>(IParseInput input, IParser<T> parser)
        => ParseContext<T>.Create(input, parser);

    /// <summary>
    /// Creates a new <see cref="IParseContext{T}"/> from the given <paramref name="input"/>
    /// and the initial <paramref name="parser"/>.
    /// </summary>
    /// <param name="input">The input to be parsed.</param>
    /// <param name="parser">The parser that starts the parsing.</param>
    /// <typeparam name="T">The output type of the initial parser.</typeparam>
    /// <returns>The newly created <see cref="IParseContext{T}"/> instance.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IParseContext<T> Create<T>(string input, IParser<T> parser)
        => Create(ParseInput.CreateFromMemory(input), parser);

    /// <summary>
    /// Creates a new <see cref="IParseContext{T}"/> from the given <paramref name="input"/>
    /// and the initial <paramref name="parser"/>.
    /// </summary>
    /// <param name="input">The input to be parsed.</param>
    /// <param name="parser">The parser that starts the parsing.</param>
    /// <returns>The newly created <see cref="IParseContext{T}"/> instance.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IParseContext Create(IParseInput input, IParser parser)
    {
        var expectedInterface = typeof(IParser<>).MakeGenericType(parser.ResultType);
        if (!parser.GetType().GetInterfaces().Contains(expectedInterface))
        {
            throw new ArgumentException($"Parser does not implement 'IParser<{parser.ResultType.FullName}>'.", nameof(parser));
        }

        var method = createMethodInfo.MakeGenericMethod(parser.ResultType);
        return (IParseContext)method.Invoke(null, new object[] { input, parser })!;
    }

    /// <summary>
    /// Creates a new <see cref="IParseContext{T}"/> from the given <paramref name="input"/>
    /// and the initial <paramref name="parser"/>.
    /// </summary>
    /// <param name="input">The input to be parsed.</param>
    /// <param name="parser">The parser that starts the parsing.</param>
    /// <returns>The newly created <see cref="IParseContext{T}"/> instance.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IParseContext Create(string input, IParser parser)
        => Create(ParseInput.CreateFromMemory(input), parser);
}

/// <summary>
/// Represents a parse context.
/// </summary>
/// <typeparam name="T">The output type.</typeparam>
public sealed class ParseContext<T> : IParseContext<T>
{
    private readonly IMemoTable memo;
    private readonly IReadOnlyMemoTable readOnlyMemo;
    private readonly FlagTable growing = new();
    private readonly IReadOnlyParseContext<T> readOnlySelf;

    private readonly IterativeExecutor executor;

    private ParseContext(IParseInput input, IParser<T> parser)
    {
        Input = input;
        Parser = parser;
        executor = IterativeExecutor.Create(Iterative.Done(() => ApplyRule(parser, 0)));
        memo = new MemoTable();
        readOnlyMemo = memo.AsReadOnly();
        readOnlySelf = new ReadOnlyParseContext<T>(this);
    }

    /// <inheritdoc />
    public IParseInput Input { get; }

    /// <inheritdoc />
    public IParser<T> Parser { get; }

    /// <inheritdoc />
    IParser IReadOnlyParseContext.Parser => Parser;

    /// <inheritdoc />
    public bool Done => executor.Done;

    /// <inheritdoc />
    public IParseResult<T> Result => RunToEnd(default);

    /// <inheritdoc />
    IParseResult IReadOnlyParseContext.Result => Result;

    /// <inheritdoc />
    public IReadOnlyMemoTable MemoTable => readOnlyMemo;

    /// <inheritdoc />
    public bool Step()
    {
        if (executor.Done)
        {
            return false;
        }

        lock (executor)
        {
            if (executor.Done)
            {
                return false;
            }

            executor.Step();
        }

        return true;
    }

    /// <inheritdoc />
    public IParseResult<T> RunToEnd(CancellationToken cancellationToken)
        => cancellationToken.CanBeCanceled
        ? RunToEndWithCancellation(cancellationToken)
        : RunToEndWithoutCancellation();

    private IParseResult<T> RunToEndWithCancellation(CancellationToken cancellationToken)
    {
        if (executor.Done)
        {
            return (IParseResult<T>)executor.Result!;
        }

        lock (executor)
        {
            while (!executor.Done)
            {
                cancellationToken.ThrowIfCancellationRequested();
                executor.Step();
            }
        }

        return (IParseResult<T>)executor.Result!;
    }

    private IParseResult<T> RunToEndWithoutCancellation()
    {
        if (executor.Done)
        {
            return (IParseResult<T>)executor.Result!;
        }

        lock (executor)
        {
            while (!executor.Done)
            {
                executor.Step();
            }
        }

        return (IParseResult<T>)executor.Result!;
    }

    /// <inheritdoc />
    [MethodImpl(InlinedOptimized)]
    IParseResult IParseContext.RunToEnd(CancellationToken cancellationToken)
        => RunToEnd(cancellationToken);

    private IterativeStep ApplyRule(IParser parser, int p)
    {
        if (memo[p, parser] is not { } m)
        {
            memo[p, parser] = parser.Fail(readOnlySelf, p);
            return Iterative.More(
                () => Eval(parser, p),
                untypedM =>
                {
                    var m = untypedM.AssertOfType<IParseResult>();

                    memo[p, parser] = m;

                    if (!growing[p, parser])
                    {
                        return Iterative.Done(m);
                    }

                    return Iterative.More(
                        () => GrowLR(parser, p),
                        _ =>
                        {
                            growing[p, parser] = false;
                            var m = memo[p, parser]!;
                            return Iterative.Done(m);
                        });
                });
        }

        if (m.Status == ParseStatus.Fail)
        {
            m = parser.Mismatch(readOnlySelf, p, m.Errors);
            memo[p, parser] = m;
            growing[p, parser] = true;
            return Iterative.Done(m);
        }

        return Iterative.Done(m);
    }

    private IterativeStep ApplyRuleGrow(IParser parser, int p, IImmutableSet<IParser> limits)
    {
        limits = limits.Add(parser);

        return Iterative.More(
            () => EvalGrow(parser, p, limits),
            untypedAns =>
            {
                var ans = untypedAns.AssertOfType<IParseResult>();

                if (memo[p, parser] is { } prev && (ans.Status != ParseStatus.Match || ans.NextPosition <= prev.NextPosition))
                {
                    return Iterative.Done(prev);
                }

                memo[p, parser] = ans;
                return Iterative.Done(ans);
            });
    }

    [MethodImpl(InlinedOptimized)]
    private IterativeStep GrowLR(IParser parser, int p)
        => GrowLR(parser, p, -1);

    private IterativeStep GrowLR(IParser parser, int p, int prevPos)
        => Iterative.More(
            () => EvalGrow(parser, p, ImmutableHashSet.Create(parser)),
            untypedAns =>
            {
                var ans = untypedAns.AssertOfType<IParseResult>();

                if (ans.Status != ParseStatus.Match || ans.NextPosition <= prevPos)
                {
                    return Iterative.Done();
                }

                memo[p, parser] = ans;
                return Iterative.Done(() => GrowLR(parser, p, ans.NextPosition));
            });

    [MethodImpl(InlinedOptimized)]
    private IterativeStep Eval(IParser parser, int position)
        => parser.Eval(readOnlySelf, position, ApplyRule);

    [MethodImpl(InlinedOptimized)]
    private IterativeStep EvalGrow(IParser parser, int position, IImmutableSet<IParser> limits)
        => parser.Eval(readOnlySelf, position, (calledParser, calledPosition) =>
        {
            if (calledPosition == position && !limits.Contains(calledParser))
            {
                return ApplyRuleGrow(calledParser, calledPosition, limits);
            }

            return ApplyRule(calledParser, calledPosition);
        });

    /// <summary>
    /// Creates a new <see cref="IParseContext{T}"/> from the given <paramref name="input"/>
    /// and the initial <paramref name="parser"/>.
    /// </summary>
    /// <param name="input">The input to be parsed.</param>
    /// <param name="parser">The parser that starts the parsing.</param>
    /// <returns>The newly created <see cref="IParseContext{T}"/> instance.</returns>
    [MethodImpl(InlinedOptimized)]
    public static IParseContext<T> Create(IParseInput input, IParser<T> parser)
        => new ParseContext<T>(input, parser);
}
