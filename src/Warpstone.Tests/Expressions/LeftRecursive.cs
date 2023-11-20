namespace Warpstone.Tests.Expressions;

public sealed class LeftRecursive
{
    private interface IExp;

    private sealed record Num(int Value) : IExp
    {
        public override string ToString()
            => Value.ToString();
    }

    private sealed record Add(IExp Left, IExp Right) : IExp
    {
        public override string ToString()
            => $"({Left} + {Right})";
    }

    private sealed record Mul(IExp Left, IExp Right) : IExp
    {
        public override string ToString()
            => $"({Left} * {Right})";
    }

    public static readonly IParser<string> OptionalWhitespaces
        = Regex(@"\s*");

    private static readonly IParser<IExp> ParenthesisParser
        = Char('(')
        .ThenSkip(OptionalWhitespaces)
        .Then(Lazy(() => Exp1!))
        .ThenSkip(OptionalWhitespaces)
        .ThenSkip(Char(')'));

    private static readonly IParser<IExp> NumParser
        = Regex(@"-?[0-9]+").Transform(x => new Num(int.Parse(x)));

    private static readonly IParser<IExp> Exp3
        = Or(NumParser, ParenthesisParser);

    private static readonly IParser<IExp> MulParser
        = Lazy(() => Exp2!)
        .ThenSkip(OptionalWhitespaces)
        .ThenSkip(Char('*'))
        .ThenSkip(OptionalWhitespaces)
        .ThenAdd(Exp3)
        .Transform((x, y) => new Mul(x, y));

    private static readonly IParser<IExp> Exp2
        = Or(MulParser, Exp3);

    private static readonly IParser<IExp> AddParser
        = Lazy(() => Exp1!)
        .ThenSkip(OptionalWhitespaces)
        .ThenSkip(Char('+'))
        .ThenSkip(OptionalWhitespaces)
        .ThenAdd(Exp2)
        .Transform((x, y) => new Add(x, y));

    private static readonly IParser<IExp> Exp1
        = Or(AddParser, Exp2);

    private static readonly IParser<IExp> Exp
        = OptionalWhitespaces
        .Then(Exp1)
        .ThenSkip(OptionalWhitespaces)
        .ThenEnd();

    [Theory]
    [InlineData("0", "0")]
    [InlineData(" 0", "0")]
    [InlineData("0 ", "0")]
    [InlineData(" 0 ", "0")]
    [InlineData("(42)", "42")]
    [InlineData("1 + 2", "(1 + 2)")]
    [InlineData("1 * 2", "(1 * 2)")]
    [InlineData("1 * 2 + 3", "((1 * 2) + 3)")]
    [InlineData("1 + 2 * 3", "(1 + (2 * 3))")]
    [InlineData("(1 + 2) * 3", "((1 + 2) * 3)")]
    [InlineData("1 + 2 + 3", "((1 + 2) + 3)")]
    [InlineData("1 * 2 * 3", "((1 * 2) * 3)")]
    [InlineData("1 + 2 + 3 * 4 * 5 + 6 * 7 * 8 + 9", "((((1 + 2) + ((3 * 4) * 5)) + ((6 * 7) * 8)) + 9)")]
    public static void Parse_correctly(string input, string expected)
    {
        var parsed = Exp.Parse(input);
        parsed.ToString().Should().Be(expected);
    }
}
