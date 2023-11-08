namespace Warpstone.V2;

using Warpstone.V2.Parsers;

public static class Program
{
    private static readonly IParser<string> ManyARight = EOF.Or(Char('a').Concat(() => ManyARight!));

    private static readonly Func<IParser<string>> ManyALeftLazy = () => ManyALeft!;
    private static readonly IParser<string> ManyALeft = ManyALeftLazy.Concat(Char('a')).Or(Char('a'));

    private static readonly IParser<string> Digit
        = Char('0')
        .Or(Char('1'))
        .Or(Char('2'))
        .Or(Char('3'))
        .Or(Char('4'))
        .Or(Char('5'))
        .Or(Char('6'))
        .Or(Char('7'))
        .Or(Char('8'))
        .Or(Char('9'));

    private static readonly Func<IParser<string>> LazyNumber = () => Number!;
    private static readonly IParser<string> Number
        = LazyNumber.Concat(Digit).Or(Digit);

    private static readonly Func<IParser<string>> LazyExp = () => Exp!;
    private static readonly Func<IParser<string>> LazyAddSub = () => AddSub!;
    private static readonly IParser<string> Add = LazyExp.Concat(Char('+')).Concat(LazyAddSub);
    private static readonly IParser<string> Sub = LazyExp.Concat(Char('-')).Concat(LazyAddSub);
    private static readonly IParser<string> AddSub = Add.Or(Sub).Or(Number);

    private static readonly Func<IParser<string>> LazyMulDiv = () => MulDiv!;
    private static readonly IParser<string> Mul = LazyExp.Concat(Char('*')).Concat(LazyMulDiv);
    private static readonly IParser<string> Div = LazyExp.Concat(Char('/')).Concat(LazyMulDiv);
    private static readonly IParser<string> MulDiv = Mul.Or(Div).Or(AddSub);

    private static readonly IParser<string> Exp = MulDiv;

    public static void Main(string[] args)
    {
        var parser
            = Char('a')
            .Concat(
                Char('b')
                .Concat(
                    Char('c')
                    .Or(Char('d'))))
            .Concat(EOF);

        var result1 = Do(parser, "abc");
        var result2 = Do(parser, "abd");
        var result3 = Do(parser, "abe");
        var result4 = Do(parser, "abdz");
        var result5 = Do(ManyARight, "");
        var result6 = Do(ManyARight, "a");
        var result7 = Do(ManyARight, "aa");
        var result8 = Do(ManyARight, "aaaaaaaaaaaaaa");
        var result9 = Do(ManyARight, new string('a', 10_000));

        var result10 = Do(ManyALeft, "aaa");
        var result11 = Do(ManyALeft, new string('a', 100));
        var result12 = Do(ManyALeft, new string('a', 10_000));
        var result13 = Do(Exp, "1");
        var result14 = Do(Exp, "12");
        var result15 = Do(Exp, "12+54");
        var result16 = Do(Exp, "12+54+23");
        var result17 = Do(Exp, "12-54-23");
        var result18 = Do(Exp, "1+2-3+4-2");
        var result19 = Do(Exp, "1-2+3-4+2");
    }

    private static IParseResult<string> Do(IParser<string> parser, string input)
    {
        var result = parser.Parse(input);
        if (result.Status == ParseStatus.Match)
        {
            Console.WriteLine($"[{result.Value == input}] Expected: '{input}' Found: '{result.Value}'");
        }
        else
        {
            Console.WriteLine($"[False] Expected: '{input}' Found: Error");
        }
        return result;
    }

    private static IParser<(T1, T2)> Then<T1, T2>(this Func<IParser<T1>> p1, Func<IParser<T2>> p2)
        => new SequenceParser<T1, T2>(p1, p2);

    private static IParser<(T1, T2)> Then<T1, T2>(this Func<IParser<T1>> p1, IParser<T2> p2)
        => p1.Then(() => p2);

    private static IParser<(T1, T2)> Then<T1, T2>(this IParser<T1> p1, Func<IParser<T2>> p2)
        => new SequenceParser<T1, T2>(() => p1, p2);

    private static IParser<(T1, T2)> Then<T1, T2>(this IParser<T1> p1, IParser<T2> p2)
        => p1.Then(() => p2);

    private static IParser<T> Or<T>(this IParser<T> p1, IParser<T> p2)
        => new ChoiceParser<T>(() => p1, () => p2);

    private static IParser<TOut> Map<TIn, TOut>(this IParser<TIn> p, Func<TIn, TOut> map)
        => new TransformationParser<TIn, TOut>(() => p, map);

    private static IParser<string> Char(char c)
        => new CharParser(c).Map(c => c.ToString());

    private static IParser<string> Concat(this Func<IParser<string>> p1, Func<IParser<string>> p2)
        => p1.Then(p2).Map(x => x.Item1 + x.Item2);

    private static IParser<string> Concat(this Func<IParser<string>> p1, IParser<string> p2)
        => p1.Then(p2).Map(x => x.Item1 + x.Item2);

    private static IParser<string> Concat(this IParser<string> p1, Func<IParser<string>> p2)
        => p1.Then(p2).Map(x => x.Item1 + x.Item2);

    private static IParser<string> Concat(this IParser<string> p1, IParser<string> p2)
        => p1.Then(p2).Map(x => x.Item1 + x.Item2);

    private static IParser<string> EOF => EndOfFileParser.Instance;
}
