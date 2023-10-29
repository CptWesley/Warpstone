namespace Warpstone.V2;

public sealed class FromMemorySource : IParseInputSource
{
    public static readonly FromMemorySource Instance = new();

    private FromMemorySource()
    {
    }
}
