namespace Warpstone.Tests.Parsers;

/// <summary>
/// <c>Then</c>, <c>ThenSkip</c>, <c>ThenAdd</c> and a <c>Transform</c> of their result run as one fused sequence parser.
/// </summary>
public static class SequenceParser
{
    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public static void Transforms_a_sequence_of_any_length(ParseOptions options)
    {
        var parser = Char('a').ThenAdd(Char('b')).ThenAdd(Char('c')).ThenSkip(Char(';')).Transform((a, b, c) => $"{c}{b}{a}");

        parser.Parse("abc;", options).Should().Be("cba");
    }

    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public static void Keeps_value_types(ParseOptions options)
    {
        var parser = Char('1').Transform(_ => 1).ThenAdd(Char('2').Transform(_ => 2)).Transform((a, b) => a + b);

        parser.Parse("12", options).Should().Be(3);
    }

    [TestCaseSource(typeof(TestOptions), nameof(TestOptions.All))]
    public static void Reports_exceptions_thrown_by_the_transformation(ParseOptions options)
    {
        var parser = Char('a').ThenAdd(Char('b')).Transform<char, char, string>((_, _) => throw new FormatException("boom"));

        var result = parser.TryParse("ab", options);

        result.Success.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().BeOfType<TransformationError>()
            .Which.InnerException.Should().BeOfType<FormatException>();
    }
}
