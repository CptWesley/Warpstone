using Legacy.Warpstone2.Parsers;

namespace Legacy.Warpstone2;

/// <summary>
/// Provides write access to the memo table.
/// </summary>
public interface IMemoTable : IReadOnlyMemoTable
{
    /// <summary>
    /// Gets the <see cref="IParseResult"/> stored at the given <paramref name="position"/>
    /// for the given <paramref name="parser"/>.
    /// </summary>
    /// <param name="position">The position in the input.</param>
    /// <param name="parser">The parser used.</param>
    /// <returns>The stored result or <c>null</c> if not present.</returns>
    public new IParseResult? this[int position, IParser parser] { get; set; }
}
