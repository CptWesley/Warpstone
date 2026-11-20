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

    // A chunk starts as a small hash table and turns into a plain array once it holds many positions.
    [TestCase(1)]
    [TestCase(7)]
    [TestCase(37)]
    public static void Finds_every_position_as_chunks_fill_up(int stride)
    {
        var table = new MemoTable();
        var positions = Enumerable.Range(0, 3_000).Select(i => i * stride).ToArray();

        foreach (var position in positions.Reverse())
        {
            table.TryGetValue(position, First, out _).Should().BeFalse();
            table[position, First] = new(position, 1, 'a');
        }

        table.Count.Should().Be(positions.Length);
        foreach (var position in positions)
        {
            table[position, First].Position.Should().Be(position);
            table.TryGetValue(position + 1, Second, out _).Should().BeFalse();
        }

        table.Keys.Select(k => k.Item1).Should().Equal(positions);
    }

    [Test]
    public static void Finds_a_row_created_after_a_miss_at_the_same_position()
    {
        var table = new MemoTable();

        table.TryGetValue(3, First, out _).Should().BeFalse();
        table[3, First] = new(3, 1, 'a');
        table[3, Second] = new(3, 1, 'b');

        table[3, First].Value.Should().Be('a');
        table[3, Second].Value.Should().Be('b');
        table.TryGetValue(4, First, out _).Should().BeFalse();
        table[3, Second].Value.Should().Be('b');
    }
}
