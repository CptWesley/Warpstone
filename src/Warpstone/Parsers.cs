using Warpstone.ParserImplementations;

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

    public static IParser<string> Regex([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, RegexOptions options)
        => new RegexParser(pattern, options);

    public static IParser<string> Regex([StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
        => new RegexParser(pattern);

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

    /// <summary>
    /// Creates a parser that first applies the given parser and then applies a transformation on its result.
    /// </summary>
    /// <typeparam name="TInput">The result type of the given input parser.</typeparam>
    /// <typeparam name="TOutput">The result type of the transformation.</typeparam>
    /// <param name="parser">The given input parser.</param>
    /// <param name="transformation">The transformation to apply on the parser result.</param>
    /// <returns>A parser first applying the given parser and then applying a transformation on its result.</returns>
    public static IParser<TOutput> Transform<TInput, TOutput>(this IParser<TInput> parser, Func<TInput, TOutput> transformation)
    {
        var t = typeof(TInput);
        var boxed = t.IsValueType;
        var genericParserType = boxed ? typeof(MapBoxedParser<,>) : typeof(MapRefParser<,>);
        var parserType = genericParserType.MakeGenericType(t, typeof(TOutput));
        var result = (IParser<TOutput>)Activator.CreateInstance(parserType, [parser, transformation])!;
        return result;
    }

    /// <summary>
    /// Creates a parser that first applies the given parser and then applies a transformation on its result.
    /// </summary>
    /// <typeparam name="T1">The first result type of the given input parser.</typeparam>
    /// <typeparam name="T2">The second result type of the given input parser.</typeparam>
    /// <typeparam name="TOutput">The result type of the transformation.</typeparam>
    /// <param name="parser">The given input parser.</param>
    /// <param name="transformation">The transformation to apply on the parser result.</param>
    /// <returns>A parser first applying the given parser and then applying a transformation on its result.</returns>
    public static IParser<TOutput> Transform<T1, T2, TOutput>(this IParser<(T1 First, T2 Second)> parser, Func<T1, T2, TOutput> transformation)
        => parser.Transform(x => transformation(x.First, x.Second));

    /// <summary>
    /// Creates a parser that first applies the given parser and then applies a transformation on its result.
    /// </summary>
    /// <typeparam name="T1">The first result type of the given input parser.</typeparam>
    /// <typeparam name="T2">The second result type of the given input parser.</typeparam>
    /// <typeparam name="T3">The third result type of the given input parser.</typeparam>
    /// <typeparam name="TOutput">The result type of the transformation.</typeparam>
    /// <param name="parser">The given input parser.</param>
    /// <param name="transformation">The transformation to apply on the parser result.</param>
    /// <returns>A parser first applying the given parser and then applying a transformation on its result.</returns>
    public static IParser<TOutput> Transform<T1, T2, T3, TOutput>(this IParser<(T1 First, T2 Second, T3 Third)> parser, Func<T1, T2, T3, TOutput> transformation)
        => parser.Transform(x => transformation(x.First, x.Second, x.Third));

    /// <summary>
    /// Creates a parser that first applies the given parser and then applies a transformation on its result.
    /// </summary>
    /// <typeparam name="T1">The first result type of the given input parser.</typeparam>
    /// <typeparam name="T2">The second result type of the given input parser.</typeparam>
    /// <typeparam name="T3">The third result type of the given input parser.</typeparam>
    /// <typeparam name="T4">The fourth result type of the given input parser.</typeparam>
    /// <typeparam name="TOutput">The result type of the transformation.</typeparam>
    /// <param name="parser">The given input parser.</param>
    /// <param name="transformation">The transformation to apply on the parser result.</param>
    /// <returns>A parser first applying the given parser and then applying a transformation on its result.</returns>
    public static IParser<TOutput> Transform<T1, T2, T3, T4, TOutput>(this IParser<(T1 First, T2 Second, T3 Third, T4 Fourth)> parser, Func<T1, T2, T3, T4, TOutput> transformation)
        => parser.Transform(x => transformation(x.First, x.Second, x.Third, x.Fourth));

    /// <summary>
    /// Creates a parser that first applies the given parser and then applies a transformation on its result.
    /// </summary>
    /// <typeparam name="T1">The first result type of the given input parser.</typeparam>
    /// <typeparam name="T2">The second result type of the given input parser.</typeparam>
    /// <typeparam name="T3">The third result type of the given input parser.</typeparam>
    /// <typeparam name="T4">The fourth result type of the given input parser.</typeparam>
    /// <typeparam name="T5">The fifth result type of the given input parser.</typeparam>
    /// <typeparam name="TOutput">The result type of the transformation.</typeparam>
    /// <param name="parser">The given input parser.</param>
    /// <param name="transformation">The transformation to apply on the parser result.</param>
    /// <returns>A parser first applying the given parser and then applying a transformation on its result.</returns>
    public static IParser<TOutput> Transform<T1, T2, T3, T4, T5, TOutput>(this IParser<(T1 First, T2 Second, T3 Third, T4 Fourth, T5 Fifth)> parser, Func<T1, T2, T3, T4, T5, TOutput> transformation)
        => parser.Transform(x => transformation(x.First, x.Second, x.Third, x.Fourth, x.Fifth));

    /// <summary>
    /// Creates a parser that first applies the given parser and then applies a transformation on its result.
    /// </summary>
    /// <typeparam name="T1">The first result type of the given input parser.</typeparam>
    /// <typeparam name="T2">The second result type of the given input parser.</typeparam>
    /// <typeparam name="T3">The third result type of the given input parser.</typeparam>
    /// <typeparam name="T4">The fourth result type of the given input parser.</typeparam>
    /// <typeparam name="T5">The fifth result type of the given input parser.</typeparam>
    /// <typeparam name="T6">The sixth result type of the given input parser.</typeparam>
    /// <typeparam name="TOutput">The result type of the transformation.</typeparam>
    /// <param name="parser">The given input parser.</param>
    /// <param name="transformation">The transformation to apply on the parser result.</param>
    /// <returns>A parser first applying the given parser and then applying a transformation on its result.</returns>
    public static IParser<TOutput> Transform<T1, T2, T3, T4, T5, T6, TOutput>(this IParser<(T1 First, T2 Second, T3 Third, T4 Fourth, T5 Fifth, T6 Sixth)> parser, Func<T1, T2, T3, T4, T5, T6, TOutput> transformation)
        => parser.Transform(x => transformation(x.First, x.Second, x.Third, x.Fourth, x.Fifth, x.Sixth));

    /// <summary>
    /// Creates a parser that first applies the given parser and then applies a transformation on its result.
    /// </summary>
    /// <typeparam name="T1">The first result type of the given input parser.</typeparam>
    /// <typeparam name="T2">The second result type of the given input parser.</typeparam>
    /// <typeparam name="T3">The third result type of the given input parser.</typeparam>
    /// <typeparam name="T4">The fourth result type of the given input parser.</typeparam>
    /// <typeparam name="T5">The fifth result type of the given input parser.</typeparam>
    /// <typeparam name="T6">The sixth result type of the given input parser.</typeparam>
    /// <typeparam name="T7">The seventh result type of the given input parser.</typeparam>
    /// <typeparam name="TOutput">The result type of the transformation.</typeparam>
    /// <param name="parser">The given input parser.</param>
    /// <param name="transformation">The transformation to apply on the parser result.</param>
    /// <returns>A parser first applying the given parser and then applying a transformation on its result.</returns>
    public static IParser<TOutput> Transform<T1, T2, T3, T4, T5, T6, T7, TOutput>(this IParser<(T1 First, T2 Second, T3 Third, T4 Fourth, T5 Fifth, T6 Sixth, T7 Seventh)> parser, Func<T1, T2, T3, T4, T5, T6, T7, TOutput> transformation)
        => parser.Transform(x => transformation(x.First, x.Second, x.Third, x.Fourth, x.Fifth, x.Sixth, x.Seventh));

    /// <summary>
    /// Creates a parser that first applies the given parser and then applies a transformation on its result.
    /// </summary>
    /// <typeparam name="T1">The first result type of the given input parser.</typeparam>
    /// <typeparam name="T2">The second result type of the given input parser.</typeparam>
    /// <typeparam name="T3">The third result type of the given input parser.</typeparam>
    /// <typeparam name="T4">The fourth result type of the given input parser.</typeparam>
    /// <typeparam name="T5">The fifth result type of the given input parser.</typeparam>
    /// <typeparam name="T6">The sixth result type of the given input parser.</typeparam>
    /// <typeparam name="T7">The seventh result type of the given input parser.</typeparam>
    /// <typeparam name="T8">The eight result type of the given input parser.</typeparam>
    /// <typeparam name="TOutput">The result type of the transformation.</typeparam>
    /// <param name="parser">The given input parser.</param>
    /// <param name="transformation">The transformation to apply on the parser result.</param>
    /// <returns>A parser first applying the given parser and then applying a transformation on its result.</returns>
    public static IParser<TOutput> Transform<T1, T2, T3, T4, T5, T6, T7, T8, TOutput>(this IParser<(T1 First, T2 Second, T3 Third, T4 Fourth, T5 Fifth, T6 Sixth, T7 Seventh, T8 Eigth)> parser, Func<T1, T2, T3, T4, T5, T6, T7, T8, TOutput> transformation)
        => parser.Transform(x => transformation(x.First, x.Second, x.Third, x.Fourth, x.Fifth, x.Sixth, x.Seventh, x.Eigth));

    /// <summary>
    /// Creates a parser that applies two parsers and combines the results.
    /// </summary>
    /// <typeparam name="T1">The result type of the first parser.</typeparam>
    /// <typeparam name="T2">The result type of the second parser.</typeparam>
    /// <param name="first">The first parser.</param>
    /// <param name="second">The second parser.</param>
    /// <returns>A parser combining the results of both parsers.</returns>
    public static IParser<(T1 Left, T2 Right)> ThenAdd<T1, T2>(this IParser<T1> first, IParser<T2> second)
    {
        var tLeft = typeof(T1);
        var tRight = typeof(T2);

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
        var parser = (IParser<(T1, T2)>)Activator.CreateInstance(parserType, [first, second])!;
        return parser;
    }

    /// <summary>
    /// Creates a parser that applies two parsers and combines the results.
    /// </summary>
    /// <typeparam name="T1">The first result type of the first parser.</typeparam>
    /// <typeparam name="T2">The second result type of the first parser.</typeparam>
    /// <typeparam name="T3">The result type of the second parser.</typeparam>
    /// <param name="first">The first parser.</param>
    /// <param name="second">The second parser.</param>
    /// <returns>A parser combining the results of both parsers.</returns>
    public static IParser<(T1 First, T2 Second, T3 Third)> ThenAdd<T1, T2, T3>(this IParser<(T1 First, T2 Second)> first, IParser<T3> second)
        => first
        .ThenAdd<(T1, T2), T3>(second)
        .Transform(static (x, y) => (x.Item1, x.Item2, y));

    /// <summary>
    /// Creates a parser that applies two parsers and combines the results.
    /// </summary>
    /// <typeparam name="T1">The first result type of the first parser.</typeparam>
    /// <typeparam name="T2">The second result type of the first parser.</typeparam>
    /// <typeparam name="T3">The third result type of the first parser.</typeparam>
    /// <typeparam name="T4">The result type of the second parser.</typeparam>
    /// <param name="first">The first parser.</param>
    /// <param name="second">The second parser.</param>
    /// <returns>A parser combining the results of both parsers.</returns>
    public static IParser<(T1 First, T2 Second, T3 Third, T4 Fourth)> ThenAdd<T1, T2, T3, T4>(this IParser<(T1 First, T2 Second, T3 Third)> first, IParser<T4> second)
        => first
        .ThenAdd<(T1, T2, T3), T4>(second)
        .Transform(static (x, y) => (x.Item1, x.Item2, x.Item3, y));

    /// <summary>
    /// Creates a parser that applies two parsers and combines the results.
    /// </summary>
    /// <typeparam name="T1">The first result type of the first parser.</typeparam>
    /// <typeparam name="T2">The second result type of the first parser.</typeparam>
    /// <typeparam name="T3">The third result type of the first parser.</typeparam>
    /// <typeparam name="T4">The fourth result type of the first parser.</typeparam>
    /// <typeparam name="T5">The result type of the second parser.</typeparam>
    /// <param name="first">The first parser.</param>
    /// <param name="second">The second parser.</param>
    /// <returns>A parser combining the results of both parsers.</returns>
    public static IParser<(T1 First, T2 Second, T3 Third, T4 Fourth, T5 Fifth)> ThenAdd<T1, T2, T3, T4, T5>(this IParser<(T1 First, T2 Second, T3 Third, T4 Fourth)> first, IParser<T5> second)
        => first
        .ThenAdd<(T1, T2, T3, T4), T5>(second)
        .Transform(static (x, y) => (x.Item1, x.Item2, x.Item3, x.Item4, y));

    /// <summary>
    /// Creates a parser that applies two parsers and combines the results.
    /// </summary>
    /// <typeparam name="T1">The first result type of the first parser.</typeparam>
    /// <typeparam name="T2">The second result type of the first parser.</typeparam>
    /// <typeparam name="T3">The third result type of the first parser.</typeparam>
    /// <typeparam name="T4">The fourth result type of the first parser.</typeparam>
    /// <typeparam name="T5">The fifth result type of the first parser.</typeparam>
    /// <typeparam name="T6">The result type of the second parser.</typeparam>
    /// <param name="first">The first parser.</param>
    /// <param name="second">The second parser.</param>
    /// <returns>A parser combining the results of both parsers.</returns>
    public static IParser<(T1 First, T2 Second, T3 Third, T4 Fourth, T5 Fifth, T6 Sixth)> ThenAdd<T1, T2, T3, T4, T5, T6>(this IParser<(T1 First, T2 Second, T3 Third, T4 Fourth, T5 Fifth)> first, IParser<T6> second)
        => first
        .ThenAdd<(T1, T2, T3, T4, T5), T6>(second)
        .Transform(static (x, y) => (x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, y));

    /// <summary>
    /// Creates a parser that applies two parsers and combines the results.
    /// </summary>
    /// <typeparam name="T1">The first result type of the first parser.</typeparam>
    /// <typeparam name="T2">The second result type of the first parser.</typeparam>
    /// <typeparam name="T3">The third result type of the first parser.</typeparam>
    /// <typeparam name="T4">The fourth result type of the first parser.</typeparam>
    /// <typeparam name="T5">The fifth result type of the first parser.</typeparam>
    /// <typeparam name="T6">The sixth result type of the first parser.</typeparam>
    /// <typeparam name="T7">The result type of the second parser.</typeparam>
    /// <param name="first">The first parser.</param>
    /// <param name="second">The second parser.</param>
    /// <returns>A parser combining the results of both parsers.</returns>
    public static IParser<(T1 First, T2 Second, T3 Third, T4 Fourth, T5 Fifth, T6 Sixth, T7 Seventh)> ThenAdd<T1, T2, T3, T4, T5, T6, T7>(this IParser<(T1 First, T2 Second, T3 Third, T4 Fourth, T5 Fifth, T6 Sixth)> first, IParser<T7> second)
        => first
        .ThenAdd<(T1, T2, T3, T4, T5, T6), T7>(second)
        .Transform(static (x, y) => (x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, y));

    /// <summary>
    /// Creates a parser that applies two parsers and combines the results.
    /// </summary>
    /// <typeparam name="T1">The first result type of the first parser.</typeparam>
    /// <typeparam name="T2">The second result type of the first parser.</typeparam>
    /// <typeparam name="T3">The third result type of the first parser.</typeparam>
    /// <typeparam name="T4">The fourth result type of the first parser.</typeparam>
    /// <typeparam name="T5">The fifth result type of the first parser.</typeparam>
    /// <typeparam name="T6">The sixth result type of the first parser.</typeparam>
    /// <typeparam name="T7">The seventh result type of the first parser.</typeparam>
    /// <typeparam name="T8">The result type of the second parser.</typeparam>
    /// <param name="first">The first parser.</param>
    /// <param name="second">The second parser.</param>
    /// <returns>A parser combining the results of both parsers.</returns>
    public static IParser<(T1 First, T2 Second, T3 Third, T4 Fourth, T5 Fifth, T6 Sixth, T7 Seventh, T8 Eigth)> ThenAdd<T1, T2, T3, T4, T5, T6, T7, T8>(this IParser<(T1 First, T2 Second, T3 Third, T4 Fourth, T5 Fifth, T6 Sixth, T7 Seventh)> first, IParser<T8> second)
        => first
        .ThenAdd<(T1, T2, T3, T4, T5, T6, T7), T8>(second)
        .Transform(static (x, y) => (x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, y));

    /// <summary>
    /// Creates a parser that applies two parsers and returns the result of the second one.
    /// </summary>
    /// <typeparam name="T1">The result type of the first parser.</typeparam>
    /// <typeparam name="T2">The result type of the second parser.</typeparam>
    /// <param name="first">The first parser.</param>
    /// <param name="second">The second parser.</param>
    /// <returns>A parser returning the result of the second parser.</returns>
    public static IParser<T2> Then<T1, T2>(this IParser<T1> first, IParser<T2> second)
        => first.ThenAdd(second).Transform(static (_, r) => r);

    /// <summary>
    /// Creates a parser that applies two parsers and returns the result of the first one.
    /// </summary>
    /// <typeparam name="T1">The result type of the first parser.</typeparam>
    /// <typeparam name="T2">The result type of the second parser.</typeparam>
    /// <param name="first">The first parser.</param>
    /// <param name="second">The second parser.</param>
    /// <returns>A parser returning the result of the first parser.</returns>
    public static IParser<T1> ThenSkip<T1, T2>(this IParser<T1> first, IParser<T2> second)
        => first.ThenAdd(second).Transform(static (l, _) => l);

    /// <summary>
    /// Creates a parser that applies the given parser and then expects the input stream to end.
    /// </summary>
    /// <typeparam name="T">The result type of the given parser.</typeparam>
    /// <param name="parser">The given parser.</param>
    /// <returns>A parser applying the given parser and then expects the input stream to end.</returns>
    public static IParser<T> ThenEnd<T>(this IParser<T> parser)
        => parser.ThenSkip(End);

    /// <summary>
    /// Creates a parser that always fails.
    /// </summary>
    /// <typeparam name="T">The return type of the parser.</typeparam>
    /// <returns>A parser that always fails.</returns>
    public static IParser<T> Fail<T>()
        => FailParser<T>.Instance;

    /// <summary>
    /// Creates a parser that always passes and creates an object.
    /// </summary>
    /// <typeparam name="T">The type of the parser result.</typeparam>
    /// <param name="value">The value to always return from the parser.</param>
    /// <returns>A parser always returning the object.</returns>
    public static IParser<T> Create<T>(T value)
        => new CreateParser<T>(value);

    public static IParser<TAccumulator> Aggregate<TSource, TDelimiter, TAccumulator>(
        IParser<TSource> element,
        IParser<TDelimiter>? delimiter,
        int minCount,
        int maxCount,
        Func<TAccumulator> createSeed,
        Func<TAccumulator, TSource, TAccumulator> accumulate)
        => new AggregateParser<TSource, TDelimiter, TAccumulator>(
            element,
            delimiter,
            minCount.MustBeGreaterThanOrEqualTo(0),
            maxCount.MustBeGreaterThanOrEqualTo(minCount),
            createSeed,
            accumulate);

    public static IParser<TAccumulator> Aggregate<TSource, TDelimiter, TAccumulator>(
        IParser<TSource> element,
        IParser<TDelimiter>? delimiter,
        int count,
        Func<TAccumulator> createSeed,
        Func<TAccumulator, TSource, TAccumulator> accumulate)
        => Aggregate(
            element: element,
            delimiter: delimiter,
            minCount: count,
            maxCount: count,
            createSeed: createSeed,
            accumulate: accumulate);

    public static IParser<TAccumulator> Aggregate<TSource, TAccumulator>(
        IParser<TSource> element,
        int minCount,
        int maxCount,
        Func<TAccumulator> createSeed,
        Func<TAccumulator, TSource, TAccumulator> accumulate)
        => Aggregate<TSource, object?, TAccumulator>(
            element: element,
            delimiter: null,
            minCount: minCount,
            maxCount: maxCount,
            createSeed: createSeed,
            accumulate: accumulate);

    public static IParser<TAccumulator> Aggregate<TSource, TAccumulator>(
        IParser<TSource> element,
        int count,
        Func<TAccumulator> createSeed,
        Func<TAccumulator, TSource, TAccumulator> accumulate)
        => Aggregate(
            element: element,
            minCount: count,
            maxCount: count,
            createSeed: createSeed,
            accumulate: accumulate);

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="TSource">The type of results collected.</typeparam>
    /// <param name="element">The parser to apply multiple times.</param>
    /// <param name="count">The exact number of matches.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<TSource>> Multiple<TSource>(
        IParser<TSource> element,
        int count)
        => Multiple(
            element: element,
            minCount: count.MustBeGreaterThanOrEqualTo(0),
            maxCount: count);

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="TSource">The type of results collected.</typeparam>
    /// <typeparam name="TDelimiter">The type of delimiters.</typeparam>
    /// <param name="element">The parser to apply multiple times.</param>
    /// <param name="delimiter">The delimiter seperating the different elements.</param>
    /// <param name="count">The exact number of matches.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<TSource>> Multiple<TSource, TDelimiter>(
        IParser<TSource> element,
        IParser<TDelimiter>? delimiter,
        int count)
        => Multiple(
            element: element,
            delimiter: delimiter,
            minCount: count.MustBeGreaterThanOrEqualTo(0),
            maxCount: count);

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="TSource">The type of results collected.</typeparam>
    /// <param name="element">The parser to apply multiple times.</param>
    /// <param name="minCount">The minimum number of matches.</param>
    /// <param name="maxCount">The maximum number of matches.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<TSource>> Multiple<TSource>(
        IParser<TSource> element,
        int minCount,
        int maxCount)
        => Aggregate<TSource, IImmutableList<TSource>>(
            element: element,
            minCount: minCount.MustBeGreaterThanOrEqualTo(0),
            maxCount: maxCount.MustBeGreaterThanOrEqualTo(minCount),
            createSeed: static () => ImmutableList<TSource>.Empty,
            accumulate: static (acc, el) => acc.Add(el));

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="TSource">The type of results collected.</typeparam>
    /// <typeparam name="TDelimiter">The type of delimiters.</typeparam>
    /// <param name="element">The parser to apply multiple times.</param>
    /// <param name="delimiter">The delimiter seperating the different elements.</param>
    /// <param name="minCount">The minimum number of matches.</param>
    /// <param name="maxCount">The maximum number of matches.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<TSource>> Multiple<TSource, TDelimiter>(
        IParser<TSource> element,
        IParser<TDelimiter>? delimiter,
        int minCount,
        int maxCount)
        => Aggregate<TSource, TDelimiter, IImmutableList<TSource>>(
            element: element,
            delimiter: delimiter,
            minCount: minCount.MustBeGreaterThanOrEqualTo(0),
            maxCount: maxCount.MustBeGreaterThanOrEqualTo(minCount),
            createSeed: static () => ImmutableList<TSource>.Empty,
            accumulate: static (acc, el) => acc.Add(el));

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="TSource">The type of results collected.</typeparam>
    /// <param name="element">The parser to apply multiple times.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<TSource>> Many<TSource>(
        IParser<TSource> element)
        => Multiple(
            element: element,
            minCount: 0,
            maxCount: int.MaxValue);

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="TSource">The type of results collected.</typeparam>
    /// <typeparam name="TDelimiter">The type of delimiters.</typeparam>
    /// <param name="element">The parser to apply multiple times.</param>
    /// <param name="delimiter">The delimiter seperating the different elements.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<TSource>> Many<TSource, TDelimiter>(
        IParser<TSource> element,
        IParser<TDelimiter>? delimiter)
        => Multiple(
            element: element,
            delimiter: delimiter,
            minCount: 0,
            maxCount: int.MaxValue);

    /// <summary>
    /// Creates a parser which applies the given parser at least once and collects all results.
    /// </summary>
    /// <typeparam name="TSource">The result type of the parser.</typeparam>
    /// <param name="element">The given parser.</param>
    /// <returns>A parser applying the given parser at least once and collecting all results.</returns>
    public static IParser<IImmutableList<TSource>> OneOrMore<TSource>(
        IParser<TSource> element)
        => Multiple(
            element: element,
            minCount: 1,
            maxCount: int.MaxValue);

    /// <summary>
    /// Creates a parser which applies the given parser at least once and collects all results.
    /// </summary>
    /// <typeparam name="TSource">The type of results collected.</typeparam>
    /// <typeparam name="TDelimiter">The type of delimiters.</typeparam>
    /// <param name="element">The parser to apply multiple times.</param>
    /// <param name="delimiter">The delimiter seperating the different elements.</param>
    /// <returns>A parser applying the given parser at least once and collecting all results.</returns>
    public static IParser<IImmutableList<TSource>> OneOrMore<TSource, TDelimiter>(
        IParser<TSource> element,
        IParser<TDelimiter>? delimiter)
        => Multiple(
            element: element,
            delimiter: delimiter,
            minCount: 1,
            maxCount: int.MaxValue);

    /// <summary>
    /// Creates a parser that applies the given parser but does not consume the input.
    /// </summary>
    /// <typeparam name="T">The result type of the given parser.</typeparam>
    /// <param name="parser">The given parser.</param>
    /// <returns>A parser applying the given parser that does not consume the input.</returns>
    public static IParser<T> Peek<T>(IParser<T> parser)
        => new PositiveLookaheadParser<T>(parser);

    /// <summary>
    /// Creates a parser that fails if the specified parser succeeds.
    /// </summary>
    /// <typeparam name="T">The result type of the parser that should fail.</typeparam>
    /// <param name="not">The parser which, if it succeeds, causes the returned parser to fail.</param>
    /// <returns>A parser trying the given parser, and failing if it succeeds.</returns>
    public static IParser<T?> Not<T>(IParser<T> not)
        => new NegativeLookaheadParser<T>(not.MustNotBeNull());
}
