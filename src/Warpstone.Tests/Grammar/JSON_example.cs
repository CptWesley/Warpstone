using static Grammar_JSON_example.JsonNodeKind;
using Grammr = Warpstone.Grammar<Grammar_JSON_example.JsonNodeKind>;

namespace Grammar_JSON_example;

public class Parses
{
    [Theory]
    [InlineData("-1")]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("1234")]
    [InlineData("3.1415")]
    [InlineData("-3.1415")]
    [InlineData("3.1415E+10")]
    [InlineData("3.1415E-10")]
    [InlineData("3.1415E10")]
    public void numbers(string json)
        => JsonGrammar.json.Tokenize(Source.Text(json))
        .Should().HaveTokenized(Token.New(0, json, NumberToken));

    [Theory]
    [InlineData("?")]
    [InlineData("Hello, World")]
    public void String(string json)
        => JsonGrammar.json.Tokenize(Source.Text('"' + json + '"'))
        .Should().HaveTokenized(
            Token.New(0, "\"", QuotationToken),
            Token.New(1, json, StringToken),
            Token.New(json.Length + 1, "\"", QuotationToken));

    [Fact]
    public void True() 
        => JsonGrammar.json.Tokenize(Source.Text("true"))
        .Should().HaveTokenized(Token.New(0, "true", TrueToken));

    [Fact]
    public void False()
        => JsonGrammar.json.Tokenize(Source.Text("false"))
        .Should().HaveTokenized(Token.New(0, "false", FalseToken));

    [Fact]
    public void Null()
       => JsonGrammar.json.Tokenize(Source.Text("null"))
       .Should().HaveTokenized(Token.New(0, "null", NullToken));

    [Fact]
    public void Member()
        => JsonGrammar.member.Tokenize(Source.Text("\"No\":false"))
        .Should().HaveTokenized(
            Token.New(0, "\"", QuotationToken),
            Token.New(1, "No", StringToken),
            Token.New(3, "\"", QuotationToken),
            Token.New(4, ":", ColonToken),
            Token.New(5, "false", FalseToken));

    [Fact]
    public void Object()
        => JsonGrammar.json.Tokenize(Source.Text("{\"No\":false,\"Yes\":1}"))
        .Should().HaveTokenized(
            Token.New(00, "{", ObjectStartToken),
            Token.New(01, "\"", QuotationToken),
            Token.New(02, "No", StringToken),
            Token.New(04, "\"", QuotationToken),
            Token.New(05, ":", ColonToken),
            Token.New(06, "false", FalseToken),
            Token.New(11, ",", CommaToken),
            Token.New(12, "\"", QuotationToken),
            Token.New(13, "Yes", StringToken),
            Token.New(16, "\"", QuotationToken),
            Token.New(17, ":", ColonToken),
            Token.New(18, "1", NumberToken),
            Token.New(19, "}", ObjectEndToken));

    [Fact]
    public void Array()
        => JsonGrammar.array.Tokenize(Source.Text("[false,true]"))
        .Should().HaveTokenized(
            Token.New(0, "[", ArrayStartToken),
            Token.New(1, "false", FalseToken),
            Token.New(6, ",", CommaToken),
            Token.New(7, "true", TrueToken),
            Token.New(11, "]", ArrayEndToken));

    [Fact]
    public void JSON_example()
    {
        var source = Source.Text(@"{""widget"": {
    ""debug"": ""on"",
    ""window"": {
        ""title"": ""Sample Konfabulator Widget"",
        ""name"": ""main_window"",
        ""width"": 500,
        ""height"": 500
    },
    ""image"": { 
        ""src"": ""Images/Sun.png"",
        ""name"": ""sun1"",
        ""hOffset"": 250,
        ""vOffset"": 250,
        ""alignment"": ""center""
    },
    ""text"": {
        ""data"": ""Click Here"",
        ""size"": 36,
        ""style"": ""bold"",
        ""name"": ""text1"",
        ""hOffset"": 250,
        ""vOffset"": 100,
        ""alignment"": ""center"",
        ""onMouseUp"": ""sun1.opacity = (sun1.opacity / 100) * 90;""
    }
}}    "
        );

        var tokenizer = JsonGrammar.json.Tokenize(source);
        tokenizer.Should().HaveTokenized(null);
    }
}

public sealed class JsonGrammar : Grammr
{
    public static readonly Grammr ws = regex(@"[ \s\t\r\n]*", WhiteSpaceToken);

    public static readonly Grammr sign = (ch('-', MinusToken) | ch('+', PlusToken)).Option;

    public static readonly Grammr fraction = ch('.', DecimalSeparatorToken).Option;

    public static readonly Grammr number = regex(@"-?(0|[1-9][0-9]*)(\.[0-9]+)?([eE][+-]?(0|[1-9][0-9]*))?", NumberToken);

    public static readonly Grammr @string = ch('"', QuotationToken) & match(c => c != '"', StringToken) & ch('"', QuotationToken);

    public static readonly Grammr member = ws & @string & ws & ch(':', ColonToken) & Lazy(() => element);

    public static readonly Grammr members = member & (ch(',', CommaToken) & member).Star;

    public static readonly Grammr @object =
       ch('{', ObjectStartToken) & (members | ws) & ch('}', ObjectEndToken);

    public static readonly Grammr array =
       ch('[', ArrayStartToken) & (Lazy(() => elements) | ws) & ch(']', ArrayEndToken);

    public static readonly Grammr value =
        @object
        | array
        | @string
        | number
        | str("true", TrueToken)
        | str("false", FalseToken)
        | str("null", NullToken);


    public static readonly Grammr element = ws & value & ws;

    public static readonly Grammr elements = element & (ch(',', CommaToken) & element).Star;

    public static readonly Grammr json = element;
}

public enum JsonNodeKind
{
    None,
    WhiteSpaceToken,
    StringToken,
    TrueToken,
    FalseToken,
    NullToken,
    DigitToken,
    NumberToken,
    CommaToken = ',',
    ColonToken = ':',
    ArrayStartToken = '[',
    ArrayEndToken = ']',
    ObjectStartToken = '{',
    ObjectEndToken = '}',
    DecimalSeparatorToken = '.',
    PlusToken = '+',
    MinusToken = '-',
    ExponentToken = 'e',
    QuotationToken = '"',
}
