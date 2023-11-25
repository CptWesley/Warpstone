namespace Warpstone.Tests.Parsers;

public sealed class SelectParser
{
    private abstract record Base(string Value);
    private sealed record Foo(string Value) : Base(Value);
    private sealed record Bar(string Value) : Base(Value);

    private static readonly IParser<Base> parser
        = Regex(@"f|b")
        .Then<string, Base>(x => x switch
        {
            "f" => Regex(@"\w+").Transform(x => new Foo(x)),
            "b" => Regex(@"\d+").Transform(x => new Bar(x)),
            _ => throw new NotImplementedException(),
        });

    [Theory]
    [InlineData("fHello", typeof(Foo), "Hello")]
    [InlineData("b45", typeof(Bar), "45")]
    public void Parses(string input, Type expectedType, string expectedValue)
    {
        var result = parser.Parse(input);

        result.Should().BeOfType(expectedType);
        result.Value.Should().Be(expectedValue);
    }
}
