using System.Diagnostics.CodeAnalysis;
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
public class ParseUnit<TOutput> : IParseUnit<TOutput>
{
    private readonly SemaphoreSlim lck = new SemaphoreSlim(1);
    private readonly IParseState state;

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
    {
        lck.Dispose();
    }

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
        throw new System.NotImplementedException();
    }

    /// <inheritdoc/>
    [MemberNotNull(nameof(Result))]
    public IParseResult<TOutput> Parse(CancellationToken cancellationToken)
    {
        if (Finished)
        {
            return Result;
        }

        lck.Wait();

        try
        {
            if (Finished)
            {
                return Result;
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

        await lck.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            if (Finished)
            {
                return Result;
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

    [MemberNotNull(nameof(Result))]
    private IParseResult<TOutput> ParseInternal(CancellationToken cancellationToken)
    {
        Result = Packrat.ApplyRule(Parser, state, StartingPosition, MaxLength, cancellationToken);
        Finished = true;
        return Result;
    }
}
