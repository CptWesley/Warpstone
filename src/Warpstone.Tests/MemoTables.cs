namespace Warpstone.Tests;

public static class MemoTables
{
    private static readonly IParserImplementation First = Char('a').GetImplementation(ParseOptions.Default);
    private static readonly IParserImplementation Second = Char('b').GetImplementation(ParseOptions.Default);

    [Test]
    public static void Stores_results_per_position_and_parser()
    {
        var table = new MemoTable();

        // Positions in different chunks, and a second parser added after rows already exist.
        table[0, First] = new(0, 1, 'a');
        table[5_000, First] = new(5_000, 1, 'a');
        table[5_000, Second] = new(5_000, 1, 'b');
        table[5_000, Second] = new(5_000, 2, 'b');

        table.Count.Should().Be(3);
        table[5_000, Second].Length.Should().Be(2);
        table.TryGetValue(0, Second, out _).Should().BeFalse();
        table.TryGetValue(1, First, out _).Should().BeFalse();
        table.TryGetValue(100_000, First, out _).Should().BeFalse();
        table.Keys.Should().BeEquivalentTo([(0, First), (5_000, First), (5_000, Second)]);
    }

    [Test]
    public static void Keeps_failures_apart_from_missing_entries()
    {
        var table = new MemoTable();

        table[0, First] = UnsafeParseResult.Failure(0);

        table.TryGetValue(0, First, out var result).Should().BeTrue();
        result.Success.Should().BeFalse();
    }
}
