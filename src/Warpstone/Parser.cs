using System;
using System.Collections.Generic;
using System.Threading;
using Warpstone.ParseState;

namespace Warpstone;

/// <summary>
/// Parser class for parsing textual input.
/// </summary>
/// <typeparam name="TOutput">The type of the output.</typeparam>
public abstract class Parser<TOutput> : IParser<TOutput>
{
    /// <summary>
    /// An empty set of results.
    /// </summary>
    protected static readonly IEnumerable<IParseResult> EmptyResults = Array.Empty<IParseResult>();

    /// <inheritdoc/>
    public Type OutputType => typeof(TOutput);

    /// <inheritdoc/>
    public string ToString(int depth)
    {
        if (depth < 0)
        {
            return "...";
        }

        return InternalToString(depth);
    }

    /// <inheritdoc/>
    public sealed override string ToString()
        => ToString(4);

    /// <inheritdoc/>
    public IParseResult<TOutput> TryMatch(IParseState state, int position, int maxLength, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IMemoTable memoTable = state.MemoTable;
        string input = state.Unit.Input;

        if (memoTable.TryGet(position, this, out IParseResult<TOutput>? prevResult))
        {
            return prevResult;
        }

        memoTable.Set(position, this, new ParseResult<TOutput>(this, new UnboundedRecursionError(new SourcePosition(input, position, position)), EmptyResults));

        IParseResult<TOutput> internalResult = InternalTryMatch(state, position, maxLength, cancellationToken);
        memoTable.Set(position, this, internalResult);
        return internalResult;
    }

    /// <inheritdoc/>
    IParseResult IParser.TryMatch(IParseState state, int position, int maxLength, CancellationToken cancellationToken)
        => TryMatch(state, position, maxLength, cancellationToken);

    /// <summary>
    /// Gets the found characters.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="position">The position.</param>
    /// <returns>The found characters.</returns>
    protected string GetFound(string input, int position)
        => position < input?.Length ? $"'{input[position]}'" : "EOF";

    /// <summary>
    /// Provides a stringified version of the parser without depth checks.
    /// </summary>
    /// <param name="depth">The maximum depth to explore.</param>
    /// <returns>The stringified version of the parser.</returns>
    protected abstract string InternalToString(int depth);

    /// <summary>
    /// Attempts to match the current parser at the given <paramref name="position"/> without any checks.
    /// </summary>
    /// <param name="state">The current parse state.</param>
    /// <param name="position">The position to match.</param>
    /// <param name="maxLength">The maximum length of the match.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the parsing task.</param>
    /// <returns>The found parse result.</returns>
    protected abstract IParseResult<TOutput> InternalTryMatch(IParseState state, int position, int maxLength, CancellationToken cancellationToken);
}
