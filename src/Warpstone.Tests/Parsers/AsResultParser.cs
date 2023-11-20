namespace Warpstone.Tests.Parsers;

public sealed class AsResultParser
{
    private sealed record Value(string String, ParseInputPosition Start, ParseInputPosition End);

    private static readonly IParser<Value> parser
        = Regex(@"\s*")
        .Then(Regex(@"[a-zA-Z0-9]+").AsResult())
        .ThenSkip(Regex(@"\s*"))
        .Transform(result => new Value(result.Value, result.InputStartPosition, result.InputEndPosition));

    [Theory]
    [InlineData("simple", 0, 5)]
    [InlineData(" simple", 1, 6)]
    [InlineData("   simple", 3, 8)]
    public void Parses(string input, int startIndex, int endIndex)
    {
        var result = parser.Parse(input);

        result.String.Should().Be(input.Trim());
        result.Start.Index.Should().Be(startIndex);
        result.End.Index.Should().Be(endIndex);
    }
}
