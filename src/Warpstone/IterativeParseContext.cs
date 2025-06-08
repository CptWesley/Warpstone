namespace Warpstone;

/// <summary>
/// Parsing context for iterative parsing.
/// </summary>
/// <typeparam name="T">The result type of the parsing.</typeparam>
public sealed class IterativeParseContext<T> : IParseContext<T>, IIterativeParseContext
{
    private readonly Lock lck = new();

    private readonly MemoTable memoTable;
    private readonly IReadOnlyMemoTable readOnlyMemoTable;
    private readonly IParserImplementation<T> implementation;

    private readonly Stack<UnsafeParseResult> resultStack;
    private readonly Stack<(int Position, IParserImplementation Parser)> executionStack;

    private IParseResult<T>? result;

    /// <summary>
    /// Initializes a new instance of the <see cref="IterativeParseContext{T}"/> class.
    /// </summary>
    /// <param name="input">The input to parse.</param>
    /// <param name="parser">The parser to run.</param>
    /// <param name="options">The options used for parsing.</param>
    public IterativeParseContext(IParseInput input, IParser<T> parser, ParseOptions options)
    {
        Parser = parser;
        Input = input;
        Options = options;

        implementation = parser.GetImplementation(options);

        memoTable = new MemoTable();
        readOnlyMemoTable = memoTable.AsReadOnly();

        resultStack = new();
        executionStack = new();
        executionStack.Push((0, implementation));
    }

    /// <inheritdoc />
    public IParser<T> Parser { get; }

    /// <inheritdoc />
    public ParseOptions Options { get; }

    /// <inheritdoc />
    IParser IReadOnlyParseContext.Parser => Parser;

    /// <inheritdoc />
    public IParseResult<T> Result => RunToEnd(default);

    /// <inheritdoc />
    IParseResult IReadOnlyParseContext.Result => Result;

    /// <inheritdoc />
    public IParseInput Input { get; }

    /// <inheritdoc />
    public IReadOnlyMemoTable MemoTable => readOnlyMemoTable;

    /// <inheritdoc />
    IMemoTable IParseContext.MemoTable => memoTable;

    /// <inheritdoc />
    Stack<UnsafeParseResult> IIterativeParseContext.ResultStack => resultStack;

    /// <inheritdoc cref="IIterativeParseContext.ResultStack" />
    public IReadOnlyCollection<UnsafeParseResult> ResultStack => resultStack;

    /// <inheritdoc />
    Stack<(int Position, IParserImplementation Parser)> IIterativeParseContext.ExecutionStack => executionStack;

    /// <inheritdoc cref="IIterativeParseContext.ExecutionStack" />
    public IReadOnlyCollection<(int Position, IParserImplementation Parser)> ExecutionStack => executionStack;

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

            result = resultStack.Pop().AsSafe<T>(this);
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

            result = resultStack.Pop().AsSafe<T>(this);
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
                result = resultStack.Pop().AsSafe<T>(this);
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
