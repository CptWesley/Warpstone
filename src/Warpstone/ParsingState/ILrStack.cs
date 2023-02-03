namespace Warpstone.ParsingState;

/// <summary>
/// Represents an LR instance in the left-recursive packrat algorithm.
/// </summary>
public interface ILrStack : IReadOnlyLrStack
{
    /// <summary>
    /// Gets or sets the parse seed.
    /// </summary>
    public new IParseResult Seed { get; set; }

    /// <summary>
    /// Gets or sets the head instance.
    /// </summary>
    public new IHead? Head { get; set; }

    /// <summary>
    /// Gets or sets the next instance.
    /// </summary>
    public new ILrStack? Next { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not this LR stack is a finished value.
    /// </summary>
    public new bool Finished { get; set; }
}

/// <summary>
/// Represents an LR instance in the left-recursive packrat algorithm.
/// </summary>
/// <typeparam name="TOut">The output type of the parser.</typeparam>
public interface ILrStack<TOut> : ILrStack, IReadOnlyLrStack<TOut>
{
    /// <summary>
    /// Gets or sets the parse seed.
    /// </summary>
    public new IParseResult<TOut> Seed { get; set; }

    /// <summary>
    /// Gets or sets the head instance.
    /// </summary>
    public new IHead<TOut>? Head { get; set; }
}