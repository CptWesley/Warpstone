namespace Warpstone
{
    /// <summary>
    /// Provides access to a series of basic parsers.
    /// </summary>
    public static class Parsers
    {
        /// <summary>
        /// A parser matching the end of an input stream.
        /// </summary>
        public static IParser<string> End { get; } = EndParser.Instance;

        /// <summary>
        /// Creates a parser parsing the given character.
        /// </summary>
        /// <param name="value">The character to parse.</param>
        /// <returns>A parser parsing the given character.</returns>
        public static IParser<char> Char(char value)
            => new CharacterParser(value);

        /// <summary>
        /// Creates a parser that parses a string, configuring the used <paramref name="culture"/> and compare <paramref name="options"/>.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <param name="culture">The used culture.</param>
        /// <param name="options">The used compare options.</param>
        /// <returns>A parser parsing a string.</returns>
        public static IParser<string> String(string value, CultureInfo? culture, CompareOptions options)
            => new StringParser(value.MustNotBeNull(), culture ?? CultureInfo.CurrentCulture, options);

        /// <summary>
        /// Creates a parser that parses a string, configuring the case-sensitivity and used <paramref name="culture"/>.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <param name="ignoreCase">Indicates if the matching should be case-insensitive.</param>
        /// <param name="culture">The used culture.</param>
        /// <returns>A parser parsing a string.</returns>
        public static IParser<string> String(string value, bool ignoreCase, CultureInfo? culture)
            => String(value: value.MustNotBeNull(), culture: culture, options: ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);

        /// <summary>
        /// Creates a parser that parses a string, using the specified string comparison method.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <param name="comparisonType">The string comparison method to use.</param>
        /// <returns>A parser parsing a string.</returns>
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

            return String(value: value.MustNotBeNull(), culture: culture, options: options);
        }

        /// <summary>
        /// Creates a parser that parses a string, configuring the case-sensitivity.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <param name="ignoreCase">Indicates if the matching should be case-insensitive.</param>
        /// <returns>A parser parsing a string.</returns>
        public static IParser<string> String(string value, bool ignoreCase)
            => String(value: value.MustNotBeNull(), ignoreCase: ignoreCase, culture: null);

        /// <summary>
        /// Creates a parser that parses a string.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <returns>A parser parsing a string.</returns>
        public static IParser<string> String(string value)
            => String(value: value.MustNotBeNull(), ignoreCase: false);

        /// <summary>
        /// Creates a parser which matches a regular expression <paramref name="pattern"/> with the given <paramref name="options"/>.
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">The regular expression options to use.</param>
        /// <returns>A parser matching a regular expression.</returns>
        public static IParser<string> Regex([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, RegexOptions options)
            => new RegexParser(pattern.MustNotBeNull(), options);

        /// <summary>
        /// Creates a parser which matches a regular expression <paramref name="pattern"/>.
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <returns>A parser matching a regular expression.</returns>
        public static IParser<string> Regex([StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
            => new RegexParser(pattern.MustNotBeNull());

        /// <summary>
        /// Creates a parser that lazily applies a given parser allowing for recursion.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The given parser.</param>
        /// <returns>A parser that lazily applies a given parser.</returns>
        public static IParser<T> Lazy<T>(Func<IParser<T>> parser)
            => new LazyParser<T>(parser.MustNotBeNull());

        /// <summary>
        /// Creates a parser that tries to apply the given parsers in order and returns the result of the first successful one.
        /// </summary>
        /// <typeparam name="T">The type of results of the given parsers.</typeparam>
        /// <param name="parsers">The parsers to try.</param>
        /// <returns>A parser trying multiple parsers in order and returning the result of the first successful one.</returns>
        public static IParser<T> Or<T>(params IParser<T>[]? parsers)
            => Or((IEnumerable<IParser<T>>?)parsers);

        /// <summary>
        /// Creates a parser that tries to apply the given parsers in order and returns the result of the first successful one.
        /// </summary>
        /// <typeparam name="T">The type of results of the given parsers.</typeparam>
        /// <param name="parsers">The parsers to try.</param>
        /// <returns>A parser trying multiple parsers in order and returning the result of the first successful one.</returns>
        public static IParser<T> Or<T>(IEnumerable<IParser<T>>? parsers)
        {
            var result = Fail<T>();

            if (parsers is null)
            {
                return result;
            }

            var hasFirst = false;

            foreach (var parser in parsers)
            {
                if (!hasFirst)
                {
                    result = parser;
                    hasFirst = true;
                }
                else
                {
                    result = new OrParser<T>(result, parser);
                }
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
            => new MapParser<TInput, TOutput>(parser.MustNotBeNull(), transformation.MustNotBeNull());

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
            => parser.MustNotBeNull().Transform(x => transformation(x.First, x.Second));

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
            => parser.MustNotBeNull().Transform(x => transformation(x.First, x.Second, x.Third));

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
            => parser.MustNotBeNull().Transform(x => transformation(x.First, x.Second, x.Third, x.Fourth));

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
            => parser.MustNotBeNull().Transform(x => transformation(x.First, x.Second, x.Third, x.Fourth, x.Fifth));

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
            => parser.MustNotBeNull().Transform(x => transformation(x.First, x.Second, x.Third, x.Fourth, x.Fifth, x.Sixth));

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
            => parser.MustNotBeNull().Transform(x => transformation(x.First, x.Second, x.Third, x.Fourth, x.Fifth, x.Sixth, x.Seventh));

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
            => parser.MustNotBeNull().Transform(x => transformation(x.First, x.Second, x.Third, x.Fourth, x.Fifth, x.Sixth, x.Seventh, x.Eigth));

        /// <summary>
        /// Creates a parser that applies two parsers and combines the results.
        /// </summary>
        /// <typeparam name="T1">The result type of the first parser.</typeparam>
        /// <typeparam name="T2">The result type of the second parser.</typeparam>
        /// <param name="first">The first parser.</param>
        /// <param name="second">The second parser.</param>
        /// <returns>A parser combining the results of both parsers.</returns>
        public static IParser<(T1 Left, T2 Right)> ThenAdd<T1, T2>(this IParser<T1> first, IParser<T2> second)
            => new AndParser<T1, T2>(first.MustNotBeNull(), second.MustNotBeNull());

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
            => first.MustNotBeNull()
            .ThenAdd<(T1, T2), T3>(second.MustNotBeNull())
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
            => first.MustNotBeNull()
            .ThenAdd<(T1, T2, T3), T4>(second.MustNotBeNull())
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
            => first.MustNotBeNull()
            .ThenAdd<(T1, T2, T3, T4), T5>(second.MustNotBeNull())
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
            => first.MustNotBeNull()
            .ThenAdd<(T1, T2, T3, T4, T5), T6>(second.MustNotBeNull())
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
            => first.MustNotBeNull()
            .ThenAdd<(T1, T2, T3, T4, T5, T6), T7>(second.MustNotBeNull())
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
            => first.MustNotBeNull()
            .ThenAdd<(T1, T2, T3, T4, T5, T6, T7), T8>(second.MustNotBeNull())
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
            => first.MustNotBeNull()
            .ThenAdd(second.MustNotBeNull())
            .Transform(static (_, r) => r);

        /// <summary>
        /// Creates a parser that applies two parsers and returns the result of the first one.
        /// </summary>
        /// <typeparam name="T1">The result type of the first parser.</typeparam>
        /// <typeparam name="T2">The result type of the second parser.</typeparam>
        /// <param name="first">The first parser.</param>
        /// <param name="second">The second parser.</param>
        /// <returns>A parser returning the result of the first parser.</returns>
        public static IParser<T1> ThenSkip<T1, T2>(this IParser<T1> first, IParser<T2> second)
            => first.MustNotBeNull()
            .ThenAdd(second.MustNotBeNull())
            .Transform(static (l, _) => l);

        /// <summary>
        /// Creates a parser that applies the given parser and then expects the input stream to end.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The given parser.</param>
        /// <returns>A parser applying the given parser and then expects the input stream to end.</returns>
        public static IParser<T> ThenEnd<T>(this IParser<T> parser)
            => parser.MustNotBeNull().ThenSkip(End);

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

        /// <summary>
        /// Creates a parser which aggregates a series with minimum length <paramref name="minCount"/>
        /// and maximum length <paramref name="maxCount"/> of <paramref name="element"/> results,
        /// optionally delimited by <paramref name="delimiter"/>.
        /// Using the <paramref name="createSeed"/> function to create the seed accumulator and
        /// the <paramref name="accumulate"/> function to accumulate the found values.
        /// </summary>
        /// <typeparam name="TSource">The type of values parsed by the <paramref name="element"/> parser.</typeparam>
        /// <typeparam name="TAccumulator">The type of the accumulator.</typeparam>
        /// <param name="element">The element parser.</param>
        /// <param name="delimiter">The optional delimiter parser.</param>
        /// <param name="minCount">The minimum count.</param>
        /// <param name="maxCount">The maximum count.</param>
        /// <param name="createSeed">The function to create the initial value of the accumulator.</param>
        /// <param name="accumulate">The function used to accumulate the found values.</param>
        /// <returns>The newly created parser.</returns>
        public static IParser<TAccumulator> Aggregate<TSource, TAccumulator>(
            IParser<TSource> element,
            IParser? delimiter,
            int minCount,
            int maxCount,
            Func<TAccumulator> createSeed,
            Func<TAccumulator, TSource, TAccumulator> accumulate)
            => new AggregateParser<TSource, TAccumulator>(
                element.MustNotBeNull(),
                delimiter,
                minCount.MustBeGreaterThanOrEqualTo(0),
                maxCount.MustBeGreaterThanOrEqualTo(minCount),
                createSeed.MustNotBeNull(),
                accumulate.MustNotBeNull());

        /// <summary>
        /// Creates a parser which aggregates a series with the exact length <paramref name="count"/>
        /// of <paramref name="element"/> results,
        /// optionally delimited by <paramref name="delimiter"/>.
        /// Using the <paramref name="createSeed"/> function to create the seed accumulator and
        /// the <paramref name="accumulate"/> function to accumulate the found values.
        /// </summary>
        /// <typeparam name="TSource">The type of values parsed by the <paramref name="element"/> parser.</typeparam>
        /// <typeparam name="TAccumulator">The type of the accumulator.</typeparam>
        /// <param name="element">The element parser.</param>
        /// <param name="delimiter">The optionaldelimiter parser.</param>
        /// <param name="count">The exact count.</param>
        /// <param name="createSeed">The function to create the initial value of the accumulator.</param>
        /// <param name="accumulate">The function used to accumulate the found values.</param>
        /// <returns>The newly created parser.</returns>
        public static IParser<TAccumulator> Aggregate<TSource, TAccumulator>(
            IParser<TSource> element,
            IParser? delimiter,
            int count,
            Func<TAccumulator> createSeed,
            Func<TAccumulator, TSource, TAccumulator> accumulate)
            => Aggregate(
                element: element.MustNotBeNull(),
                delimiter: delimiter,
                minCount: count.MustBeGreaterThanOrEqualTo(0),
                maxCount: count,
                createSeed: createSeed.MustNotBeNull(),
                accumulate: accumulate.MustNotBeNull());

        /// <summary>
        /// Creates a parser which aggregates a series with minimum length <paramref name="minCount"/>
        /// and maximum length <paramref name="maxCount"/> of <paramref name="element"/> results.
        /// Using the <paramref name="createSeed"/> function to create the seed accumulator and
        /// the <paramref name="accumulate"/> function to accumulate the found values.
        /// </summary>
        /// <typeparam name="TSource">The type of values parsed by the <paramref name="element"/> parser.</typeparam>
        /// <typeparam name="TAccumulator">The type of the accumulator.</typeparam>
        /// <param name="element">The element parser.</param>
        /// <param name="minCount">The minimum count.</param>
        /// <param name="maxCount">The maximum count.</param>
        /// <param name="createSeed">The function to create the initial value of the accumulator.</param>
        /// <param name="accumulate">The function used to accumulate the found values.</param>
        /// <returns>The newly created parser.</returns>
        public static IParser<TAccumulator> Aggregate<TSource, TAccumulator>(
            IParser<TSource> element,
            int minCount,
            int maxCount,
            Func<TAccumulator> createSeed,
            Func<TAccumulator, TSource, TAccumulator> accumulate)
            => Aggregate(
                element: element.MustNotBeNull(),
                delimiter: null,
                minCount: minCount.MustBeGreaterThanOrEqualTo(0),
                maxCount: maxCount.MustBeGreaterThanOrEqualTo(minCount),
                createSeed: createSeed.MustNotBeNull(),
                accumulate: accumulate.MustNotBeNull());

        /// <summary>
        /// Creates a parser which aggregates a series with the exact length <paramref name="count"/>
        /// of <paramref name="element"/> results.
        /// Using the <paramref name="createSeed"/> function to create the seed accumulator and
        /// the <paramref name="accumulate"/> function to accumulate the found values.
        /// </summary>
        /// <typeparam name="TSource">The type of values parsed by the <paramref name="element"/> parser.</typeparam>
        /// <typeparam name="TAccumulator">The type of the accumulator.</typeparam>
        /// <param name="element">The element parser.</param>
        /// <param name="count">The exact count.</param>
        /// <param name="createSeed">The function to create the initial value of the accumulator.</param>
        /// <param name="accumulate">The function used to accumulate the found values.</param>
        /// <returns>The newly created parser.</returns>
        public static IParser<TAccumulator> Aggregate<TSource, TAccumulator>(
            IParser<TSource> element,
            int count,
            Func<TAccumulator> createSeed,
            Func<TAccumulator, TSource, TAccumulator> accumulate)
            => Aggregate(
                element: element.MustNotBeNull(),
                minCount: count.MustBeGreaterThanOrEqualTo(0),
                maxCount: count,
                createSeed: createSeed.MustNotBeNull(),
                accumulate: accumulate.MustNotBeNull());

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
                element: element.MustNotBeNull(),
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
                element: element.MustNotBeNull(),
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
                element: element.MustNotBeNull(),
                minCount: minCount.MustBeGreaterThanOrEqualTo(0),
                maxCount: maxCount.MustBeGreaterThanOrEqualTo(minCount),
                createSeed: static () => ImmutableList<TSource>.Empty,
                accumulate: static (acc, el) => acc.Add(el));

        /// <summary>
        /// Creates a parser applying the given parser multiple times and collects all results.
        /// </summary>
        /// <typeparam name="TSource">The type of results collected.</typeparam>
        /// <param name="element">The parser to apply multiple times.</param>
        /// <param name="delimiter">The delimiter seperating the different elements.</param>
        /// <param name="minCount">The minimum number of matches.</param>
        /// <param name="maxCount">The maximum number of matches.</param>
        /// <returns>A parser applying the given parser multiple times.</returns>
        public static IParser<IImmutableList<TSource>> Multiple<TSource>(
            IParser<TSource> element,
            IParser? delimiter,
            int minCount,
            int maxCount)
            => Aggregate<TSource, IImmutableList<TSource>>(
                element: element.MustNotBeNull(),
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
                element: element.MustNotBeNull(),
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
                element: element.MustNotBeNull(),
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
                element: element.MustNotBeNull(),
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
                element: element.MustNotBeNull(),
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
            => new PositiveLookaheadParser<T>(parser.MustNotBeNull());

        /// <summary>
        /// Creates a parser that fails if the specified parser succeeds.
        /// </summary>
        /// <typeparam name="T">The result type of the parser that should fail.</typeparam>
        /// <param name="not">The parser which, if it succeeds, causes the returned parser to fail.</param>
        /// <returns>A parser trying the given parser, and failing if it succeeds.</returns>
        public static IParser<T?> Not<T>(IParser<T> not)
            => new NegativeLookaheadParser<T>(not.MustNotBeNull());

        /// <summary>
        /// Creates a parser that parses the given parser, except if the exclusion parser succeedds, in which case it fails.
        /// </summary>
        /// <typeparam name="T1">The result type of condition the parser.</typeparam>
        /// <typeparam name="T2">The result type of the nested parser.</typeparam>
        /// <param name="parser">The nested parser. This parser is executed if exclusion parser fails.</param>
        /// <param name="exclusion">The exclusion parser. If this parser succeeds, the expression fails. Otherwise, the value from the nested parser is produced.</param>
        /// <returns>A parser trying the given parser, running the nested parser if the condition fails, or failing if the condition succeeds.</returns>
        public static IParser<T2> Except<T1, T2>(this IParser<T2> parser, IParser<T1> exclusion)
            => Not(exclusion.MustNotBeNull()).Then(parser.MustNotBeNull());

        /// <summary>
        /// Creates a parser that returns its inner <see cref="IParseResult{T}"/> directly.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The given parser.</param>
        /// <returns>A parser that returns its inner <see cref="IParseResult{T}"/> directly.</returns>
        public static IParser<IParseResult<T>> AsResult<T>(this IParser<T> parser)
            => new AsResultParser<T>(parser.MustNotBeNull());

        /// <summary>
        /// Creates a parser that applies a parser and then applies a different parser depending on the result.
        /// </summary>
        /// <typeparam name="TCondition">The result type of the attempted parser.</typeparam>
        /// <typeparam name="TBranches">The result type of the branch parsers.</typeparam>
        /// <param name="if">The condition parser.</param>
        /// <param name="then">The then branch parser.</param>
        /// <param name="else">The else branch parser.</param>
        /// <returns>A parser applying a parser based on a condition.</returns>
        public static IParser<TBranches> If<TCondition, TBranches>(IParser<TCondition> @if, IParser<TBranches> then, IParser<TBranches> @else)
            => Or(
                @if.MustNotBeNull().Then(then.MustNotBeNull()),
                @else.MustNotBeNull());

        /// <summary>
        /// Creates a parser that tries to parse something, but still proceeds if it fails.
        /// </summary>
        /// <typeparam name="T">The result type of the parser.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <returns>A parser trying to apply a parser, but always proceeding.</returns>
        public static IParser<T?> Maybe<T>(IParser<T> parser)
            => Maybe(parser!, default(T));

        /// <summary>
        /// Creates a parser that tries to apply a given parser, but proceeds and returns a default value if it fails.
        /// </summary>
        /// <typeparam name="T">The result type of the parser.</typeparam>
        /// <param name="parser">The given parser.</param>
        /// <param name="defaultValue">The default value to return when the parser fails.</param>
        /// <returns>A parser applying a parser, but returning a default value if it fails.</returns>
        public static IParser<T> Maybe<T>(IParser<T> parser, T defaultValue)
            => Or(parser.MustNotBeNull(), Create(defaultValue));

        /// <summary>
        /// Creates a parser that replaces the nested expected values with a given expected name.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The given parser.</param>
        /// <param name="name">The name.</param>
        /// <returns>A parser that replaces the nested expected values with a given expected name.</returns>
        public static IParser<T> WithName<T>(this IParser<T> parser, string name)
            => new ExpectedParser<T>(parser.MustNotBeNull(), name.MustNotBeNull());

        /// <summary>
        /// Creates a parser which caches the results of the given
        /// <paramref name="parser"/> per position in the input.
        /// </summary>
        /// <typeparam name="T">The return type of the <paramref name="parser"/>.</typeparam>
        /// <param name="parser">The parser whose results to cache.</param>
        /// <returns>
        /// A parser which caches the results of the given
        /// <paramref name="parser"/> per position in the input.
        /// </returns>
        public static IParser<T> Memo<T>(IParser<T> parser)
            => new MemoParser<T>(parser.MustNotBeNull());

        /// <inheritdoc cref="Memo{T}(IParser{T})" />
        public static IParser<T> Memo<T>(Func<IParser<T>> parser)
            => Memo(Lazy(parser.MustNotBeNull()));

        /// <summary>
        /// Creates a parser that allows left-recursive execution of the <paramref name="parser"/>
        /// through a variant of memoization.
        /// </summary>
        /// <typeparam name="T">The return type of the <paramref name="parser"/>.</typeparam>
        /// <param name="parser">The parser whose results to cache.</param>
        /// <returns>
        /// A parser which caches the results of the given
        /// <paramref name="parser"/> per position in the input.
        /// </returns>
        public static IParser<T> Grow<T>(IParser<T> parser)
            => new GrowParser<T>(parser.MustNotBeNull());

        /// <inheritdoc cref="Grow{T}(IParser{T})" />
        public static IParser<T> Grow<T>(Func<IParser<T>> parser)
            => Grow(Lazy(parser.MustNotBeNull()));
    }
}
