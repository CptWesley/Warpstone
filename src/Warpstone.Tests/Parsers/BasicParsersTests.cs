namespace Warpstone.Tests.Parsers;

/// <summary>
/// Test class for the <see cref="BasicParsers"/> class.
/// </summary>
public static class BasicParsersTests
{
    /// <summary>
    /// Checks that parsing chars works correctly.
    /// </summary>
    [Fact]
    public static void CharParserCorrect()
        => AssertThat(Char('x').Parse("xyz")).IsEqualTo('x');

    /// <summary>
    /// Checks that parsing chars works correctly.
    /// </summary>
    [Fact]
    public static void CharParserIncorrect()
        => AssertThat(() => Char('x').Parse("yz")).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that parsing strings works correctly.
    /// </summary>
    [Fact]
    public static void StringParserCorrect()
        => AssertThat(String("abc").Parse("abcdef")).IsEqualTo("abc");

    /// <summary>
    /// Checks that parsing strings works correctly.
    /// </summary>
    [Fact]
    public static void StringParserIncorrect()
        => AssertThat(() => String("abc").Parse("def")).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that parsing regexes works correctly.
    /// </summary>
    [Fact]
    public static void RegexParserCorrect()
        => AssertThat(Regex("[abc]+").Parse("abcabcdef")).IsEqualTo("abcabc");

    /// <summary>
    /// Checks that parsing regexes works correctly.
    /// </summary>
    [Fact]
    public static void RegexParserIncorrect()
        => AssertThat(() => Regex("[abc]+").Parse("def")).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that branch parsing works correctly.
    /// </summary>
    [Fact]
    public static void OrParserCorrectLeft()
        => AssertThat(Or(Char('x'), Char('y')).Parse("xyz")).IsEqualTo('x');

    /// <summary>
    /// Checks that branch parsing works correctly.
    /// </summary>
    [Fact]
    public static void OrParserCorrectRight()
        => AssertThat(Or(Char('x'), Char('y')).Parse("yz")).IsEqualTo('y');

    /// <summary>
    /// Checks that branch parsing works correctly.
    /// </summary>
    [Fact]
    public static void OrParserCorrect3()
        => AssertThat(Or(Char('x'), Char('y'), Char('z')).Parse("z")).IsEqualTo('z');

    /// <summary>
    /// Checks that branch parsing works correctly.
    /// </summary>
    [Fact]
    public static void OrParserIncorrect()
        => AssertThat(() => Or(Char('x'), Char('y')).Parse("zzz")).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that parsing one or more works correctly.
    /// </summary>
    [Fact]
    public static void OneOrMoreParserCorrectOne()
        => AssertThat(OneOrMore(Char('x')).Parse("xyz")).ContainsExactly('x');

    /// <summary>
    /// Checks that parsing one or more works correctly.
    /// </summary>
    [Fact]
    public static void OneOrMoreParserCorrectMore()
        => AssertThat(OneOrMore(Char('x')).Parse("xxxyz")).ContainsExactly('x', 'x', 'x');

    /// <summary>
    /// Checks that parsing one or more works correctly with delimiters.
    /// </summary>
    [Fact]
    public static void OneOrMoreParserCorrectMoreDelimited1()
        => AssertThat(OneOrMore(Char('x'), Char('y')).Parse("xyxyxzzz")).ContainsExactly('x', 'x', 'x');

    /// <summary>
    /// Checks that parsing one or more works correctly with delimiters.
    /// </summary>
    [Fact]
    public static void OneOrMoreParserCorrectMoreDelimited2()
        => AssertThat(OneOrMore(Char('x'), Char('y')).Parse("xyxyxy")).ContainsExactly('x', 'x', 'x');

    /// <summary>
    /// Checks that parsing one or more works correctly.
    /// </summary>
    [Fact]
    public static void OneOrMoreParserIncorrect()
        => AssertThat(() => OneOrMore(Char('x')).Parse("yz")).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that parsing many works correctly.
    /// </summary>
    [Fact]
    public static void ManyParserCorrectNone()
        => AssertThat(Many(Char('x')).Parse("yz")).IsEmpty();

    /// <summary>
    /// Checks that parsing many works correctly.
    /// </summary>
    [Fact]
    public static void ManyParserCorrectOne()
        => AssertThat(Many(Char('x')).Parse("xyz")).ContainsExactly('x');

    /// <summary>
    /// Checks that parsing many works correctly.
    /// </summary>
    [Fact]
    public static void ManyParserCorrectMore()
        => AssertThat(Many(Char('x')).Parse("xxxyz")).ContainsExactly('x', 'x', 'x');

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Fact]
    public static void ThenParserCorrect()
        => AssertThat(Char('x').Then(Char('y')).Parse("xyz")).IsEqualTo('y');

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Fact]
    public static void ThenParserIncorrectLeft()
        => AssertThat(() => Char('x').Then(Char('y')).Parse("yz")).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Fact]
    public static void ThenParserIncorrectRight()
        => AssertThat(() => Char('x').Then(Char('y')).Parse("xz")).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Fact]
    public static void ThenSkipParserCorrect()
        => AssertThat(Char('x').ThenSkip(Char('y')).Parse("xyz")).IsEqualTo('x');

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Fact]
    public static void ThenSkipParserIncorrectLeft()
        => AssertThat(() => Char('x').ThenSkip(Char('y')).Parse("yz")).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Fact]
    public static void ThenSkipParserIncorrectRight()
        => AssertThat(() => Char('x').ThenSkip(Char('y')).Parse("xz")).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Fact]
    public static void ThenAddParserCorrect2()
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .Parse("abcdefghijklmnop")).IsEqualTo(('a', 'b'));

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Fact]
    public static void ThenAddParserCorrect3()
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .ThenAdd(Char('c'))
            .Parse("abcdefghijklmnop")).IsEqualTo(('a', 'b', 'c'));

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Fact]
    public static void ThenAddParserCorrect4()
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .ThenAdd(Char('c'))
            .ThenAdd(Char('d'))
            .Parse("abcdefghijklmnop")).IsEqualTo(('a', 'b', 'c', 'd'));

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Fact]
    public static void ThenAddParserCorrect5()
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .ThenAdd(Char('c'))
            .ThenAdd(Char('d'))
            .ThenAdd(Char('e'))
            .Parse("abcdefghijklmnop")).IsEqualTo(('a', 'b', 'c', 'd', 'e'));

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Fact]
    public static void ThenAddParserCorrect6()
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .ThenAdd(Char('c'))
            .ThenAdd(Char('d'))
            .ThenAdd(Char('e'))
            .ThenAdd(Char('f'))
            .Parse("abcdefghijklmnop")).IsEqualTo(('a', 'b', 'c', 'd', 'e', 'f'));

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Fact]
    public static void ThenAddParserCorrect7()
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .ThenAdd(Char('c'))
            .ThenAdd(Char('d'))
            .ThenAdd(Char('e'))
            .ThenAdd(Char('f'))
            .ThenAdd(Char('g'))
            .Parse("abcdefghijklmnop")).IsEqualTo(('a', 'b', 'c', 'd', 'e', 'f', 'g'));

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Fact]
    public static void ThenAddParserCorrect8()
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .ThenAdd(Char('c'))
            .ThenAdd(Char('d'))
            .ThenAdd(Char('e'))
            .ThenAdd(Char('f'))
            .ThenAdd(Char('g'))
            .ThenAdd(Char('h'))
            .Parse("abcdefghijklmnop")).IsEqualTo(('a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'));

    /// <summary>
    /// Checks that parsing transformations works correctly.
    /// </summary>
    [Fact]
    public static void TransformParserCorrect1()
        => AssertThat(Char('a')
            .Transform(a => $"{a}")
            .Parse("abcdefghijklmnop")).IsEqualTo("a");
    /*
    /// <summary>
    /// Checks that parsing transformations works correctly.
    /// </summary>
    [Fact]
    public static void TransformParserCorrectIParsed()
    {
        Parsed parsed = Char('a').Then(Char('b').Transform(x => new Parsed(x))).Parse("abc");
        AssertThat(parsed.Position.Start).IsEqualTo(1);
        AssertThat(parsed.Position.Length).IsEqualTo(1);
        AssertThat(parsed.Value).IsEqualTo('b');
    }
    */
    /// <summary>
    /// Checks that parsing transformations works correctly.
    /// </summary>
    [Fact]
    public static void TransformParserCorrect2()
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .Transform((a, b) => $"{a}{b}")
            .Parse("abcdefghijklmnop")).IsEqualTo("ab");

    /// <summary>
    /// Checks that parsing transformations works correctly.
    /// </summary>
    [Fact]
    public static void TransformParserCorrect3()
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .ThenAdd(Char('c'))
            .Transform((a, b, c) => $"{a}{b}{c}")
            .Parse("abcdefghijklmnop")).IsEqualTo("abc");

    /// <summary>
    /// Checks that parsing transformations works correctly.
    /// </summary>
    [Fact]
    public static void TransformParserCorrect4()
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .ThenAdd(Char('c'))
            .ThenAdd(Char('d'))
            .Transform((a, b, c, d) => $"{a}{b}{c}{d}")
            .Parse("abcdefghijklmnop")).IsEqualTo("abcd");

    /// <summary>
    /// Checks that parsing transformations works correctly.
    /// </summary>
    [Fact]
    public static void TransformParserCorrect5()
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .ThenAdd(Char('c'))
            .ThenAdd(Char('d'))
            .ThenAdd(Char('e'))
            .Transform((a, b, c, d, e) => $"{a}{b}{c}{d}{e}")
            .Parse("abcdefghijklmnop")).IsEqualTo("abcde");

    /// <summary>
    /// Checks that parsing transformations works correctly.
    /// </summary>
    [Fact]
    public static void TransformParserCorrect6()
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .ThenAdd(Char('c'))
            .ThenAdd(Char('d'))
            .ThenAdd(Char('e'))
            .ThenAdd(Char('f'))
            .Transform((a, b, c, d, e, f) => $"{a}{b}{c}{d}{e}{f}")
            .Parse("abcdefghijklmnop")).IsEqualTo("abcdef");

    /// <summary>
    /// Checks that parsing transformations works correctly.
    /// </summary>
    [Fact]
    public static void TransformParserCorrect7()
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .ThenAdd(Char('c'))
            .ThenAdd(Char('d'))
            .ThenAdd(Char('e'))
            .ThenAdd(Char('f'))
            .ThenAdd(Char('g'))
            .Transform((a, b, c, d, e, f, g) => $"{a}{b}{c}{d}{e}{f}{g}")
            .Parse("abcdefghijklmnop")).IsEqualTo("abcdefg");

    /// <summary>
    /// Checks that parsing transformations works correctly.
    /// </summary>
    [Fact]
    public static void TransformParserCorrect8()
        => AssertThat(Char('a')
            .ThenAdd(Char('b'))
            .ThenAdd(Char('c'))
            .ThenAdd(Char('d'))
            .ThenAdd(Char('e'))
            .ThenAdd(Char('f'))
            .ThenAdd(Char('g'))
            .ThenAdd(Char('h'))
            .Transform((a, b, c, d, e, f, g, h) => $"{a}{b}{c}{d}{e}{f}{g}{h}")
            .Parse("abcdefghijklmnop")).IsEqualTo("abcdefgh");

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Fact]
    public static void ThenAddParserIncorrectLeft()
        => AssertThat(() => Char('x').ThenAdd(Char('y')).Parse("yz")).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that parsing sequentially works correctly.
    /// </summary>
    [Fact]
    public static void ThenAddParserIncorrectRight()
        => AssertThat(() => Char('x').ThenAdd(Char('y')).Parse("xz")).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that peek parsing works correctly.
    /// </summary>
    [Fact]
    public static void PeekParserCorrect()
        => AssertThat(Peek(Char('x')).ThenAdd(Char('x')).Parse("xyz")).IsEqualTo(('x', 'x'));

    /// <summary>
    /// Checks that peek parsing works correctly.
    /// </summary>
    [Fact]
    public static void PeekParserIncorrect()
        => AssertThat(() => Peek(Char('x')).Parse("yz")).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that end parsing works correctly.
    /// </summary>
    [Fact]
    public static void EndParserCorrect()
        => AssertThat(End.Parse(string.Empty)).IsEqualTo(string.Empty);

    /// <summary>
    /// Checks that end parsing works correctly.
    /// </summary>
    [Fact]
    public static void EndParserIncorrect()
        => AssertThat(() => End.Parse("yz")).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that end parsing works correctly.
    /// </summary>
    [Fact]
    public static void ThenEndParserCorrect()
        => AssertThat(Char('a').ThenEnd().Parse("a")).IsEqualTo('a');

    /// <summary>
    /// Checks that end parsing works correctly.
    /// </summary>
    [Fact]
    public static void ThenEndParserIncorrectLeft()
        => AssertThat(() => Char('a').ThenEnd().Parse("yz")).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that end parsing works correctly.
    /// </summary>
    [Fact]
    public static void ThenEndParserIncorrectRight()
        => AssertThat(() => Char('a').ThenEnd().Parse("ayz")).ThrowsExactlyException<UnexpectedTokenError>();

    /// <summary>
    /// Checks that creation parsing works correctly.
    /// </summary>
    [Fact]
    public static void CreateParserCorrect()
        => AssertThat(Create("abc").Parse(string.Empty)).IsEqualTo("abc");

    /// <summary>
    /// Checks that conditional parsing works correctly.
    /// </summary>
    [Fact]
    public static void IfParserCorrectTrue()
        => AssertThat(If(Char('x'), Char('y'), Char('z')).Parse("xyz")).IsEqualTo('y');

    /// <summary>
    /// Checks that conditional parsing works correctly.
    /// </summary>
    [Fact]
    public static void IfParserCorrectFalse()
        => AssertThat(If(Char('x'), Char('y'), Char('z')).Parse("zd")).IsEqualTo('z');

    /// <summary>
    /// Checks that conditional parsing works correctly.
    /// </summary>
    [Fact]
    public static void IfParserIncorrect()
        => AssertThat(() => If(Char('x'), Char('y'), Char('z')).Parse("g")).ThrowsExactlyException<UnexpectedTokenError>();
    /*
    /// <summary>
    /// Checks that optional parsing works correctly.
    /// </summary>
    [Fact]
    public static void MaybeParserCorrectExists()
        => AssertThat(Maybe(Char('x')).Parse("xyz")).IsExactlyInstanceOf<Some<char>>();

    /// <summary>
    /// Checks that optional parsing works correctly.
    /// </summary>
    [Fact]
    public static void MaybeParserCorrectDoesNotExist()
        => AssertThat(Maybe(Char('x')).Parse("yz")).IsExactlyInstanceOf<None<char>>();

    /// <summary>
    /// Checks that optional parsing works correctly.
    /// </summary>
    [Fact]
    public static void MaybeParserCorrectExistsDefaultValue()
        => AssertThat(Maybe(Char('x'), '-').Parse("xyz")).IsEqualTo('x');

    /// <summary>
    /// Checks that optional parsing works correctly.
    /// </summary>
    [Fact]
    public static void MaybeParserCorrectDoesNotExistDefaultValue()
        => AssertThat(Maybe(Char('x'), '-').Parse("yz")).IsEqualTo('-');

    /// <summary>
    /// Checks that optional parsing works correctly.
    /// </summary>
    [Fact]
    public static void MaybeParserOptions()
    {
        IParser<IOption<char>> parser = Maybe(Char('x'));
        IOption<char> result1 = parser.Parse("xyz");
        AssertThat(result1.HasValue).IsTrue();
        AssertThat(result1.Value).IsEqualTo('x');

        IOption<char> result2 = parser.Parse("yz");
        AssertThat(result2.HasValue).IsFalse();
        AssertThat(result2.Value).IsEqualTo(default(char));
    }
    */

    /// <summary>
    /// Checks that lazy parsing works correctly.
    /// </summary>
    [Fact]
    public static void LazyParserCorrect()
        => AssertThat(Lazy(() => Char('x')).Parse("xyz")).IsEqualTo('x');
    
    /// <summary>
    /// Checks that the expected parser works correctly.
    /// </summary>
    [Fact]
    public static void ExpectedParserCorrect()
    {
        IParser<string> parser = Or(String("x").WithName("booboo"), String("z").WithName("bahbah"));
        IParseResult<string> result = parser.TryParse("y");
        AssertThat(result.Errors).HasSize(1);
        AssertThat(result.Errors[0]).IsExactlyInstanceOf<UnexpectedTokenError>();
        AssertThat(((UnexpectedTokenError)result.Errors[0]).Expected).ContainsExactly("bahbah", "booboo");
    }
    
    /// <summary>
    /// Checks that the expected parser works correctly.
    /// </summary>
    [Fact]
    public static void StringComparisonCorrect()
    {
        IParser<string> parser = String("test-string", System.StringComparison.InvariantCultureIgnoreCase);
        IParseResult<string> result = parser.TryParse("TeSt-StRiNg");
        AssertThat(result.Success).IsTrue();
        AssertThat(result.Value).IsSameAs("test-string");
    }

    /// <summary>
    /// Checks that the unary not parser works correctly.
    /// </summary>
    [Fact]
    public static void UnaryNotTest()
    {
        IParser<string?> parser = Not(String("test")).WithName("test");
        IParseResult<string?> result1 = parser.TryParse("test string");
        AssertThat(result1.Success).IsFalse();
        IParseResult<string?> result2 = parser.TryParse("other string");
        AssertThat(result2.Success).IsTrue();
    }

    /// <summary>
    /// Checks that the binary not parser works correctly.
    /// </summary>
    [Fact]
    public static void ExceptTest()
    {
        IParser<string?> identifier = Regex("[a-zA-Z_][a-zA-Z0-9_]*").Except(Or(String("if"), String("while"), String("for"))).WithName("a keyword");
        AssertThat(identifier.TryParse("while warpstone parser").Success).IsFalse();
        AssertThat(identifier.TryParse("for warpstone parser").Success).IsFalse();
        AssertThat(identifier.TryParse("if warpstone parser").Success).IsFalse();

        AssertThat(identifier.TryParse("the warpstone parser").Success).IsTrue();
        AssertThat(identifier.TryParse("fOr warpstone parser").Success).IsTrue();
    }
    /*
    /// <summary>
    /// Checks that transformation exceptions are handled correctly.
    /// </summary>
    [Fact]
    public static void TransformationExceptionCorrect()
    {
        IParser<int> parser = String("x").Transform(x => int.Parse(x, CultureInfo.InvariantCulture));
        IParseResult<int> result = parser.TryParse("x");
        AssertThat(result.Error).IsInstanceOf<TransformationError>();
        TransformationError? error = result.Error as TransformationError;
        AssertThat(error).IsNotNull();
        AssertThat(error!.Exception).IsNotNull();
        AssertThat(error.GetMessage()).IsEqualTo(error.Exception.Message + " At 1:1.");
    }

    /// <summary>
    /// Checks that position is correct.
    /// </summary>
    [Fact]
    public static void PositionCorrect()
    {
        IParser<string> parser = Regex("\\s*").Then(String("hello"));
        ParseInputPosition pos1 = parser.TryParse("hello").Position;
        AssertThat(pos1.Start).IsEqualTo(0);
        AssertThat(pos1.End).IsEqualTo(5);
        AssertThat(pos1.StartLine).IsEqualTo(1);
        AssertThat(pos1.StartLinePosition).IsEqualTo(1);
        AssertThat(pos1.EndLine).IsEqualTo(1);
        AssertThat(pos1.EndLinePosition).IsEqualTo(5);

        ParseInputPosition pos2 = parser.TryParse("\n \t\nhello").Position;
        AssertThat(pos2.Start).IsEqualTo(0);
        AssertThat(pos2.End).IsEqualTo(9);
        AssertThat(pos2.StartLine).IsEqualTo(1);
        AssertThat(pos2.StartLinePosition).IsEqualTo(1);
        AssertThat(pos2.EndLine).IsEqualTo(3);
        AssertThat(pos2.EndLinePosition).IsEqualTo(5);
    }
    */
}
