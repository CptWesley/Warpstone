using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warpstone.Internal.Parsers;

namespace Warpstone;

public static class Parsers
{
    public static IParser<string> End { get; } = String(string.Empty);

    public static IParser<string> String(string value)
        => new StringParser(value);

    public static IParser<T> Lazy<T>(Func<IParser<T>> parser)
        => new LazyParser<T>(parser);

    public static IParser<T> Or<T>(IParser<T> option1, IParser<T> option2, params IEnumerable<IParser<T>> options)
    {
        var result = new OrParser<T>(option1, option2);

        foreach (var option in options)
        {
            result = new OrParser<T>(result, option);
        }

        return result;
    }

    public static IParser<(TLeft Left, TRight Right)> ThenAdd<TLeft, TRight>(this IParser<TLeft> first, IParser<TRight> second)
    {
        var tLeft = typeof(TLeft);
        var tRight = typeof(TRight);

        var leftBoxed = tLeft.IsValueType;
        var rightBoxed = tRight.IsValueType;

        var genericParserType = (leftBoxed, rightBoxed) switch
        {
            (false, false) => typeof(AndRefRefParser<,>),
            (false, true) => typeof(AndRefBoxedParser<,>),
            (true, false) => typeof(AndBoxedRefParser<,>),
            (true, true) => typeof(AndBoxedBoxedParser<,>),
        };

        var parserType = genericParserType.MakeGenericType(tLeft, tRight);
        var parser = (IParser<(TLeft, TRight)>)Activator.CreateInstance(parserType, [first, second])!;
        return parser;
    }

    public static IParser<TOut> Transform<TIn, TOut>(this IParser<TIn> parser, Func<TIn, TOut> transform)
    {
        var t = typeof(TIn);
        var boxed = t.IsValueType;
        var genericParserType = boxed ? typeof(MapBoxedParser<,>) : typeof(MapRefParser<,>);
        var parserType = genericParserType.MakeGenericType(t, typeof(TOut));
        var result = (IParser<TOut>)Activator.CreateInstance(parserType, [parser, transform])!;
        return result;
    }
}
