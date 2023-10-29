using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Warpstone.Parsers.Internal;
using Warpstone.ParseState;

namespace Warpstone;

/// <summary>
/// Represents a stateful parsing unit.
/// </summary>
/// <typeparam name="TOutput">The output type of the parsing operation.</typeparam>
[SuppressMessage("Design", "CA1001", Justification = "Parse unit instances are not expected to be created in large numbers, we can rely on GC.")]
public sealed class ParseUnit<TOutput> : IParseUnit<TOutput>, IDisposable
{
    private readonly SemaphoreSlim lck = new SemaphoreSlim(1);
    private readonly IParseState state;

    private IParseJob<TOutput>? mainJob = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParseUnit{TOutput}"/> class.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="startingPosition">The starting position inside the input string.</param>
    /// <param name="maxLength">The maximum search length.</param>
    /// <param name="parser">The top-level parser used for parsing.</param>
    public ParseUnit(string input, int startingPosition, int maxLength, IParser<TOutput> parser)
    {
        Input = input;
        StartingPosition = startingPosition;
        MaxLength = maxLength;
        Parser = parser;
        state = new ParseState.ParseState(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParseUnit{TOutput}"/> class.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="parser">The top-level parser used for parsing.</param>
    public ParseUnit(string input, IParser<TOutput> parser)
        : this(input, 0, input.Length, parser)
    {
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="ParseUnit{TOutput}"/> class.
    /// </summary>
    ~ParseUnit()
        => Dispose(false);

    /// <summary>
    /// Gets a value indicating whether or not the resources associated to this parse unit have been freed.
    /// </summary>
    public bool Disposed { get; private set; }

    /// <inheritdoc/>
    public string Input { get; }

    /// <inheritdoc/>
    public int StartingPosition { get; }

    /// <inheritdoc/>
    public int MaxLength { get; }

    /// <inheritdoc/>
    public IParser<TOutput> Parser { get; }

    /// <inheritdoc/>
    IParser IParseUnit.Parser => Parser;

    /// <inheritdoc/>
    public IReadOnlyParseState State => state;

    /// <summary>
    /// Gets a value indicating whether or not the parsing unit has finished parsing.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Result))]
    public bool Finished { get; private set; }

    /// <summary>
    /// Gets a value indicating whether or not the parsing unit has started parsing.
    /// </summary>
    public bool Started => mainJob is not null;

    /// <inheritdoc/>
    public IParseResult<TOutput>? Result { get; private set; }

    /// <inheritdoc/>
    IParseResult? IParseUnit.Result => Result;

    /// <inheritdoc/>
    public bool TryGetResult([NotNullWhen(true)] out IParseResult<TOutput>? result)
    {
        if (Finished && Result is not null)
        {
            result = Result;
            return true;
        }

        result = default;
        return false;
    }

    /// <inheritdoc/>
    public bool TryGetResult([NotNullWhen(true)] out IParseResult? result)
    {
        bool success = TryGetResult(out IParseResult<TOutput>? typedResult);
        result = typedResult;
        return success;
    }

    /// <inheritdoc/>
    [MemberNotNull(nameof(Result))]
    public IParseResult<TOutput> Parse(CancellationToken cancellationToken)
    {
        if (Finished)
        {
            return Result;
        }

        if (Disposed)
        {
            throw new ObjectDisposedException(nameof(ParseUnit<TOutput>));
        }

        lck.Wait();

        try
        {
            if (Finished)
            {
                return Result;
            }

            if (Disposed)
            {
                throw new ObjectDisposedException(nameof(ParseUnit<TOutput>));
            }

            return ParseInternal(cancellationToken);
        }
        finally
        {
            lck.Release();
        }
    }

    /// <inheritdoc/>
    [MemberNotNull(nameof(Result))]
    IParseResult IParseUnit.Parse(CancellationToken cancellationToken)
        => Parse(cancellationToken);

    /// <inheritdoc/>
    [MemberNotNull(nameof(Result))]
    public IParseResult<TOutput> Parse()
        => Parse(CancellationToken.None);

    /// <inheritdoc/>
    [MemberNotNull(nameof(Result))]
    IParseResult IParseUnit.Parse()
        => Parse();

    /// <inheritdoc/>
    public async Task<IParseResult<TOutput>> ParseAsync(CancellationToken cancellationToken)
    {
        if (Finished)
        {
            return Result;
        }

        if (Disposed)
        {
            throw new ObjectDisposedException(nameof(ParseUnit<TOutput>));
        }

        await lck.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            if (Finished)
            {
                return Result;
            }

            if (Disposed)
            {
                throw new ObjectDisposedException(nameof(ParseUnit<TOutput>));
            }

            return ParseInternal(cancellationToken);
        }
        finally
        {
            lck.Release();
        }
    }

    /// <inheritdoc/>
    Task<IParseResult> IParseUnit.ParseAsync(CancellationToken cancellationToken)
        => ParseAsync(cancellationToken).ContinueWith(x => x.Result as IParseResult);

    /// <inheritdoc/>
    public Task<IParseResult<TOutput>> ParseAsync()
        => ParseAsync(CancellationToken.None);

    /// <inheritdoc/>
    Task<IParseResult> IParseUnit.ParseAsync()
        => ParseAsync().ContinueWith(x => x.Result as IParseResult);

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            state.MemoTable.Dispose();
            state.Queue.Dispose();
        }

        lck.Dispose();
        Disposed = true;
    }

    [MemberNotNull(nameof(Result))]
    private IParseResult<TOutput> ParseInternal(CancellationToken cancellationToken)
    {
        Result = Packrat.ApplyRule(Parser, state, StartingPosition, MaxLength, cancellationToken);
        Finished = true;
        return Result;
    }

    private IParseResult<TOutput> QueueParseInternal(CancellationToken cancellationToken)
    {
        if (!Started)
        {
            mainJob = new ParseJob<TOutput>(Parser, StartingPosition, MaxLength);
            state.Queue.Enqueue(mainJob, null);
        }

        while (state.Queue.Any())
        {
            ParseJobInstance ji = state.Queue.Dequeue();
            ji.
        }

        if (mainJob is null || mainJob.Result is null)
        {
            throw new InvalidOperationException("Parse queue is empty, but main job did not finish. This should not be able to happen.");
        }

        return mainJob.Result;
    }
}
