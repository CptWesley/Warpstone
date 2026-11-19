namespace Warpstone.Tests.Parsers;

/// <summary>
/// <c>Or</c> of any number of alternatives runs as one flat choice parser.
/// </summary>
public static class OrParser
{
    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public static void Returns_the_first_successful_alternative(ParseOptions options)
    {
        var parser = Or(String("ab"), String("a").Transform(_ => "first a"), String("a"), String("b"));

        parser.Parse("ab", options).Should().Be("ab");
        parser.Parse("a", options).Should().Be("first a");
        parser.Parse("b", options).Should().Be("b");
        parser.TryParse("c", options).Success.Should().BeFalse();
    }

    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public static void Tries_many_alternatives(ParseOptions options)
    {
        var parser = Or(Enumerable.Range(0, 10_000).Select(i => String($"<{i}>")));

        parser.Parse("<9999>", options).Should().Be("<9999>");
        parser.Analyze().MaximumNestedParserDepth.Should().Be(2);
    }
}
