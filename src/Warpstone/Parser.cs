using System;
using System.Collections.Generic;
using System.Threading;
using Warpstone.ParsingState;

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
    public IParseResult<TOutput> TryMatch(IParseUnit parseUnit, int position, int maxLength, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ApplyRule(parseUnit, position, maxLength, cancellationToken);
    }

    /// <inheritdoc/>
    IParseResult IParser.TryMatch(IParseUnit parseUnit, int position, int maxLength, CancellationToken cancellationToken)
        => TryMatch(parseUnit, position, maxLength, cancellationToken);

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
    /// <param name="parseUnit">The parse unit.</param>
    /// <param name="position">The position to match.</param>
    /// <param name="maxLength">The maximum length of the match.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the parsing task.</param>
    /// <returns>The found parse result.</returns>
    protected abstract IParseResult<TOutput> InternalTryMatch(IParseUnit parseUnit, int position, int maxLength, CancellationToken cancellationToken);

    private IParseResult<TOutput> ApplyRule(IParseUnit parseUnit, int position, int maxLength, CancellationToken cancellationToken)
    {
        // Parse units should always have writeable memo tables inside them.
        // This cast is a hacky way of hiding the writeable part to the exposed API.
        IMemoTable memoTable = (IMemoTable)parseUnit.MemoTable;

        if (memoTable.TryGet(position, this, out ILrStack<TOutput>? prevResult))
        {
            return prevResult.Seed;
        }

        ILrStack<TOutput> lr = LrStack<TOutput>.Create(this, parseUnit.Input, position);
        memoTable.Set(position, lr);

        IParseResult<TOutput> internalResult = InternalTryMatch(parseUnit, position, maxLength, cancellationToken);
        lr.Seed = internalResult;
        return internalResult;
    }
}
