namespace Warpstone;

public sealed class FromMemorySource : IParseInputSource
{
    public static readonly FromMemorySource Instance = new();

    private FromMemorySource()
    {
    }
}
