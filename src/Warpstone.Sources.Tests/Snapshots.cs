namespace Warpstone.Sources.Tests;

public static class Snapshots
{
    [Fact]
    public static void Foo()
    {
        42.Should().Be(42);
    }
}
