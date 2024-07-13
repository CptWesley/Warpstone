using static Grammar_specs.TokenKind;
using Grammr = Warpstone.Grammar<Grammar_specs.TokenKind>;
using Tokenized = Warpstone.Tokenizer<Grammar_specs.TokenKind>;

namespace Grammar_specs;

public class Parses
{
    [Fact]
    public void second_part_of_or()
    {
        var source = Source.Text("ab");
        var tokenizer = Tokenized.Tokenize(source, SimpleGrammar.a_b_c_OR_ab);
        tokenizer.Should().HaveTokenized(Token.New(0, "ab", None));
    }

    [Theory]
    [InlineData("   ")]
    [InlineData(" \t  ")]
    [InlineData(" ")]
    [InlineData("\r")]
    public void whitespace_only(string text)
    {
        var source = Source.Text(text);
        var tokenizer = Tokenized.Tokenize(source, IniGrammar.line);
        tokenizer.Should().HaveTokenized(Token.New(00, text, WhitespaceToken));
    }

    [Fact]
    public void key_value_pair_without_comment()
    {
        var source = "Greet = Hello,world!  ".Text();
        var tokenizer = Tokenized.Tokenize(source, IniGrammar.line);

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
        var tokenizer = Tokenized.Tokenize(source, IniGrammar.line);

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

file sealed class SimpleGrammar : Grammr
{
    public static Grammr a_b_c_OR_ab => a_b_c | ab;

    public static Grammr a_b_c => ch('a') & ch('b') & ch('c');

    public static Grammr ab => str("ab");
}

file sealed class IniGrammar : Grammr
{
    public static Grammr line => kvp | comment | space;

    public static Grammr kvp =>
         space & key & space & assign & space & value & space & comment.Option;

    public static Grammr key => line(@"[^\s:=]+", KeyToken);

    public static Grammr assign => ch('=', EqualsToken) | ch(':', ColonToken);

    public static Grammr value => line(@"[^\s#;]+", ValueToken);

    public static Grammr comment => space & comment_delimiter & comment_text;

    public static Grammr comment_delimiter => ch('#', CommentDelimiterToken) | ch(';', CommentDelimiterToken);

    public static Grammr comment_text => line(".*", CommentToken);

    public static Grammr space => line(@"\s*", WhitespaceToken);

    public static Grammr eol => eof | str("\r\n", EoLToken) | ch('\n', EoLToken);
}

public enum TokenKind
{
    None = 0,
    ValueToken,
    KeyToken,
    EqualsToken,
    ColonToken,
    WhitespaceToken,
    CommentDelimiterToken,
    CommentToken,
    EoLToken,
}
