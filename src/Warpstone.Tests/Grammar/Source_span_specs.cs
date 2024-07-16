using Microsoft.CodeAnalysis.Text;
using Warpstone;

namespace Source_span_specs;

public class Line
{
    [Theory]
    [InlineData(00, "Hello\nWorld!", "Hello")]
    [InlineData(00, "Hello\r\nWorld!", "Hello")]
    [InlineData(06, "Hello\nWorld!", "World!")]
    [InlineData(12, "Hello\nWorld\n!", "!")]
    public void trims_until_new_line(int left, string text, string line)
    {
        var source = Source.Text(text);
        var span = new SourceSpan(source).TrimLeft(left);
        source.ToString(span.Line()).Should().Be(line);
    }

    [Fact]
    public void matches_until_end_of_line()
    {
        Source.Span("Hello\r\n").Line(new(".+")).Should().Be(new TextSpan(0, 5));
    }
}

public class Regex_
{
    [Fact]
    public void pattern_must_match_from_start()
    {
        var span = Source.Span("Hello");

        new Regex("el+").Invoking(span.Regex)
            .Should().Throw<Exception>()
            .WithMessage("Pattern 'el+' did match from the start.");
    }

    [Theory]
    [InlineData(0, "Hello")]
    [InlineData(2, "Hello")]
    [InlineData(5, "Hello")]
    public void matches_empty_pattern(int left, string text)
    {
        var span = Source.Span(text).TrimLeft(left);
        span.Regex(new("^Z*")).Should().Be(new TextSpan(left, 0));
    }

    [Theory]
    [InlineData(0, "Hello")]
    [InlineData(2, "Hello")]
    [InlineData(5, "Hello")]
    public void returns_null_on_no_match(int left, string text)
    {
        var span = Source.Span(text).TrimLeft(left);
        span.Regex(new("^Z+")).Should().BeNull();
    }
}

public class Starts_with
{
    public class Char
    {
        [Theory]
        [InlineData(0, "Hello", 'A')]
        [InlineData(1, "Hello", 'H')]
        [InlineData(5, "Hello", '0')]
        public void returns_null_if_no_match(int left, string text, char first)
        {
            var span = Source.Span(text).TrimLeft(left);
            span.StartsWith(first).Should().BeNull();
        }

        [Theory]
        [InlineData("hello", 'H')]
        [InlineData("Hello", 'h')]
        public void is_case_sensitive(string text, char first)
        {
            var span = Source.Span(text);
            span.StartsWith(first).Should().BeNull();
        }

        [Theory]
        [InlineData(0, "hello")]
        [InlineData(1, " hell")]
        public void returns_length_1(int left, string text)
        {
            var span = Source.Span(text).TrimLeft(left);
            span.StartsWith('h').Should().Be(new TextSpan(left, 1));
        }
    }

    public class String
    {
        [Theory]
        [InlineData(0, "Hello", "HA")]
        [InlineData(1, "Hello", "be")]
        [InlineData(4, "Hello", "o ")]
        [InlineData(5, "Hello", "ox")]
        public void returns_null_if_no_match(int left, string text, string first)
        {
            var span = Source.Span(text).TrimLeft(left);
            span.StartsWith(first).Should().BeNull();
        }

        [Theory]
        [InlineData("hello", "He")]
        [InlineData("Hello", "he")]
        [InlineData("Hello", "HE")]
        public void is_case_sensitive(string text, string first)
        {
            var span = Source.Span(text);
            span.StartsWith(first).Should().BeNull();
        }

        [Theory]
        [InlineData(0, "hello", "hell")]
        [InlineData(1, " hell", "hel")]
        public void returns_length_of_left_with(int left, string text, string first)
        {
            var span = Source.Span(text).TrimLeft(left);
            span.StartsWith(first).Should().Be(new TextSpan(left, first.Length));
        }
    }
}


