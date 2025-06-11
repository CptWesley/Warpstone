namespace Warpstone.Sources.Tests;

public static class Snapshots
{
    [Fact]
    public static void Generates()
    {
        GeneratorSnapshot.Verify<Generator>();
    }

    [Fact]
    public static void Incremental()
    {
        GeneratorIncrementalCache.Verify<Generator>();
    }
}
