namespace Warpstone.Tests.Expressions;

public static class AnalysisScaling
{
    [Fact]
    public static void Shared_parsers_are_analyzed_once()
    {
        // 64 levels of Or(p, p): a walk that follows every path visits the leaf 2^64 times.
        var leaf = Char('a');
        var parser = leaf;
        for (var i = 0; i < 64; i++)
        {
            parser = Or(parser, parser);
        }

        var analysis = parser.Analyze();

        analysis.MaximumNestedParserDepth.Should().Be(65);
        analysis.OccurrenceCounts[leaf].Should().Be(2);
        analysis.HasRecursiveParsers.Should().BeFalse();
        parser.TryParse("a").Success.Should().BeTrue();
    }

    [Fact]
    public static void Depth_counts_the_longest_path_through_shared_parsers()
    {
        // The shared parser is first reached through the short branch, then again through the long one.
        static IParser<char> Grammar(IParser<char> first, IParser<char> second)
            => Or(first, second.Transform(x => x).Transform(x => x).Transform(x => x));

        var shared = Char('a').ThenSkip(Char('b'));
        var sharedDepth = Grammar(shared, shared).Analyze().MaximumNestedParserDepth;
        var separateDepth = Grammar(Char('a').ThenSkip(Char('b')), Char('a').ThenSkip(Char('b'))).Analyze().MaximumNestedParserDepth;

        sharedDepth.Should().Be(separateDepth);
    }
}
