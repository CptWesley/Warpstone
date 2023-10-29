﻿namespace Warpstone.V2;

using System.Runtime.CompilerServices;
using Warpstone.V2.Parsers;

public static class Program
{
    private static readonly IParser<string> ManyA = EOF.Or(Char('a').Concat(() => ManyA!));

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
        var result1 = parser.Parse("abc");
        var result2 = parser.Parse("abd");
        var result3 = parser.Parse("abe");
        var result4 = parser.Parse("abdz");
        var result5 = ManyA.Parse("");
        var result6 = ManyA.Parse("a");
        var result7 = ManyA.Parse("aa");
        var result8 = ManyA.Parse("aaaaaaaaaaaaaa");
    }

    private static IParser<(T1, T2)> Then<T1, T2>(this IParser<T1> p1, IParser<T2> p2)
        => new SequenceParser<T1, T2>(() => p1, () => p2);

    private static IParser<(T1, T2)> Then<T1, T2>(this IParser<T1> p1, Func<IParser<T2>> p2)
        => new SequenceParser<T1, T2>(() => p1, p2);

    private static IParser<T> Or<T>(this IParser<T> p1, IParser<T> p2)
        => new ChoiceParser<T>(() => p1, () => p2);

    private static IParser<TOut> Map<TIn, TOut>(this IParser<TIn> p, Func<TIn, TOut> map)
        => new TransformationParser<TIn, TOut>(() => p, map);

    private static IParser<string> Char(char c)
        => new CharParser(c).Map(c => c.ToString());

    private static IParser<string> Concat(this IParser<string> p1, IParser<string> p2)
        => p1.Then(p2).Map(x => x.Item1 + x.Item2);

    private static IParser<string> Concat(this IParser<string> p1, Func<IParser<string>> p2)
        => p1.Then(p2).Map(x => x.Item1 + x.Item2);

    private static IParser<string> EOF => EndOfFileParser.Instance;
}
