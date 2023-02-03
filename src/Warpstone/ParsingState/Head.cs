using System.Collections.Generic;

namespace Warpstone.ParsingState;

/// <summary>
/// Represents a head instance in the left-recursive packrat algorithm.
/// </summary>
/// <typeparam name="TOut">The output type of the parser.</typeparam>
public class Head<TOut> : IHead<TOut>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Head{TOut}"/> class.
    /// </summary>
    /// <param name="parser">The parser of this head instance.</param>
    public Head(IParser<TOut> parser)
        => Parser = parser;

    /// <inheritdoc/>
    public IParser<TOut> Parser { get; }

    /// <inheritdoc/>
    IParser IReadOnlyHead.Parser => Parser;

    /// <inheritdoc/>
    public HashSet<IParser> InvolvedSet { get; } = new HashSet<IParser>();

    /// <inheritdoc/>
    IReadOnlyCollection<IParser> IReadOnlyHead.InvolvedSet => InvolvedSet;

    /// <inheritdoc/>
    public HashSet<IParser> EvalSet { get; } = new HashSet<IParser>();

    /// <inheritdoc/>
    IReadOnlyCollection<IParser> IReadOnlyHead.EvalSet => EvalSet;
}
