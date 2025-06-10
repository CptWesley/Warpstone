using static Warpstone.Tests.Legacy.Examples.Json.JsonParser;

namespace Warpstone.Tests.Legacy.Examples.Json;

public sealed class Numbers
{
    public static readonly IEnumerable<object[]> Inputs =
    [
        [0],
        [1],
        [2],
        [3],
        [4],
        [5],
        [6],
        [7],
        [8],
        [9],
        [10],
        [42],
        [1337],
    ];

    [Theory]
    [MemberData(nameof(Inputs))]
    public void Without_padding(int v)
    {
        Parse(@$"{v}").Value
            .Should()
            .BeEquivalentTo(new JsonInt(v));
    }

    [Theory]
    [MemberData(nameof(Inputs))]
    public void With_left_padding(int v)
    {
        Parse(@$"  {v}").Value
            .Should()
            .BeEquivalentTo(new JsonInt(v));
    }

    [Theory]
    [MemberData(nameof(Inputs))]
    public void With_right_padding(int v)
    {
        Parse(@$"{v}  ").Value
            .Should()
            .BeEquivalentTo(new JsonInt(v));
    }

    [Theory]
    [MemberData(nameof(Inputs))]
    public void With_padding(int v)
    {
        Parse(@$"  {v}  ").Value
            .Should()
            .BeEquivalentTo(new JsonInt(v));
    }
}

public sealed class Strings
{
    public static readonly IEnumerable<object[]> Inputs =
    [
        [""],
        ["Hello"],
        ["Hello, world!"],
    ];

    [Theory]
    [MemberData(nameof(Inputs))]
    public void Without_padding(string v)
    {
        Parse(@$"""{v}""").Value
            .Should()
            .BeEquivalentTo(new JsonString(v));
    }

    [Theory]
    [MemberData(nameof(Inputs))]
    public void With_left_padding(string v)
    {
        Parse(@$"  ""{v}""").Value
            .Should()
            .BeEquivalentTo(new JsonString(v));
    }

    [Theory]
    [MemberData(nameof(Inputs))]
    public void With_right_padding(string v)
    {
        Parse(@$"""{v}""  ").Value
            .Should()
            .BeEquivalentTo(new JsonString(v));
    }

    [Theory]
    [MemberData(nameof(Inputs))]
    public void With_padding(string v)
    {
        Parse(@$"  ""{v}""  ").Value
            .Should()
            .BeEquivalentTo(new JsonString(v));
    }
}

public sealed class Booleans
{
    public static readonly IEnumerable<object[]> Inputs =
    [
        [true],
        [false],
    ];

    [Theory]
    [MemberData(nameof(Inputs))]
    public void Without_padding(bool v)
    {
        Parse(@$"{v.ToString().ToLowerInvariant()}").Value
            .Should()
            .BeEquivalentTo(new JsonBoolean(v));
    }

    [Theory]
    [MemberData(nameof(Inputs))]
    public void With_left_padding(bool v)
    {
        Parse(@$"  {v.ToString().ToLowerInvariant()}").Value
            .Should()
            .BeEquivalentTo(new JsonBoolean(v));
    }

    [Theory]
    [MemberData(nameof(Inputs))]
    public void With_right_padding(bool v)
    {
        Parse(@$"{v.ToString().ToLowerInvariant()}  ").Value
            .Should()
            .BeEquivalentTo(new JsonBoolean(v));
    }

    [Theory]
    [MemberData(nameof(Inputs))]
    public void With_padding(bool v)
    {
        Parse(@$"  {v.ToString().ToLowerInvariant()}  ").Value
            .Should()
            .BeEquivalentTo(new JsonBoolean(v));
    }
}

public sealed class Complex
{
    [Fact]
    public void Simple_object()
    {
        var result = Parse(@"
        {
            ""elements"": [null, null],
            ""name"": ""NullContainer"",
            ""body"": ""This is the first paragraph\nThis is the second paragraph"",
            ""inner"": {
                ""numVal"": 42,
                ""boolVal"": true
            }
        }");

        result.Value.Should()
            .BeEquivalentTo(new JsonObject(ImmutableArray.Create( 
                new KeyValuePair<JsonString, IJsonValue>(new JsonString("elements"), new JsonArray(ImmutableArray.Create<IJsonValue>(new JsonNull(), new JsonNull()))),
                new KeyValuePair<JsonString, IJsonValue>(new JsonString("name"), new JsonString("NullContainer")),
                new KeyValuePair<JsonString, IJsonValue>(new JsonString("body"), new JsonString("This is the first paragraph\\nThis is the second paragraph")),
                new KeyValuePair<JsonString, IJsonValue>(new JsonString("inner"), new JsonObject(ImmutableArray.Create(
                    new KeyValuePair<JsonString, IJsonValue>(new JsonString("numVal"), new JsonInt(42)),
                    new KeyValuePair<JsonString, IJsonValue>(new JsonString("boolVal"), new JsonBoolean(true))
                )))
            )));
    }
}
