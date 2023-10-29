namespace Warpstone.V2;

public sealed class FromMemorySource : IParsingInputSource
{
    public static readonly FromMemorySource Instance = new();

    private FromMemorySource()
    {
    }
}
