﻿using System.Collections.Immutable;

namespace Warpstone.V2;

public static class ParseContext
{
    public static IParseContext<T> Create<T>(IParseInput input, IParser<T> parser)
        => ParseContext<T>.Create(input, parser);

    public static IParseContext<T> Create<T>(string input, IParser<T> parser)
        => Create(new ParseInput(input), parser);
}

public sealed class ParseContext<T> : IParseContext<T>
{
    private readonly MemoTable memo = new();
    private readonly FlagTable growing = new();

    private readonly IterativeExecutor executor;

    private ParseContext(IParseInput input, IParser<T> parser)
    {
        Input = input;
        Parser = parser;
        executor = IterativeExecutor.Create(ApplyRule(parser, 0));
    }

    public IParseInput Input { get; }

    public IParser<T> Parser { get; }

    public bool Done => executor.Done;

    public IParseResult<T> Result => RunToEnd(default);

    IParser IReadOnlyParseContext.Parser => throw new NotImplementedException();

    public IReadOnlyMemoTable MemoTable => throw new NotImplementedException();

    IParseResult IReadOnlyParseContext.Result => throw new NotImplementedException();

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

    public IParseResult<T> RunToEnd(CancellationToken cancellationToken)
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

    IParseResult IParseContext.RunToEnd(CancellationToken cancellationToken)
        => RunToEnd(cancellationToken);

    private IterativeStep ApplyRule(IParser parser, int p)
    {
        if (memo[p, parser] is not { } m)
        {
            memo[p, parser] = parser.Fail(p, Input);
            return Iterative.More(
                () => Eval(parser, p),
                untypedM =>
                {
                    var m = (IParseResult)untypedM!;

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
            m = parser.Mismatch(p, m.Errors);
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
                var ans = (IParseResult)untypedAns!;

                if (memo[p, parser] is { } prev && (ans.Status != ParseStatus.Match || ans.NextPosition <= prev.NextPosition))
                {
                    return Iterative.Done(prev);
                }

                memo[p, parser] = ans;
                return Iterative.Done(ans);
            });
    }

    private IterativeStep GrowLR(IParser parser, int p)
        => GrowLR(parser, p, -1);

    private IterativeStep GrowLR(IParser parser, int p, int prevPos)
        => Iterative.More(
            () => EvalGrow(parser, p, ImmutableHashSet.Create(parser)),
            untypedAns =>
            {
                var ans = (IParseResult)untypedAns!;
                if (ans.Status != ParseStatus.Match || ans.NextPosition <= prevPos)
                {
                    return Iterative.Done();
                }

                memo[p, parser] = ans;
                return Iterative.Done(() => GrowLR(parser, p, ans.NextPosition));
            });

    private IterativeStep Eval(IParser parser, int position)
        => parser.Eval(Input, position, ApplyRule);

    private IterativeStep EvalGrow(IParser parser, int position, IImmutableSet<IParser> limits)
        => parser.Eval(Input, position, (calledParser, calledPosition) =>
        {
            if (calledPosition == position && !limits.Contains(calledParser))
            {
                return ApplyRuleGrow(calledParser, calledPosition, limits);
            }

            return ApplyRule(calledParser, calledPosition);
        });

    public static IParseContext<T> Create(IParseInput input, IParser<T> parser)
        => new ParseContext<T>(input, parser);
}
