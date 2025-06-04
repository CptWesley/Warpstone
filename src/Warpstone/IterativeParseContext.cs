using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;

namespace Warpstone;

/// <summary>
/// Parsing context for iterative parsing.
/// </summary>
/// <typeparam name="T">The result type of the parsing.</typeparam>
public sealed class IterativeParseContext<T> : IParseContext<T>, IIterativeParseContext
{
    private readonly Lock lck = new();
    private readonly string inputString;

    private readonly MemoTable memoTable;
    private readonly IReadOnlyMemoTable readOnlyMemoTable;

    private readonly Stack<UnsafeParseResult> resultStack;
    private readonly Stack<(int Position, IParser Parser)> executionStack;

    private IParseResult<T>? result;

    /// <summary>
    /// Initializes a new instance of the <see cref="IterativeParseContext{T}"/> class.
    /// </summary>
    /// <param name="input">The input to parse.</param>
    /// <param name="parser">The parser to run.</param>
    public IterativeParseContext(IParseInput input, IParser<T> parser)
    {
        Parser = parser;
        Input = input;
        inputString = input.Content;

        memoTable = new MemoTable();
        readOnlyMemoTable = memoTable.AsReadOnly();

        resultStack = new();
        executionStack = new();
        executionStack.Push((0, parser));
    }

    /// <inheritdoc />
    public IParser<T> Parser { get; }

    /// <inheritdoc />
    IParser IReadOnlyParseContext.Parser => Parser;

    /// <inheritdoc />
    public IParseResult<T> Result => RunToEnd(default);

    /// <inheritdoc />
    IParseResult IReadOnlyParseContext.Result => Result;

    /// <inheritdoc />
    public IParseInput Input { get; }

    /// <inheritdoc />
    string IIterativeParseContext.Input => inputString;

    /// <inheritdoc />
    public IReadOnlyMemoTable MemoTable => readOnlyMemoTable;

    /// <inheritdoc />
    IMemoTable IIterativeParseContext.MemoTable => memoTable;

    /// <inheritdoc />
    Stack<UnsafeParseResult> IIterativeParseContext.ResultStack => resultStack;

    /// <inheritdoc cref="IIterativeParseContext.ResultStack" />
    public IReadOnlyCollection<UnsafeParseResult> ResultStack => resultStack;

    /// <inheritdoc />
    Stack<(int Position, IParser Parser)> IIterativeParseContext.ExecutionStack => executionStack;

    /// <inheritdoc cref="IIterativeParseContext.ExecutionStack" />
    public IReadOnlyCollection<(int Position, IParser Parser)> ExecutionStack => executionStack;

    /// <inheritdoc />
    public bool Done => result is { };

    /// <inheritdoc />
    public IParseResult<T> RunToEnd(CancellationToken cancellationToken)
        => cancellationToken.CanBeCanceled
        ? RunToEndWithCancellation(cancellationToken)
        : RunToEndWithoutCancellation();

    /// <inheritdoc />
    IParseResult IParseContext.RunToEnd(CancellationToken cancellationToken)
        => RunToEnd(cancellationToken);

    private IParseResult<T> RunToEndWithCancellation(CancellationToken cancellationToken)
    {
        if (result is { })
        {
            return result;
        }

        lock (lck)
        {
            while (executionStack.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                InternalStep();
            }

            result = resultStack.Pop().AsSafe(this);
        }

        return result;
    }

    private IParseResult<T> RunToEndWithoutCancellation()
    {
        if (result is { })
        {
            return result;
        }

        lock (lck)
        {
            while (executionStack.Count > 0)
            {
                InternalStep();
            }

            result = resultStack.Pop().AsSafe(this);
        }

        return result;
    }

    /// <inheritdoc />
    public bool Step()
    {
        if (result is { })
        {
            return false;
        }

        lock (lck)
        {
            if (result is { })
            {
                return false;
            }

            InternalStep();

            if (executionStack.Count <= 0)
            {
                result = resultStack.Pop().AsSafe(this);
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void InternalStep()
    {
        var (pos, cur) = executionStack.Pop();
        cur.Apply(this, pos);
    }
}
