using System;
using System.Collections.Generic;
using System.Threading;
using Warpstone.ParseState;

namespace Warpstone.Parsers.Internal;

/// <summary>
/// Contains the packrat parsing algorithm.
/// Left recursion support is based on the paper: 'Packrat Parsers Can Support Multiple Left-recursive Calls at the Same Position'.
/// Authors: Masaki Umeda, Atusi Maeda
/// URL: <seealso href="https://doi.org/10.2197/ipsjjip.29.174"/>.
/// </summary>
internal static class Packrat
{
    private static readonly IEnumerable<IParseResult> EmptyResults = Array.Empty<IParseResult>();

    /// <summary>
    /// Performs the "ApplyRule" function as described in the paper.
    /// </summary>
    /// <typeparam name="TOutput">The result type of the provided parser.</typeparam>
    /// <param name="parser">The provided parser.</param>
    /// <param name="state">The current parse state.</param>
    /// <param name="position">The position to apply the parser to.</param>
    /// <param name="maxLength">The maximum number of tokens to consume.</param>
    /// <param name="cancellationToken">The token which can be used for cancelling the parsing.</param>
    /// <returns>The found parsing result.</returns>
    public static IParseResult<TOutput> ApplyRule<TOutput>(IParser<TOutput> parser, IParseState state, int position, int maxLength, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IMemoTable memoTable = state.MemoTable;
        string input = state.Unit.Input;

        if (!memoTable.TryGet(position, parser, out IParseResult<TOutput>? prev))
        {
            memoTable.Set(position, parser, new ParseResult<TOutput>(parser, new UnboundedRecursionError(new SourcePosition(input, position, 0)), position));

            IParseResult<TOutput> ans = parser.Eval(state, position, maxLength, RegularRecursion.Instance, cancellationToken);
            memoTable.Set(position, parser, ans);

            if (memoTable.IsGrowing(position, parser))
            {
                IParseResult<TOutput> grown = GrowLR(parser, position, ans, state, maxLength, cancellationToken);
                memoTable.SetGrowing(position, parser, false);
                return grown;
            }

            return ans;
        }
        else if (!prev.Success && prev.Error is UnboundedRecursionError err)
        {
            memoTable.SetGrowing(position, parser, true);
            return prev;
        }

        return prev;
    }

    private static IParseResult<TOutput> GrowLR<TOutput>(IParser<TOutput> parser, int growthPosition, IParseResult<TOutput> best, IParseState state, int maxLength, CancellationToken cancellationToken)
    {
        int oldPosition = growthPosition;

        while (true)
        {
            HashSet<IParser> limits = new HashSet<IParser> { parser };

            IParseResult<TOutput> ans = EvalGrow(parser, growthPosition, limits, state, growthPosition, maxLength, cancellationToken);

            if (!ans.Success || ans.End <= oldPosition)
            {
                break;
            }

            best = ans;

            state.MemoTable.Set(growthPosition, parser, ans);
            oldPosition = ans.End;
        }

        return best;
    }

    private static IParseResult<TOutput> EvalGrow<TOutput>(IParser<TOutput> parser, int growthPosition, ISet<IParser> limits, IParseState state, int position, int maxLength, CancellationToken cancellationToken)
    {
        IRecursionParser recursion = new GrowthRecursion(growthPosition, limits);
        return parser.Eval(state, position, maxLength, recursion, cancellationToken);
    }

    public static IParseResult<TOutput> ApplyRuleGrow<TOutput>(IParser<TOutput> parser, int growthPosition, ISet<IParser> limits, IParseState state, int position, int maxLength, CancellationToken cancellationToken)
    {
        limits.Add(parser);
        IParseResult<TOutput> ans = EvalGrow(parser, growthPosition, limits, state, position, maxLength, cancellationToken);

        if (state.MemoTable.TryGet(growthPosition, parser, out IParseResult<TOutput>? m) && (!ans.Success || ans.End <= m.End))
        {
            return m;
        }

        state.MemoTable.Set(growthPosition, parser, ans);
        return ans;
    }
}
