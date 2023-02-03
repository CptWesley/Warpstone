using System.Collections.Generic;

namespace Warpstone.ParsingState;

/// <summary>
/// Represents a head instance in the left-recursive packrat algorithm.
/// </summary>
public interface IHead : IReadOnlyHead
{
    /// <summary>
    /// Gets the set of involved parsers.
    /// </summary>
    public new HashSet<IParser> InvolvedSet { get; }

    /// <summary>
    /// Gets the set of involved parsers that may still be used for growing the parse seed.
    /// </summary>
    public new HashSet<IParser> EvalSet { get; }
}

/// <summary>
/// Represents a head instance in the left-recursive packrat algorithm.
/// </summary>
/// <typeparam name="TOut">The output type of the parser.</typeparam>
public interface IHead<out TOut> : IHead, IReadOnlyHead<TOut>
{
}