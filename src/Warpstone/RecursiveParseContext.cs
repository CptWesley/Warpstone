using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Warpstone;

/// <summary>
/// Parsing context for recursive parsing.
/// </summary>
/// <typeparam name="T">The result type of the parsing.</typeparam>
public sealed class RecursiveParseContext<T> : IParseContext<T>, IRecursiveParseContext
{
    private readonly object lck = new();
    private readonly string inputString;
    private readonly MemoTable memoTable;
    private readonly IReadOnlyMemoTable readOnlyMemoTable;

    private IParseResult<T>? result;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecursiveParseContext{T}"/> class.
    /// </summary>
    /// <param name="input">The input to parse.</param>
    /// <param name="parser">The parser to run.</param>
    public RecursiveParseContext(IParseInput input, IParser<T> parser)
    {
        Parser = parser;
        Input = input;
        inputString = input.Content;

        memoTable = new MemoTable();
        readOnlyMemoTable = memoTable.AsReadOnly();
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
    string IRecursiveParseContext.Input => inputString;

    /// <inheritdoc />
    public IReadOnlyMemoTable MemoTable => readOnlyMemoTable;

    /// <inheritdoc />
    IMemoTable IRecursiveParseContext.MemoTable => memoTable;

    /// <inheritdoc />
    public bool Done => result is { };

    /// <inheritdoc />
    public IParseResult<T> RunToEnd(CancellationToken cancellationToken)
    {
        Step();
        return result!;
    }

    /// <inheritdoc />
    IParseResult IParseContext.RunToEnd(CancellationToken cancellationToken)
        => RunToEnd(cancellationToken);

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

            var unsafeResult = Parser.Apply(this, 0);
            result = unsafeResult.AsSafe(this);

            return true;
        }
    }
}
