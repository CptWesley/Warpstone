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
    /// Gets the head instance.
    /// </summary>
    public new IHead Head { get; }

    /// <summary>
    /// Gets or sets the next instance.
    /// </summary>
    public new ILrStack? Next { get; set; }
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
    /// Gets the head instance.
    /// </summary>
    public new IHead<TOut> Head { get; }

    /// <summary>
    /// Gets or sets the next instance.
    /// </summary>
    public new ILrStack<TOut>? Next { get; set; }
}