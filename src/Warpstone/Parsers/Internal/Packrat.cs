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
        Console.WriteLine($"ApplyRule({position}, {parser.ToString(1)})");
        cancellationToken.ThrowIfCancellationRequested();

        IMemoTable memoTable = state.MemoTable;
        string input = state.Unit.Input;

        if (!memoTable.TryGet(position, parser, out IParseResult<TOutput>? prev))
        {
            Console.WriteLine($"ApplyRule({position}, {parser.ToString(1)}) -- Previous result not found");
            memoTable.Set(position, parser, new ParseResult<TOutput>(parser, new UnboundedRecursionError(new SourcePosition(input, position, 0)), position));

            Console.WriteLine($"ApplyRule({position}, {parser.ToString(1)}) -- Evaluating body");
            IParseResult<TOutput> ans = parser.Eval(state, position, maxLength, RegularRecursion.Instance, cancellationToken);
            memoTable.Set(position, parser, ans);

            if (memoTable.IsGrowing(position, parser))
            {
                Console.WriteLine($"ApplyRule({position}, {parser.ToString(1)}) -- Start growing");
                IParseResult<TOutput> grown = GrowLR(parser, position, ans, state, maxLength, cancellationToken);
                memoTable.SetGrowing(position, parser, false);
                return grown;
            }

            Console.WriteLine($"ApplyRule({position}, {parser.ToString(1)}) -- Return result");

            return ans;
        }
        else if (!prev.Success && prev.Error is UnboundedRecursionError err)
        {
            Console.WriteLine($"ApplyRule({position}, {parser.ToString(1)}) -- Recursion detected, enabling growing");
            memoTable.SetGrowing(position, parser, true);
            return prev;
        }

        Console.WriteLine($"ApplyRule({position}, {parser.ToString(1)}) -- Previous result found");

        return prev;
    }

    /// <summary>
    /// Applies a parser in the growing step.
    /// </summary>
    /// <typeparam name="TOutput">The type of the result.</typeparam>
    /// <param name="parser">The parser being applied.</param>
    /// <param name="growthRecursion">The recursive parsing function.</param>
    /// <param name="state">The current parse state.</param>
    /// <param name="position">The current parsing position.</param>
    /// <param name="maxLength">The number of remaining tokens in the input stream to be considered.</param>
    /// <param name="cancellationToken">The token used for cancelling the parsing operation.</param>
    /// <returns>The found result.</returns>
    public static IParseResult<TOutput> ApplyRuleGrow<TOutput>(IParser<TOutput> parser, GrowthRecursion growthRecursion, IParseState state, int position, int maxLength, CancellationToken cancellationToken)
    {
        Console.WriteLine($"ApplyRuleGrow({position}, {parser.ToString(1)})");

        growthRecursion.Limits.Add(parser);
        IParseResult<TOutput> ans = parser.Eval(state, position, maxLength, growthRecursion, cancellationToken);

        if (state.MemoTable.TryGet(growthRecursion.GrowthPosition, parser, out IParseResult<TOutput>? m) && (!ans.Success || ans.End <= m.End))
        {
            Console.WriteLine($"ApplyRuleGrow({position}, {parser.ToString(1)}) -- Previous result is better");
            return m;
        }

        Console.WriteLine($"ApplyRuleGrow({position}, {parser.ToString(1)}) -- Updating result");
        state.MemoTable.Set(growthRecursion.GrowthPosition, parser, ans);
        return ans;
    }

    private static IParseResult<TOutput> GrowLR<TOutput>(IParser<TOutput> parser, int growthPosition, IParseResult<TOutput> best, IParseState state, int maxLength, CancellationToken cancellationToken)
    {
        Console.WriteLine($"GrowLR({growthPosition}, {parser.ToString(1)}) -- START");

        int oldPosition = growthPosition;

        while (true)
        {
            HashSet<IParser> limits = new HashSet<IParser> { parser };

            IRecursionParser recursion = new GrowthRecursion(growthPosition, limits);
            IParseResult<TOutput> ans = parser.Eval(state, growthPosition, maxLength, recursion, cancellationToken);

            if (!ans.Success || ans.End <= oldPosition)
            {
                break;
            }

            best = ans;

            state.MemoTable.Set(growthPosition, parser, ans);
            oldPosition = ans.End;
        }

        Console.WriteLine($"GrowLR({growthPosition}, {parser.ToString(1)}) -- END");
        return best;
    }
}
