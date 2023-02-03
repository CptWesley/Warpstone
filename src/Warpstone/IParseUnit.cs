using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Warpstone;

/// <summary>
/// Provides an interface for parse units.
/// </summary>
public interface IParseUnit
{
    /// <summary>
    /// Gets the input of this <see cref="IParseUnit"/>.
    /// </summary>
    public string Input { get; }

    /// <summary>
    /// Gets the starting position of this <see cref="IParseUnit"/> in the <see cref="Input"/>.
    /// </summary>
    public int StartingPosition { get; }

    /// <summary>
    /// Gets the maximum search length of this <see cref="IParseUnit"/> in the <see cref="Input"/>.
    /// </summary>
    public int MaxLength { get; }

    /// <summary>
    /// Gets the top-most parser used by this <see cref="IParseUnit"/>.
    /// </summary>
    public IParser Parser { get; }

    /// <summary>
    /// Gets the <see cref="MemoTable"/> used by this <see cref="IParseUnit"/>.
    /// </summary>
    public IReadOnlyMemoTable MemoTable { get; }

    /// <summary>
    /// Gets a value indicating whether or not the parsing unit has finished parsing.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Result))]
    public bool Finished { get; }

    /// <summary>
    /// Gets the result match from this memo table instance.
    /// </summary>
    public IParseResult? Result { get; }

    /// <summary>
    /// Tries to get the result from this <see cref="ParseUnit{TOutput}"/>.
    /// </summary>
    /// <param name="result">The found <see cref="IParseResult{T}"/>.</param>
    /// <returns><c>true</c> if a result was found, <c>false</c> otherwise.</returns>
    public bool TryGetResult([NotNullWhen(true)] out IParseResult? result);

    /// <summary>
    /// Performs the parsing of the input provided to this <see cref="ParseUnit{TOutput}"/> instance synchronously.
    /// </summary>
    /// <param name="cancellationToken">The token used for cancelling the operation.</param>
    /// <returns>The obtained parse result.</returns>
    public IParseResult Parse(CancellationToken cancellationToken);

    /// <summary>
    /// Performs the parsing of the input provided to this <see cref="ParseUnit{TOutput}"/> instance synchronously.
    /// </summary>
    /// <returns>The obtained parse result.</returns>
    public IParseResult Parse();

    /// <summary>
    /// Performs the parsing of the input provided to this <see cref="ParseUnit{TOutput}"/> instance aynschronously.
    /// </summary>
    /// <param name="cancellationToken">The token used for cancelling the operation.</param>
    /// <returns>The task executing the parsing.</returns>
    public Task<IParseResult> ParseAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Performs the parsing of the input provided to this <see cref="ParseUnit{TOutput}"/> instance aynschronously.
    /// </summary>
    /// <returns>The task executing the parsing.</returns>
    public Task<IParseResult> ParseAsync();
}

/// <summary>
/// Provides an interface for typed parse units.
/// </summary>
/// <typeparam name="TOutput">The output type of the parse unit.</typeparam>
public interface IParseUnit<TOutput> : IParseUnit
{
    /// <summary>
    /// Gets the top-most parser used by this <see cref="IParseUnit"/>.
    /// </summary>
    public new IParser<TOutput> Parser { get; }

    /// <summary>
    /// Gets a value indicating whether or not the parsing unit has finished parsing.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Result))]
    public new bool Finished { get; }

    /// <summary>
    /// Gets the result match from this memo table instance.
    /// </summary>
    public new IParseResult<TOutput>? Result { get; }

    /// <summary>
    /// Tries to get the result from this <see cref="ParseUnit{TOutput}"/>.
    /// </summary>
    /// <param name="result">The found <see cref="IParseResult{T}"/>.</param>
    /// <returns><c>true</c> if a result was found, <c>false</c> otherwise.</returns>
    public bool TryGetResult([NotNullWhen(true)] out IParseResult<TOutput>? result);

    /// <summary>
    /// Performs the parsing of the input provided to this <see cref="ParseUnit{TOutput}"/> instance synchronously.
    /// </summary>
    /// <param name="cancellationToken">The token used for cancelling the operation.</param>
    /// <returns>The obtained parse result.</returns>
    public new IParseResult<TOutput> Parse(CancellationToken cancellationToken);

    /// <summary>
    /// Performs the parsing of the input provided to this <see cref="ParseUnit{TOutput}"/> instance synchronously.
    /// </summary>
    /// <returns>The obtained parse result.</returns>
    public new IParseResult<TOutput> Parse();

    /// <summary>
    /// Performs the parsing of the input provided to this <see cref="ParseUnit{TOutput}"/> instance aynschronously.
    /// </summary>
    /// <param name="cancellationToken">The token used for cancelling the operation.</param>
    /// <returns>The task executing the parsing.</returns>
    public new Task<IParseResult<TOutput>> ParseAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Performs the parsing of the input provided to this <see cref="ParseUnit{TOutput}"/> instance aynschronously.
    /// </summary>
    /// <returns>The task executing the parsing.</returns>
    public new Task<IParseResult<TOutput>> ParseAsync();
}