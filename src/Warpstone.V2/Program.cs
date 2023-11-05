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
        /*
        var result1 = parser.Parse("abc");
        var result2 = parser.Parse("abd");
        var result3 = parser.Parse("abe");
        var result4 = parser.Parse("abdz");
        var result5 = ManyARight.Parse("");
        var result6 = ManyARight.Parse("a");
        var result7 = ManyARight.Parse("aa");
        var result8 = ManyARight.Parse("aaaaaaaaaaaaaa");
        */
        //var result9 = ManyARight.Parse(new string('a', 10_000));

        //var result10 = ManyALeft.Parse("aaa");
        //var result11 = ManyALeft.Parse(new string('a', 100));
        var result12 = Exp.Parse("1");
        var result13 = Exp.Parse("12");
        var result14 = Exp.Parse("12+54");
        var result15 = Exp.Parse("12+54+23");
        var result16 = Exp.Parse("12-54-23");
        var result17 = Exp.Parse("1+2-3+4-2");
        var result18 = Exp.Parse("1-2+3-4+2");
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
