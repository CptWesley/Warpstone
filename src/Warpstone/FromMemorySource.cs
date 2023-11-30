namespace Warpstone;

/// <summary>
/// Represents an in-memory source.
/// </summary>
public sealed class FromMemorySource : IParseInputSource
{
    /// <summary>
    /// Gets the singleton instance of <see cref="FromMemorySource"/>.
    /// </summary>
    public static readonly FromMemorySource Instance = new();

    private FromMemorySource()
    {
    }

    /// <inheritdoc />
    public override string ToString()
        => "in-memory";
}
