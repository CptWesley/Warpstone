using System.Collections.Generic;

namespace Warpstone.ParsingState;

/// <summary>
/// Represents a read-only head instance in the left-recursive packrat algorithm.
/// </summary>
public interface IReadOnlyHead
{
    /// <summary>
    /// Gets the used parser.
    /// </summary>
    public IParser Parser { get; }

    /// <summary>
    /// Gets the set of involved parsers.
    /// </summary>
    public IReadOnlyCollection<IParser> InvolvedSet { get; }

    /// <summary>
    /// Gets the set of involved parsers that may still be used for growing the parse seed.
    /// </summary>
    public IReadOnlyCollection<IParser> EvalSet { get; }
}

/// <summary>
/// Represents a read-only head instance in the left-recursive packrat algorithm.
/// </summary>
/// <typeparam name="TOut">The output type of the parser.</typeparam>
public interface IReadOnlyHead<out TOut> : IReadOnlyHead
{
    /// <summary>
    /// Gets the used parser.
    /// </summary>
    public new IParser<TOut> Parser { get; }
}