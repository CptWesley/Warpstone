using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Warpstone;

/// <summary>
/// Provides a read-only interface for memory tables.
/// </summary>
public interface IReadOnlyMemoTable
{
    /// <summary>
    /// Tries to get a <see cref="IParseResult"/> for a given <paramref name="parser"/> at the given <paramref name="position"/>.
    /// </summary>
    /// <param name="position">The position in the input.</param>
    /// <param name="parser">The parser used at the given position.</param>
    /// <param name="parseResult">The retrieved parse result.</param>
    /// <returns><c>true</c> if a previous result was found, <c>false</c> otherwise.</returns>
    public bool TryGet(int position, IParser parser, [NotNullWhen(true)] out IParseResult? parseResult);

    /// <summary>
    /// Tries to get a <see cref="IParseResult"/> for a given <paramref name="parser"/> at the given <paramref name="position"/>.
    /// </summary>
    /// <param name="position">The position in the input.</param>
    /// <param name="parser">The parser used at the given position.</param>
    /// <param name="parseResult">The retrieved parse result.</param>
    /// <returns><c>true</c> if a previous result was found, <c>false</c> otherwise.</returns>
    /// <typeparam name="TOutput">The type of the parsed output.</typeparam>
    public bool TryGet<TOutput>(int position, IParser<TOutput> parser, [NotNullWhen(true)] out IParseResult<TOutput>? parseResult);

    /// <summary>
    /// Gets all parsers used at a given position.
    /// </summary>
    /// <param name="position">The position to search in.</param>
    /// <returns>A collection of matched parsers.</returns>
    public IReadOnlyDictionary<IParser, IParseResult> GetAtPosition(int position);

    /// <summary>
    /// Gets a list of all positions for which a parse result is matched in the memo table.
    /// </summary>
    /// <returns>A collection of positions.</returns>
    public IReadOnlyList<int> GetPositions();
}
