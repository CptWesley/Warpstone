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

        if (memoTable.TryGet(position, parser, out IParseResult<TOutput>? prevResult))
        {
            return prevResult;
        }

        memoTable.Set(position, parser, new ParseResult<TOutput>(parser, new UnboundedRecursionError(new SourcePosition(input, position, position)), EmptyResults));

        IParseResult<TOutput> internalResult = parser.Eval(state, position, maxLength, ApplyRuleRecursion.Instance, cancellationToken);
        memoTable.Set(position, parser, internalResult);
        return internalResult;
    }
}
