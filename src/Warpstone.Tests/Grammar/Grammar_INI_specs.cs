using Microsoft.CodeAnalysis.Text;
using Warpstone.Examples;
using static Warpstone.Examples.IniTokenKind;

namespace Grammar_INI_specs;

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
