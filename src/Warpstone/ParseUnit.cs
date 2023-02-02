using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Warpstone;

/// <summary>
/// Represents a stateful parsing unit.
/// </summary>
/// <typeparam name="TOutput">The output type of the parsing operation.</typeparam>
[SuppressMessage("Design", "CA1001", Justification = "Parse unit instances are not expected to be created in large numbers, we can rely on GC.")]
public class ParseUnit<TOutput>
{
    private readonly SemaphoreSlim lck = new SemaphoreSlim(1);
    private readonly MemoTable memoTable = new MemoTable();

    private IParseResult<TOutput>? result;

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
    {
        lck.Dispose();
    }

    /// <summary>
    /// Gets the input of this <see cref="ParseUnit{TOutput}"/>.
    /// </summary>
    public string Input { get; }

    /// <summary>
    /// Gets the starting position of this <see cref="ParseUnit{TOutput}"/> in the <see cref="Input"/>.
    /// </summary>
    public int StartingPosition { get; }

    /// <summary>
    /// Gets the maximum search length of this <see cref="ParseUnit{TOutput}"/> in the <see cref="Input"/>.
    /// </summary>
    public int MaxLength { get; }

    /// <summary>
    /// Gets the top-most parser used by this <see cref="ParseUnit{TOutput}"/>.
    /// </summary>
    public IParser<TOutput> Parser { get; }

    /// <summary>
    /// Gets the <see cref="MemoTable"/> used by this <see cref="ParseUnit{TOutput}"/>.
    /// </summary>
    public IReadOnlyMemoTable MemoTable => memoTable;

    /// <summary>
    /// Gets a value indicating whether or not the parsing unit has finished parsing.
    /// </summary>
    [MemberNotNullWhen(true, nameof(result))]
    public bool Finished { get; private set; }

    /// <summary>
    /// Gets the result match from this memo table instance.
    /// </summary>
    public IParseResult<TOutput> Result
    {
        get
        {
            Parse();
            return result;
        }
    }

    /// <summary>
    /// Tries to get the result from this <see cref="ParseUnit{TOutput}"/>.
    /// </summary>
    /// <param name="result">The found <see cref="IParseResult{T}"/>.</param>
    /// <returns><c>true</c> if a result was found, <c>false</c> otherwise.</returns>
    public bool TryGetResult([NotNullWhen(true)] out IParseResult<TOutput>? result)
    {
        if (Finished && this.result is not null)
        {
            result = this.result;
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Performs the parsing of the input provided to this <see cref="ParseUnit{TOutput}"/> instance synchronously.
    /// </summary>
    /// <param name="cancellationToken">The token used for cancelling the operation.</param>
    [MemberNotNull(nameof(result))]
    public void Parse(CancellationToken cancellationToken)
    {
        if (Finished)
        {
            return;
        }

        lck.Wait();

        try
        {
            if (Finished)
            {
                return;
            }

            ParseInternal(cancellationToken);
        }
        finally
        {
            lck.Release();
        }
    }

    /// <summary>
    /// Performs the parsing of the input provided to this <see cref="ParseUnit{TOutput}"/> instance synchronously.
    /// </summary>
    [MemberNotNull(nameof(result))]
    public void Parse()
        => Parse(CancellationToken.None);

    /// <summary>
    /// Performs the parsing of the input provided to this <see cref="ParseUnit{TOutput}"/> instance aynschronously.
    /// </summary>
    /// <param name="cancellationToken">The token used for cancelling the operation.</param>
    /// <returns>The task executing the parsing.</returns>
    public async Task ParseAsync(CancellationToken cancellationToken)
    {
        if (Finished)
        {
            return;
        }

        await lck.WaitAsync().ConfigureAwait(false);

        try
        {
            if (Finished)
            {
                return;
            }

            ParseInternal(cancellationToken);
        }
        finally
        {
            lck.Release();
        }
    }

    /// <summary>
    /// Performs the parsing of the input provided to this <see cref="ParseUnit{TOutput}"/> instance aynschronously.
    /// </summary>
    /// <returns>The task executing the parsing.</returns>
    public Task ParseAsync()
        => ParseAsync(CancellationToken.None);

    private void ParseInternal(CancellationToken cancellationToken)
    {
        result = Parser.TryMatch(Input, StartingPosition, MaxLength, memoTable, cancellationToken);
        Finished = true;
    }
}
