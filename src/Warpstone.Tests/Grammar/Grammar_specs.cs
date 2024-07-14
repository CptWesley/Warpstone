using Microsoft.CodeAnalysis.Text;
using static Grammar_specs.TokenKind;
using Grammr = Warpstone.Grammar<Grammar_specs.TokenKind>;

namespace Grammar_specs;

public class Parses
{
    [Fact]
    public void key_value_pair_without_comment()
    {
        var source = "Greet = Hello,world!  ".Text();
        var tokenizer = IniGrammar.single_line.Tokenize(source);

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
        var tokenizer = IniGrammar.single_line.Tokenize(source);

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

    [Fact]
    public void headers()
    {
        var source = Source.Text("[*]");
        var tokenizer = IniGrammar.header.Tokenize(source);

        tokenizer.Should().HaveTokenized(
            Token.New(0, "[", HeaderStartToken),
            Token.New(1, "*", HeaderToken),
            Token.New(2, "]", HeaderEndToken));
    }

    [Fact]
    public void complex_headers()
    {
        var source = Source.Text("[*.{cs,json,cshtml,ts}]");
        var tokenizer = IniGrammar.header.Tokenize(source);

        tokenizer.Should().HaveTokenized(
            Token.New(00, "[", HeaderStartToken),
            Token.New(01, "*.{cs,json,cshtml,ts}", HeaderToken),
            Token.New(22, "]", HeaderEndToken));
    }

    [Fact]
    public void dot_editorconfig()
    {
        using var file = new FileStream("../../../../.editorconfig", FileMode.Open, FileAccess.Read);
        var source = SourceText.From(file);
        var grammar = IniGrammar.file;

        var tokenizer = grammar.Tokenize(source);

        tokenizer.State.Should().Be(Warpstone.Matching.EoF);
        tokenizer.Tokens.Should().NotBeEmpty();
    }
}

file sealed class IniGrammar : Grammr
{
    public static readonly Grammr eol = eof | str("\r\n", EoLToken) | ch('\n', EoLToken);

    public static readonly Grammr space = line(@"\s*", WhitespaceToken);

    public static readonly Grammr header = space
       & ch('[', HeaderStartToken)
       & line(@"[^]]+", HeaderToken)
       & ch(']', HeaderEndToken)
       & space
       & eol;

    public static readonly Grammr comment =
        space
        & (ch('#', CommentDelimiterToken) | ch(';', CommentDelimiterToken))
        & line(".*", CommentToken);

    public static readonly Grammr key = line(@"[^\s:=]+", KeyToken);

    public static readonly Grammr assign = ch('=', EqualsToken) | ch(':', ColonToken);

    public static readonly Grammr value = line(@"[^\s#;]+", ValueToken);

    public static readonly Grammr kvp = space
       & key
       & space
       & assign
       & space
       & value
       & space
       & comment.Option;

    public static readonly Grammr single_line = (kvp | comment | space) & eol;

    public static readonly Grammr section = header & single_line.Star;

    public static readonly Grammr file = single_line.Star & section.Star;
}

file enum TokenKind
{
    None = 0,
    ValueToken,
    KeyToken,
    WhitespaceToken,
    CommentDelimiterToken,
    CommentToken,
    EoLToken,
    HeaderToken,
    HeaderStartToken = '[',
    HeaderEndToken = ']',
    EqualsToken = '=',
    ColonToken = ':',
}
