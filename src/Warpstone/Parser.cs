using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

    // Algorithm is based on "Packrat parsers can support left recursion" by A. Warth, J.R. Douglass and T. Millstein.
    // URL: https://www.doi.org/10.1145/1328408.1328424
    private IParseResult<TOutput> ApplyRule(IParseUnit parseUnit, int position, int maxLength, CancellationToken cancellationToken)
    {
        // Parse units should always have writeable memo tables inside them.
        // This cast is a hacky way of hiding the writeable part to the exposed API.
        IMemoTable memoTable = (IMemoTable)parseUnit.MemoTable;

        if (TryRecall(parseUnit, position, maxLength, cancellationToken, out ILrStack<TOutput>? m))
        {
            // Pos <- m.pos
            if (!m.Finished)
            {
                SetupLr(m, memoTable);
            }

            return m.Seed;
        }

        ILrStack<TOutput> lr = LrStack<TOutput>.Create(this, parseUnit.Input, position);
        lr.Next = memoTable.LrStack;
        memoTable.LrStack = lr;
        memoTable.Set(position, lr);

        IParseResult<TOutput> ans = InternalTryMatch(parseUnit, position, maxLength, cancellationToken);
        memoTable.LrStack = memoTable.LrStack.Next;
        lr.Seed = ans;

        // m.pos <- Pos
        if (lr.Head is null)
        {
            lr.Finished = true;
        }

        return AnswerLr(lr, parseUnit, position, maxLength, cancellationToken);
    }

    private bool TryRecall(IParseUnit parseUnit, int position, int maxLength, CancellationToken cancellationToken, [NotNullWhen(true)] out ILrStack<TOutput>? m)
    {
        IMemoTable memoTable = (IMemoTable)parseUnit.MemoTable;
        bool foundM = memoTable.TryGet(position, this, out m);

        // Check if currently growing a seed parse, otherwise return.
        if (!memoTable.Heads.TryGetValue(position, out IHead h))
        {
            return foundM;
        }

        if (!foundM && h.Parser != this && !h.InvolvedSet.Contains(this))
        {
            m = LrStack<TOutput>.Create(this, parseUnit.Input, position);
            return true;
        }

        if (foundM && h.EvalSet.Contains(this))
        {
            h.EvalSet.Remove(this);
            IParseResult<TOutput> ans = InternalTryMatch(parseUnit, position, maxLength, cancellationToken);
            m!.Seed = ans;
            m.Next = null;
            m.Finished = true;
            // m.pos <- Pos
        }

        return foundM;
    }

    private void SetupLr(ILrStack<TOutput> lr, IMemoTable memoTable)
    {
        if (lr.Head is null)
        {
            lr.Head = new Head<TOutput>(this);
        }

        ILrStack? s = memoTable.LrStack;

        while (s is not null && s.Head != lr.Head)
        {
            s.Head = lr.Head;
            lr.Head.InvolvedSet.Add(s.Parser);
            s = s.Next;
        }
    }

    private IParseResult<TOutput> AnswerLr(ILrStack<TOutput> m, IParseUnit parseUnit, int position, int maxLength, CancellationToken cancellationToken)
    {
        IHead h = m.Head!;

        if (h.Parser != this)
        {
            return m.Seed;
        }

        if (!m.Seed.Success)
        {
            return m.Seed;
        }

        return GrowLr(m, parseUnit, position, maxLength, cancellationToken);
    }

    private IParseResult<TOutput> GrowLr(ILrStack<TOutput> m, IParseUnit parseUnit, int position, int maxLength, CancellationToken cancellationToken)
    {
        IMemoTable memoTable = (IMemoTable)parseUnit.MemoTable;
        IHead h = m.Head!;

        memoTable.Heads[position] = h;

        while (true)
        {
            // Pos <- P
            h.EvalSet.Clear();
            foreach (IParser parser in h.InvolvedSet)
            {
                h.EvalSet.Add(parser);
            }

            IParseResult<TOutput> ans = InternalTryMatch(parseUnit, position, maxLength, cancellationToken);

            if (!ans.Success || ans.End <= m.Seed.End)
            {
                break;
            }

            m.Seed = ans;
            // m.Pos <- Pos
        }

        memoTable.Heads.Remove(position);
        // Pos <- m.pos
        return m.Seed;
    }
}
