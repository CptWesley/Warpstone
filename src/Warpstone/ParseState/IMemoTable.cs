namespace Warpstone.ParseState;

/// <summary>
/// Provides a read-only interface for memory tables.
/// </summary>
public interface IMemoTable : IReadOnlyMemoTable
{
    /// <summary>
    /// Inserts the <see cref="IParseResult"/> of the given <paramref name="parser"/> at the given <paramref name="position"/> into the table.
    /// </summary>
    /// <param name="position">The position in the input where the result was obtained.</param>
    /// <param name="parser">The parser which was used to obtain the result.</param>
    /// <param name="result">The found parsing result.</param>
    /// <returns><c>true</c> if the insertion overrode a previous value, <c>false</c> otherwise.</returns>
    public bool Set(int position, IParser parser, IParseResult result);
}
