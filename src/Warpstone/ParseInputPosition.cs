namespace Warpstone;

/// <summary>
/// Represents a position in an <see cref="IParseInput"/>.
/// </summary>
/// <param name="Input">The input.</param>
/// <param name="Index">The original index in the file.</param>
/// <param name="Line">The line number in a file.</param>
/// <param name="Column">The column position in given line.</param>
public readonly record struct ParseInputPosition(IParseInput Input, int Index, int Line, int Column)
{
    /// <summary>
    /// Indicates whether or not the input is valid.
    /// </summary>
    public bool IsValid => Line > 0;

    /// <inheritdoc />
    public override string ToString()
        => Input.Source is FromMemorySource
        ? $"Index {Index} Line {Line} Column {Column}"
        : $"Index {Index} Line {Line} Column {Column} in {Input.Source}";
}
