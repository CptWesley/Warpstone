using Warpstone;
using Tokenized = Warpstone.Tokenizer<Grammar_specs.TokenKind>;
using static Grammar_specs.TokenKind;

namespace Grammar_specs;

public class Parses
{
    [Fact]
    public void key_value_pair_without_comment()
    {
        var source = "Greet = Hello,world!  ".Text();
        var tokenizer = Tokenized.Tokenize(source, IniGrammar.kvp);

        tokenizer.Should().HaveTokenized(
            Token.New(00, "Greet", KeyToken),
            Token.New(05, " ", WhitespaceToken),
            Token.New(06, "=", EqualsToken),
            Token.New(07, " ", WhitespaceToken),
            Token.New(08, "Hello,world!", ValueToken),
            Token.New(20, "  ", WhitespaceToken));
    }

    [Fact]
    public void key_value_pair_with_comment()
    {
        var source = "Greet = Hello,world!  # What a classic. :)".Text();
        var tokenizer = Tokenized.Tokenize(source, IniGrammar.kvp);

        tokenizer.Should().HaveTokenized(
            Token.New(00, "Greet", KeyToken),
            Token.New(05, " ", WhitespaceToken),
            Token.New(06, "=", EqualsToken),
            Token.New(07, " ", WhitespaceToken),
            Token.New(08, "Hello,world!", ValueToken),
            Token.New(20, "  ", WhitespaceToken),
            Token.New(22, "#", CommentDelimiterToken),
            Token.New(23, " What a classic. :)", CommentToken));
    }
}

public class IniGrammar : Grammar<TokenKind>
{
    public static Tokenized kvp(Tokenized t) => t +
         space + key + space + assign + space + value + space + Option(comment) + eol;

    public static Tokenized key(Tokenized t) => t +
        line(@"[^\s:=]+", KeyToken);

    public static Tokenized assign(Tokenized t) => t +
        ch('=', EqualsToken) | ch(':', ColonToken);

    public static Tokenized value(Tokenized t) => t +
        line(@"[^\s#;]+", ValueToken);

    public static Tokenized comment(Tokenized t) => t +
        comment_delimiter + comment_text;

    public static Tokenized comment_delimiter(Tokenized t) => t +
        ch('#', CommentDelimiterToken) | ch(';', CommentDelimiterToken);

    public static Tokenized comment_text(Tokenized t) => t +
        line(".*", CommentToken);

    public static Tokenized space(Tokenized t) => t +
        line(@"\s*", WhitespaceToken);

    public static Tokenized eol(Tokenized t) => t +
        eof | str("\r\n", EoLToken) | ch('\n', EoLToken);
}

public enum TokenKind
{
    ValueToken,
    KeyToken,
    EqualsToken,
    ColonToken,
    WhitespaceToken,
    CommentDelimiterToken,
    CommentToken,
    EoLToken,
}
