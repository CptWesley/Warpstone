namespace Warpstone;

/// <summary>
/// Represents a source of an input file from disk.
/// </summary>
public sealed class FileSource : IParseInputSource
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileSource"/> class.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    public FileSource(string path)
        => Path = path;

    /// <summary>
    /// The path of the file.
    /// </summary>
    public string Path { get; }

    /// <inheritdoc />
    public override string ToString()
        => Path;
}
