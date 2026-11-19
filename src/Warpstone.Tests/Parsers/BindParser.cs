namespace Warpstone.Tests.Parsers;

[TestFixtureSource(typeof(TestOptions), nameof(TestOptions.All))]
public sealed class BindParser(ParseOptions options)
{
    private static readonly IParser<string> Tag
        = Char('<').Then(Regex("[a-z]+")).ThenSkip(Char('>'))
        .Bind(name => Regex("[a-z]*").ThenSkip(String($"</{name}>")));

    private static readonly IParser<IReadOnlyList<char>> LengthPrefixed
        = Regex("[0-9]").Transform(int.Parse).Bind(n => Multiple(Char('x'), n)).ThenEnd();

    private static readonly IParser<int> Num = Regex("[0-9]+").Transform(int.Parse);

    // Left recursive through the source, with a new parser selected on every call.
    private static readonly IParser<int> Sum
        = Or(Lazy(() => Sum!).ThenSkip(Char('+')).Bind(l => Num.Transform(r => l + r)), Num);

    // Recursive through the selected parser only, which the grammar analysis cannot see.
    private static readonly IParser<int> Count
        = Or(Char('a').Bind(_ => Count!).Transform(n => n + 1), Create(0));

    // Left recursive through the selected parser: the source consumes nothing.
    private static readonly IParser<int> SelfLeft = Create(0).Bind(_ => SelfLeft!);

    // As above, but every call selects a new parser instance with its own implementation.
    private static readonly IParser<int> FreshLeft = Create(0).Bind(_ => Lazy(() => FreshLeft!).Transform(x => x));

    private static readonly IParser<string> Choice
        = Or(Char('a'), Char('b')).Bind(c => c == 'a' ? String("1") : String("2")).ThenEnd();

    [TestCase("<b>hi</b>", "hi")]
    [TestCase("<div></div>", "")]
    public void Matches_closing_tag(string input, string expected)
        => Tag.Parse(input, options).Should().Be(expected);

    [TestCase("<b>hi</i>")]
    [TestCase("<b>hi</bb>")]
    public void Rejects_other_closing_tag(string input)
        => Tag.TryParse(input, options).Success.Should().BeFalse();

    [TestCase("0", 0)]
    [TestCase("3xxx", 3)]
    public void Parses_length_prefix(string input, int expected)
        => LengthPrefixed.Parse(input, options).Should().HaveCount(expected);

    [TestCase("3xx")]
    [TestCase("3xxxx")]
    public void Rejects_wrong_length(string input)
        => LengthPrefixed.TryParse(input, options).Success.Should().BeFalse();

    [TestCase("a1", "1")]
    [TestCase("b2", "2")]
    public void Selects_parser_by_value(string input, string expected)
        => Choice.Parse(input, options).Should().Be(expected);

    [Test]
    public void Rejects_unselected_parser()
        => Choice.TryParse("a2", options).Success.Should().BeFalse();

    [Test]
    public void Grows_left_recursive_source()
        => Sum.Parse("1+2+3", options).Should().Be(6);

    [TestCase(0)]
    [TestCase(100)]
    [TestCase(50_000)]
    public void Recurses_through_selected_parser(int count)
    {
        if (options.ExecutionMode == ParserExecutionMode.Recursive && count > 100)
        {
            var parse = () => Count.TryParse(new string('a', count), options);
            parse.Should().Throw<InsufficientExecutionStackException>();
            return;
        }

        Count.Parse(new string('a', count), options).Should().Be(count);
    }

    [Test]
    public void Detects_left_recursion_through_selected_parser()
        => ShouldFailWith<InfiniteRecursionError>(SelfLeft.TryParse(string.Empty, options));

    [Test]
    public void Detects_left_recursion_through_new_parser_instances()
        => ShouldFailWith<InfiniteRecursionError>(FreshLeft.TryParse(string.Empty, options));

    [Test]
    public void Reports_throwing_select()
        => ShouldFailWith<TransformationError>(Create(0).Bind<int, int>(_ => throw new InvalidOperationException()).TryParse(string.Empty, options));

    [Test]
    public void Reports_null_select()
        => ShouldFailWith<TransformationError>(Create(0).Bind<int, int>(_ => null!).TryParse(string.Empty, options));

    private static void ShouldFailWith<TError>(IParseResult result)
    {
        result.Success.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().BeOfType<TError>();
    }
}
