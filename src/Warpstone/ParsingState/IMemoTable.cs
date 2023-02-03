using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Warpstone.ParsingState;

/// <summary>
/// Provides a read-only interface for memory tables.
/// </summary>
public interface IMemoTable : IReadOnlyMemoTable
{
    /// <summary>
    /// Gets or sets the current state of the LR stack.
    /// </summary>
    public new ILrStack? LrStack { get; set; }

    /// <summary>
    /// Gets the current state of the Heads lookup table.
    /// </summary>
    public new IDictionary<int, IHead> Heads { get; }

    /// <summary>
    /// Inserts the <see cref="IParseResult"/> of the given parser at the given <paramref name="position"/> into the table.
    /// </summary>
    /// <param name="position">The position in the input where the result was obtained.</param>
    /// <param name="lr">The LR stack at the given position.</param>
    /// <returns><c>true</c> if the insertion overrode a previous value, <c>false</c> otherwise.</returns>
    public bool Set(int position, ILrStack lr);

    /// <summary>
    /// Tries to get a <see cref="IParseResult"/> for a given <paramref name="parser"/> at the given <paramref name="position"/>.
    /// </summary>
    /// <param name="position">The position in the input.</param>
    /// <param name="parser">The parser used at the given position.</param>
    /// <param name="parseResult">The retrieved parse result.</param>
    /// <returns><c>true</c> if a previous result was found, <c>false</c> otherwise.</returns>
    public bool TryGet(int position, IParser parser, [NotNullWhen(true)] out ILrStack? parseResult);

    /// <summary>
    /// Tries to get a <see cref="IParseResult"/> for a given <paramref name="parser"/> at the given <paramref name="position"/>.
    /// </summary>
    /// <param name="position">The position in the input.</param>
    /// <param name="parser">The parser used at the given position.</param>
    /// <param name="parseResult">The retrieved parse result.</param>
    /// <typeparam name="TOut">The result type of the parser.</typeparam>
    /// <returns><c>true</c> if a previous result was found, <c>false</c> otherwise.</returns>
    public bool TryGet<TOut>(int position, IParser<TOut> parser, [NotNullWhen(true)] out ILrStack<TOut>? parseResult);
}
