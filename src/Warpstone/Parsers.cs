using System.Text.RegularExpressions;

namespace Warpstone;

public static class Parsers
{
    public static IParser<string> End { get; } = EndParser.Instance;

    public static IParser<char> Char(char value)
        => new CharacterParser(value);

    public static IParser<string> String(string value, CultureInfo? culture, CompareOptions options)
        => new StringParser(value, culture ?? CultureInfo.CurrentCulture, options);

    public static IParser<string> String(string value, bool ignoreCase, CultureInfo? culture)
        => String(value: value, culture: culture, options: ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);

    public static IParser<string> String(string value, StringComparison comparisonType)
    {
        var (culture, options) = comparisonType switch
        {
            StringComparison.InvariantCulture => (CultureInfo.InvariantCulture, CompareOptions.None),
            StringComparison.InvariantCultureIgnoreCase => (CultureInfo.InvariantCulture, CompareOptions.IgnoreCase),
            StringComparison.CurrentCulture => (CultureInfo.CurrentCulture, CompareOptions.None),
            StringComparison.CurrentCultureIgnoreCase => (CultureInfo.CurrentCulture, CompareOptions.IgnoreCase),
            StringComparison.Ordinal => (CultureInfo.InvariantCulture, CompareOptions.Ordinal),
            StringComparison.OrdinalIgnoreCase => (CultureInfo.InvariantCulture, CompareOptions.OrdinalIgnoreCase),
            _ => throw new NotSupportedException($"Currently the comparison type '{comparisonType}' is not yet supported."),
        };

        return String(value: value, culture: culture, options: options);
    }

    public static IParser<string> String(string value, bool ignoreCase)
        => String(value: value, ignoreCase: ignoreCase, culture: null);

    public static IParser<string> String(string value)
        => String(value: value, ignoreCase: false);

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
