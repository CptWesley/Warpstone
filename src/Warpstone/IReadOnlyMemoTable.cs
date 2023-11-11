﻿namespace Warpstone;

/// <summary>
/// Provides read-only access to the memo table.
/// </summary>
public interface IReadOnlyMemoTable : IReadOnlyDictionary<(int, IParser), IParseResult?>
{
    /// <summary>
    /// Gets the <see cref="IParseResult"/> stored at the given <paramref name="position"/>
    /// for the given <paramref name="parser"/>.
    /// </summary>
    /// <param name="position">The position in the input.</param>
    /// <param name="parser">The parser used.</param>
    /// <returns>The stored result or <c>null</c> if not present.</returns>
    public IParseResult? this[int position, IParser parser] { get; }
}
