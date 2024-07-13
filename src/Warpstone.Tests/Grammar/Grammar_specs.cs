using Warpstone;
using Tokenized = Warpstone.Tokenizer<Grammar_specs.TokenKind>;
using static Grammar_specs.TokenKind;

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
        var source =  Source.Text(text);
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

file sealed class SimpleGrammar : Grammar<TokenKind>
{
    public static Tokenized a_b_c_OR_ab(Tokenized t) => t & a_b_c | ab;
    
    public static Tokenized a_b_c(Tokenized t) => t & ch('a') & ch('b') & ch('c');
    
    public static Tokenized ab(Tokenized t) => t & str("abc");
}

file sealed class IniGrammar : Grammar<TokenKind>
{
    public static Tokenized line(Tokenized t)
        //{
        //    if (kvp(t) is { State: not Matching.NoMatch } k) return k;
        //    if (comment(t) is { State: not Matching.NoMatch } c) return c;
        //    if (space(t) is { State: not Matching.NoMatch } s) return s;
        //    return t.NoMatch();
        //}

        => t &
        kvp | comment | space;

    public static Tokenized kvp(Tokenized t) => t &
         space & key & space & assign & space & value & space & Option(comment);

    public static Tokenized key(Tokenized t) => t & 
        line(@"[^\s:=]+", KeyToken);

    public static Tokenized assign(Tokenized t) => t &
        ch('=', EqualsToken) | ch(':', ColonToken);

    public static Tokenized value(Tokenized t) => t &
        line(@"[^\s#;]+", ValueToken);

    public static Tokenized comment(Tokenized t) => t &
        space & comment_delimiter &  comment_text;

    public static Tokenized comment_delimiter(Tokenized t) => t &
        ch('#', CommentDelimiterToken) | ch(';', CommentDelimiterToken);

    public static Tokenized comment_text(Tokenized t) => t &
        line(".*", CommentToken);

    public static Tokenized space(Tokenized t) => t &
        line(@"\s*", WhitespaceToken);

    public static Tokenized eol(Tokenized t) => t &
        eof | str("\r\n", EoLToken) | ch('\n', EoLToken);
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
