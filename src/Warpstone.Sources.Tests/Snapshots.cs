namespace Warpstone.Sources.Tests;

public static class Snapshots
{
    [Test]
    public static void Generates()
    {
        GeneratorSnapshot.Verify<Generator>();
    }

    [Test]
    public static void Incremental()
    {
        GeneratorIncrementalCache.Verify<Generator>();
    }
}
