using static Warpstone.Grammar<Grammar_Match_specs.NoKind>;
using static Grammar_Match_specs.NoKind;
using Warpstone;

namespace Grammar_Match_specs;

public class Matches
{
    [Theory]
    [InlineData("a")]
    [InlineData("b")]
    [InlineData("c")]
    public void Or(string s)
    {
        var grammar = ch('a') | ch('b') | ch('c');
        grammar.Tokenize(Source.Text(s))
            .Should()
            .HaveTokenized(Token.New(0, s, None));
    }

    [Fact]
    public void Not()
    {
        var grammar = ~ch('a') & str("bb");
        grammar.Tokenize(Source.Text("bb"))
            .Should()
            .HaveTokenized(Token.New(0, "bb", None));
    }

    [Fact]
    public void Option_none()
    {
        var grammar = ch('a').Option & str("bc");
        grammar.Tokenize(Source.Text("bc"))
            .Should()
            .HaveTokenized(
                Token.New(0, "bc", None));
    }

    [Fact]
    public void Option_once()
    {
        var grammar = ch('a').Option & str("bc");
        grammar.Tokenize(Source.Text("abc"))
            .Should()
            .HaveTokenized(
                Token.New(0, "a", None),
                Token.New(1, "bc", None));
    }

    [Fact]
    public void Predicate()
    {
        var grammar = match(char.IsDigit) & str("bc");
        grammar.Tokenize(Source.Text("0123456789bc"))
            .Should()
            .HaveTokenized(
                Token.New(0, "0123456789", None),
                Token.New(10, "bc", None));
    }

    [Fact]
    public void Repeat()
    {
        var grammar = ch('a').Plus & str("bc");
        grammar.Tokenize(Source.Text("aaaabc"))
            .Should()
            .HaveTokenized(
                Token.New(0, "a", None),
                Token.New(1, "a", None),
                Token.New(2, "a", None),
                Token.New(3, "a", None),
                Token.New(4, "bc", None));
    }

    [Fact]
    public void Repeat_till_end()
    {
        var grammar = ch('a').Star;
        grammar.Tokenize(Source.Text("aaaa"))
            .Should()
            .HaveTokenized(
                Token.New(0, "a", None),
                Token.New(1, "a", None),
                Token.New(2, "a", None),
                Token.New(3, "a", None));
    }

    [Fact]
    public void Start_with_char()
    {
        var grammar = ch('a');
        grammar.Tokenize(Source.Text("a"))
            .Should()
            .HaveTokenized(Token.New(0, "a", None));
    }

    [Fact]
    public void Start_with_string()
    {
        var grammar = str("abc");
        grammar.Tokenize(Source.Text("abc"))
            .Should()
            .HaveTokenized(Token.New(0, "abc", None));
    }

    [Fact]
    public void End_of_File()
    {
        var grammar = ch('a') & eof;
        grammar.Tokenize(Source.Text("a"))
            .Should()
            .HaveTokenized(Token.New(0, "a", None));
    }
}

public class Does_not_match
{
    [Fact]
    public void Or()
    {
        var grammar = ch('a') | ch('b') | ch('c');
        grammar.Tokenize(Source.Text("d"))
            .Should().NotHaveTokenized();
    }

    [Fact]
    public void Not()
    {
        var grammar = ~ch('a') & str("aa");
        grammar.Tokenize(Source.Text("aa"))
            .Should()
            .NotHaveTokenized();
    }

    [Fact]
    public void Predicate()
    {
        var grammar = match(char.IsDigit);
        grammar.Tokenize(Source.Text("a12"))
            .Should()
            .NotHaveTokenized();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(6)]
    public void Repeat(int count)
    {
        var grammar = ch('a').Repeat(2, 4);
        grammar.Tokenize(Source.Text(new string('a', count)))
            .Should().NotHaveTokenized();
    }

    [Fact]
    public void Start_with_char()
    {
        var grammar = ch('a');
        grammar.Tokenize(Source.Text("b"))
            .Should()
            .NotHaveTokenized();
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    [InlineData("abd")]
    public void Start_with_string(string s)
    {
        var grammar = str("abc");
        grammar.Tokenize(Source.Text(s))
            .Should()
            .NotHaveTokenized();
    }

    [Fact]
    public void End_of_File()
    {
        var grammar = ch('a') & eof & ch('a');
        grammar.Tokenize(Source.Text("aa"))
            .Should()
            .NotHaveTokenized();
    }
}

file enum NoKind { None = 0 }
