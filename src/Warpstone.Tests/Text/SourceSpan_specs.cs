using Warpstone.Text;

namespace Text.SourceSpan_specs;

public class IndexOf
{
    [Fact]
    public void null_if_not_found()
    {
        SourceSpan span = Source.From("ABC");
        span.IndexOf('D').Should().BeNull();
    }

    [Fact]
    public void null_if_only_found_before()
    {
        SourceSpan span = Source.From("ABC");
        span++;
        span.IndexOf('A').Should().BeNull();
    }

    [Fact]
    public void null_if_only_found_after()
    {
        SourceSpan span = Source.From("ABC");
        span = span.Trim(new(0, 2));
        span.IndexOf('C').Should().BeNull();
    }

    [Fact]
    public void TextSpan_of_length_1()
    {
        SourceSpan span = Source.From("ABCEFGHIJKLMNOPQR");
        span++;
        var text = span.IndexOf('Q');
        text.Should().Be(new TextSpan(15, 1));
    }
}

public class Match
{
    [Fact]
    public void null_if_not_found()
    {
        SourceSpan span = Source.From("ABC");
        span.Match(@"\d").Should().BeNull();
    }

    [Fact]
    public void null_if_only_found_before()
    {
        SourceSpan span = Source.From("1BC");
        span++;
        span.Match(@"\d").Should().BeNull();
    }

    [Fact]
    public void null_if_only_found_after()
    {
        SourceSpan span = Source.From("AB3");
        span = span.Trim(new(0, 2));
        span.Match(@"\d").Should().BeNull();
    }

    [Fact]
    public void TextSpan_of_length_matches()
    {
        SourceSpan span = Source.From("ABCEFGHI123MNOPQR");
        span++;
        var text = span.Match(@"\d+");
        text.Should().Be(new TextSpan(8, 3));
    }

    [Fact]
    public void TextSpan_of_length_zero_on_successful_empty_match()
    {
        SourceSpan span = Source.From("ABCEFGHI123MNOPQR");
        span++;
        var text = span.Match(@"\s*");
        text.Should().Be(new TextSpan(1, 0));
    }
}

public class StartsWith
{
    public class Chars
    {
        [Fact]
        public void null_if_false()
        {
            SourceSpan span = Source.From("ABC");
            span.StartsWith('D').Should().BeNull();
        }

        [Fact]
        public void null_if_character_before_span()
        {
            SourceSpan span = Source.From("ABC");
            span++;
            span.StartsWith('A').Should().BeNull();
        }

        [Fact]
        public void TextSpan_of_length_1()
        {
            SourceSpan span = Source.From("ABCEFGHIJKLMNOPQR");
            span++;
            var text = span.StartsWith('B');
            text.Should().Be(new TextSpan(1, 1));
        }
    }

    public class Strings
    {
        [Fact]
        public void null_if_false()
        {
            SourceSpan span = Source.From("ABC");
            span.StartsWith("D").Should().BeNull();
        }

        [Fact]
        public void null_if_too_long()
        {
            SourceSpan span = Source.From("ABC");
            span.StartsWith("ABCD").Should().BeNull();
        }

        [Theory]
        [InlineData("A")]
        [InlineData("AB")]
        public void null_if_character_before_span(string value)
        {
            SourceSpan span = Source.From("ABC");
            span++;
            span.StartsWith(value).Should().BeNull();
        }

        [Theory]
        [InlineData("B", 1)]
        [InlineData("BC", 2)]
        [InlineData("BCD", 3)]
        public void TextSpan_of_match(string value, int length)
        {
            SourceSpan span = Source.From("ABCDEFGHIJKLMNOPQR");
            span++;
            var text = span.StartsWith(value);
            text.Should().Be(new TextSpan(1, length));
        }
    }
}

public class Predicate
{
    [Fact]
    public void null_if_not_found()
    {
        SourceSpan span = Source.From("ABC");
        var text = span.Predicate(char.IsDigit);
        text.Should().BeNull();
    }

    [Fact]
    public void null_if_only_found_before()
    {
        SourceSpan span = Source.From("1BC");
        span++;
        span.Predicate(char.IsDigit).Should().BeNull();
    }

    [Fact]
    public void null_if_only_found_after()
    {
        SourceSpan span = Source.From("AB3");
        span = span.Trim(new(0, 2));
        span.Predicate(char.IsDigit).Should().BeNull();
    }

    [Theory]
    [InlineData("1234")]
    [InlineData("1234ABC")]
    public void TextSpan_of_length_matches(string source)
    {
        SourceSpan span = Source.From(source);
        span++;
        var text = span.Predicate(char.IsDigit);
        text.Should().Be(new TextSpan(1, 3));
    }
}

public class First
{
    [Theory]
    [InlineData('A', 0)]
    [InlineData('B', 1)]
    [InlineData('C', 2)]
    public void returns_first(char first, int skip)
    {
        SourceSpan span = Source.From("ABCD");
        span = span.Skip(skip);
        span[0].Should().Be(first);
    }

    [Fact]
    public void throws_for_empty()
    {
        SourceSpan span = Source.From(string.Empty);
        span.Invoking(s => s.First).Should().Throw<IndexOutOfRangeException>();
    }
}
