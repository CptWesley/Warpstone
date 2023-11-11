namespace Warpstone;

/// <summary>
/// Represents a tuple indicating a line number and a column.
/// </summary>
/// <param name="Index">The original index in the file.</param>
/// <param name="Line">The line number in a file.</param>
/// <param name="Column">The column position in given line.</param>
public readonly record struct LineColumn(int Index, int Line, int Column)
{
    /// <summary>
    /// Indicates whether or not the input is valid.
    /// </summary>
    public bool IsValid => Line > 0;

    /// <inheritdoc />
    public override string ToString()
        => $"Index {Index} Line {Line} Column {Column}";
}
