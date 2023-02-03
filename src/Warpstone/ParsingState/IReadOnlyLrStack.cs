namespace Warpstone.ParsingState;

/// <summary>
/// Represents a read-only LR instance in the left-recursive packrat algorithm.
/// </summary>
public interface IReadOnlyLrStack
{
    /// <summary>
    /// Gets the parse seed.
    /// </summary>
    public IParseResult Seed { get; }

    /// <summary>
    /// Gets the parser.
    /// </summary>
    public IParser Parser { get; }

    /// <summary>
    /// Gets the head instance.
    /// </summary>
    public IReadOnlyHead? Head { get; }

    /// <summary>
    /// Gets the next instance.
    /// </summary>
    public IReadOnlyLrStack? Next { get; }

    /// <summary>
    /// Gets a value indicating whether or not this LR stack is a finished value.
    /// </summary>
    public bool Finished { get; }
}

/// <summary>
/// Represents a read-only LR instance in the left-recursive packrat algorithm.
/// </summary>
/// <typeparam name="TOut">The output type of the parser.</typeparam>
public interface IReadOnlyLrStack<out TOut> : IReadOnlyLrStack
{
    /// <summary>
    /// Gets the parse seed.
    /// </summary>
    public new IParseResult<TOut> Seed { get; }

    /// <summary>
    /// Gets the parser.
    /// </summary>
    public new IParser<TOut> Parser { get; }

    /// <summary>
    /// Gets the head instance.
    /// </summary>
    public new IReadOnlyHead<TOut>? Head { get; }
}