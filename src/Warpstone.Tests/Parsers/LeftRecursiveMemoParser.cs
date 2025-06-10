namespace Warpstone.Tests.Parsers;

public static class LeftRecursiveMemoParser
{
    public static readonly IEnumerable<object[]> Options = new ParseOptions[][]
    {
        [
            ParseOptions.Default with
            {
                ExecutionMode = ParserExecutionMode.Auto,
                EnableAutomaticLeftRecursion = false,
                EnableAutomaticMemoization = false,
            }
        ],
        [
            ParseOptions.Default with
            {
                ExecutionMode = ParserExecutionMode.Recursive,
                EnableAutomaticLeftRecursion = false,
                EnableAutomaticMemoization = false,
            }
        ],
        [
            ParseOptions.Default with
            {
                ExecutionMode = ParserExecutionMode.Iterative,
                EnableAutomaticLeftRecursion = false,
                EnableAutomaticMemoization = false,
            }
        ],
    };

    [Theory]
    [MemberData(nameof(Options))]
    public static void ParseCorrectlySimple(ParseOptions options)
    {
        IParser<string> num = Memo(Regex(@"[0-9]+"));
        IParser<string> exp = null!;
        IParser<string> add = Lazy(() => exp).ThenSkip(Char('+')).ThenAdd(num).Transform((l, r) => $"({l}+{r})");
        exp = Grow(Or(add, num));

        var result = exp.TryParse("1+2+3", options);
        result.Success.Should().BeTrue();
        result.Value.Should().Be("((1+2)+3)");
    }

    private static IParser<string> GetArithmeticParser()
    {
        IParser<string> e0r = null!;
        IParser<string> e1r = null!;
        IParser<string> e2r = null!;

        var e0 = Memo(() => e0r.WithName("e0r")).WithName("e0");
        var e1 = Grow(() => e1r.WithName("e1r")).WithName("e1");
        var e2 = Grow(() => e2r.WithName("e2r")).WithName("e2");

        var n = Regex(@"[0-9]+").WithName("n");

        IParser<string> BinOp(IParser<string> left, char op, IParser<string> right)
            => left.ThenSkip(Char(op)).ThenAdd(right).Transform((l, r) => $"({l}{op}{r})");

        e0r = n;

        var mul = BinOp(e1, '*', e0).WithName("mul");
        var div = BinOp(e1, '/', e0).WithName("div");

        e1r = Or(mul, div, e0);

        var add = BinOp(e2, '+', e1).WithName("add");
        var sub = BinOp(e2, '-', e1).WithName("sub");

        e2r = Or(add, sub, e1);

        var e = e2;

        return e;
    }

    [Theory]
    [MemberData(nameof(Options))]
    public static void ParseCorrectlyArithmeticComplex(ParseOptions options)
    {
        var e = GetArithmeticParser();

        var result = e.TryParse("1+2+3*4/2-6+3", options);
        result.Success.Should().BeTrue();
        result.Value.Should().Be("((((1+2)+((3*4)/2))-6)+3)");
    }

    [Theory]
    [MemberData(nameof(Options))]
    public static void ParseCorrectlyArithmeticMul(ParseOptions options)
    {
        var e = GetArithmeticParser();

        var result = e.TryParse("1*2*3*4*5*6", options);
        result.Success.Should().BeTrue();
        result.Value.Should().Be("(((((1*2)*3)*4)*5)*6)");
    }

    [Theory]
    [MemberData(nameof(Options))]
    public static void ParseCorrectlyArithmeticDiv(ParseOptions options)
    {
        var e = GetArithmeticParser();

        var result = e.TryParse("1/2/3/4/5/6", options);
        result.Success.Should().BeTrue();
        result.Value.Should().Be("(((((1/2)/3)/4)/5)/6)");
    }

    [Theory]
    [MemberData(nameof(Options))]
    public static void ParseCorrectlyArithmeticAdd(ParseOptions options)
    {
        var e = GetArithmeticParser();

        var result = e.TryParse("1+2+3+4+5+6", options);
        result.Success.Should().BeTrue();
        result.Value.Should().Be("(((((1+2)+3)+4)+5)+6)");
    }

    [Theory]
    [MemberData(nameof(Options))]
    public static void ParseCorrectlyArithmeticSub(ParseOptions options)
    {
        var e = GetArithmeticParser();

        var result = e.TryParse("1-2-3-4-5-6", options);
        result.Success.Should().BeTrue();
        result.Value.Should().Be("(((((1-2)-3)-4)-5)-6)");
    }

    [Theory]
    [MemberData(nameof(Options))]
    public static void ParseCorrectlyArithmeticAddSub(ParseOptions options)
    {
        var e = GetArithmeticParser();

        var result = e.TryParse("1+2-3+4-5-6", options);
        result.Success.Should().BeTrue();
        result.Value.Should().Be("(((((1+2)-3)+4)-5)-6)");
    }
}
