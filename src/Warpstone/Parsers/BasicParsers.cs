using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Warpstone.Parsers.InternalParsers;

namespace Warpstone.Parsers
{
    /// <summary>
    /// Static class providing simple parsers.
    /// </summary>
    [SuppressMessage("Maintainability", "CA1506", Justification = "Not exposed to end user.")]
    public static class BasicParsers
    {
        /// <summary>
        /// A parser matching the end of an input stream.
        /// </summary>
        public static readonly IParser<object> End = new EndParser();

        /// <summary>
        /// Creates a parser that always succeeds.
        /// </summary>
        /// <typeparam name="T">The return type of the parser.</typeparam>
        /// <returns>A parser that always succeeds.</returns>
        public static IParser<T> Pass<T>()
            => new VoidParser<T>();

        /// <summary>
        /// Creates a parser that always fails.
        /// </summary>
        /// <typeparam name="T">The return type of the parser.</typeparam>
        /// <returns>A parser that always fails.</returns>
        public static IParser<T> Fail<T>()
            => new FailureParser<T>();

        /// <summary>
        /// Creates a parser which matches a regular expression.
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <returns>A parser matching a regular expression.</returns>
        public static IParser<string> Regex(string pattern)
            => new RegexParser(pattern, false);

        /// <summary>
        /// Creates a parser which matches a regular expression.
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <returns>A parser matching a regular expression.</returns>
        public static IParser<string> CompiledRegex(string pattern)
            => new RegexParser(pattern, true);

        /// <summary>
        /// Creates a parser that parses a string.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <returns>A parser parsing a string.</returns>
        public static IParser<string> String(string str)
            => new StringParser(str);

        /// <summary>
        /// Creates a parser that parses a string, using the specified string comparison method.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <param name="stringComparison">The string comparison method to use.</param>
        /// <returns>A parser parsing a string.</returns>
        public static IParser<string> String(string str, StringComparison stringComparison)
            => new StringParser(str, stringComparison);

        /// <summary>
        /// Creates a parser parsing the given character.
        /// </summary>
        /// <param name="c">The character to parse.</param>
        /// <returns>A parser parsing the given character.</returns>
        public static IParser<char> Char(char c)
            => new CharacterParser(c);

        /// <summary>
        /// Creates a parser applying the given parser multiple times and collects all results.
        /// </summary>
        /// <typeparam name="T1">The type of results collected.</typeparam>
        /// <typeparam name="T2">The type of delimiters.</typeparam>
        /// <param name="parser">The parser to apply multiple times.</param>
        /// <param name="count">The exact number of matches.</param>
        /// <param name="terminator">The terminator indicating the end of the sequence.</param>
        /// <returns>A parser applying the given parser multiple times.</returns>
        public static IParser<IList<T1>> Multiple<T1, T2>(IParser<T1> parser, int count, IParser<T2> terminator)
            => Multiple(parser, new VoidParser<T2>(), count, terminator);

        /// <summary>
        /// Creates a parser applying the given parser multiple times and collects all results.
        /// </summary>
        /// <typeparam name="T">The type of results collected.</typeparam>
        /// <param name="parser">The parser to apply multiple times.</param>
        /// <param name="count">The exact number of matches.</param>
        /// <returns>A parser applying the given parser multiple times.</returns>
        public static IParser<IList<T>> Multiple<T>(IParser<T> parser, int count)
            => Multiple(parser, count, new VoidParser<object>());

        /// <summary>
        /// Creates a parser applying the given parser multiple times and collects all results.
        /// </summary>
        /// <typeparam name="T1">The type of results collected.</typeparam>
        /// <typeparam name="T2">The type of delimiters.</typeparam>
        /// <typeparam name="T3">The type of the terminator.</typeparam>
        /// <param name="parser">The parser to apply multiple times.</param>
        /// <param name="delimiter">The delimiter seperating the different elements.</param>
        /// <param name="count">The exact number of matches.</param>
        /// <param name="terminator">The terminator indicating the end of the sequence.</param>
        /// <returns>A parser applying the given parser multiple times.</returns>
        public static IParser<IList<T1>> Multiple<T1, T2, T3>(IParser<T1> parser, IParser<T2> delimiter, int count, IParser<T3> terminator)
            => Multiple(parser, delimiter, count, count, terminator);

        /// <summary>
        /// Creates a parser applying the given parser multiple times and collects all results.
        /// </summary>
        /// <typeparam name="T1">The type of results collected.</typeparam>
        /// <typeparam name="T2">The type of delimiters.</typeparam>
        /// <param name="parser">The parser to apply multiple times.</param>
        /// <param name="delimiter">The delimiter seperating the different elements.</param>
        /// <param name="count">The exact number of matches.</param>
        /// <returns>A parser applying the given parser multiple times.</returns>
        public static IParser<IList<T1>> Multiple<T1, T2>(IParser<T1> parser, IParser<T2> delimiter, int count)
            => Multiple(parser, delimiter, count, new VoidParser<T2>());

        /// <summary>
        /// Creates a parser applying the given parser multiple times and collects all results.
        /// </summary>
        /// <typeparam name="T1">The type of results collected.</typeparam>
        /// <typeparam name="T2">The type of the terminator.</typeparam>
        /// <param name="parser">The parser to apply multiple times.</param>
        /// <param name="min">The minimum number of matches.</param>
        /// <param name="max">The maximum number of matches.</param>
        /// <param name="terminator">The terminator indicating the end of the sequence.</param>
        /// <returns>A parser applying the given parser multiple times.</returns>
        public static IParser<IList<T1>> Multiple<T1, T2>(IParser<T1> parser, int min, int max, IParser<T2> terminator)
            => Multiple(parser, new VoidParser<T2>(), min, max, terminator);

        /// <summary>
        /// Creates a parser applying the given parser multiple times and collects all results.
        /// </summary>
        /// <typeparam name="T">The type of results collected.</typeparam>
        /// <param name="parser">The parser to apply multiple times.</param>
        /// <param name="min">The minimum number of matches.</param>
        /// <param name="max">The maximum number of matches.</param>
        /// <returns>A parser applying the given parser multiple times.</returns>
        public static IParser<IList<T>> Multiple<T>(IParser<T> parser, int min, int max)
            => Multiple(parser, min, max, new VoidParser<object>());

        /// <summary>
        /// Creates a parser applying the given parser multiple times and collects all results.
        /// </summary>
        /// <typeparam name="T1">The type of results collected.</typeparam>
        /// <typeparam name="T2">The type of delimiters.</typeparam>
        /// <typeparam name="T3">The type of the terminator.</typeparam>
        /// <param name="parser">The parser to apply multiple times.</param>
        /// <param name="delimiter">The delimiter seperating the different elements.</param>
        /// <param name="min">The minimum number of matches.</param>
        /// <param name="max">The maximum number of matches.</param>
        /// <param name="terminator">The terminator indicating the end of the sequence.</param>
        /// <returns>A parser applying the given parser multiple times.</returns>
        public static IParser<IList<T1>> Multiple<T1, T2, T3>(IParser<T1> parser, IParser<T2> delimiter, int min, int max, IParser<T3> terminator)
        {
            if (min < 0)
            {
                throw new ArgumentException($"Value of '{nameof(min)}' needs to be larger than zero.", nameof(min));
            }

            if (max < 0)
            {
                throw new ArgumentException($"Value of '{nameof(max)}' needs to be larger than zero.", nameof(max));
            }

            return Multiple(parser, delimiter, (ulong)min, (ulong)max, terminator);
        }

        /// <summary>
        /// Creates a parser applying the given parser multiple times and collects all results.
        /// </summary>
        /// <typeparam name="T1">The type of results collected.</typeparam>
        /// <typeparam name="T2">The type of delimiters.</typeparam>
        /// <param name="parser">The parser to apply multiple times.</param>
        /// <param name="delimiter">The delimiter seperating the different elements.</param>
        /// <param name="min">The minimum number of matches.</param>
        /// <param name="max">The maximum number of matches.</param>
        /// <returns>A parser applying the given parser multiple times.</returns>
        public static IParser<IList<T1>> Multiple<T1, T2>(IParser<T1> parser, IParser<T2> delimiter, int min, int max)
            => Multiple(parser, delimiter, min, max, new VoidParser<T2>());

        /// <summary>
        /// Creates a parser applying the given parser multiple times and collects all results.
        /// </summary>
        /// <typeparam name="T1">The type of results collected.</typeparam>
        /// <typeparam name="T2">The type of the terminator.</typeparam>
        /// <param name="parser">The parser to apply multiple times.</param>
        /// <param name="count">The exact number of matches.</param>
        /// <param name="terminator">The terminator indicating the end of the sequence.</param>
        /// <returns>A parser applying the given parser multiple times.</returns>
        public static IParser<IList<T1>> Multiple<T1, T2>(IParser<T1> parser, ulong count, IParser<T2> terminator)
            => Multiple(parser, new VoidParser<T2>(), count, terminator);

        /// <summary>
        /// Creates a parser applying the given parser multiple times and collects all results.
        /// </summary>
        /// <typeparam name="T">The type of results collected.</typeparam>
        /// <param name="parser">The parser to apply multiple times.</param>
        /// <param name="count">The exact number of matches.</param>
        /// <returns>A parser applying the given parser multiple times.</returns>
        public static IParser<IList<T>> Multiple<T>(IParser<T> parser, ulong count)
            => Multiple(parser, count, new VoidParser<object>());

        /// <summary>
        /// Creates a parser applying the given parser multiple times and collects all results.
        /// </summary>
        /// <typeparam name="T1">The type of results collected.</typeparam>
        /// <typeparam name="T2">The type of delimiters.</typeparam>
        /// <typeparam name="T3">The type of the terminator.</typeparam>
        /// <param name="parser">The parser to apply multiple times.</param>
        /// <param name="delimiter">The delimiter seperating the different elements.</param>
        /// <param name="count">The exact number of matches.</param>
        /// <param name="terminator">The terminator indicating the end of the sequence.</param>
        /// <returns>A parser applying the given parser multiple times.</returns>
        public static IParser<IList<T1>> Multiple<T1, T2, T3>(IParser<T1> parser, IParser<T2> delimiter, ulong count, IParser<T3> terminator)
            => Multiple(parser, delimiter, count, count, terminator);

        /// <summary>
        /// Creates a parser applying the given parser multiple times and collects all results.
        /// </summary>
        /// <typeparam name="T1">The type of results collected.</typeparam>
        /// <typeparam name="T2">The type of delimiters.</typeparam>
        /// <param name="parser">The parser to apply multiple times.</param>
        /// <param name="delimiter">The delimiter seperating the different elements.</param>
        /// <param name="count">The exact number of matches.</param>
        /// <returns>A parser applying the given parser multiple times.</returns>
        public static IParser<IList<T1>> Multiple<T1, T2>(IParser<T1> parser, IParser<T2> delimiter, ulong count)
            => Multiple(parser, delimiter, count, new VoidParser<T2>());

        /// <summary>
        /// Creates a parser applying the given parser multiple times and collects all results.
        /// </summary>
        /// <typeparam name="T1">The type of results collected.</typeparam>
        /// <typeparam name="T2">The type of the terminator.</typeparam>
        /// <param name="parser">The parser to apply multiple times.</param>
        /// <param name="min">The minimum number of matches.</param>
        /// <param name="max">The maximum number of matches.</param>
        /// <param name="terminator">The terminator indicating the end of the sequence.</param>
        /// <returns>A parser applying the given parser multiple times.</returns>
        public static IParser<IList<T1>> Multiple<T1, T2>(IParser<T1> parser, ulong min, ulong max, IParser<T2> terminator)
            => Multiple(parser, new VoidParser<T2>(), min, max, terminator);

        /// <summary>
        /// Creates a parser applying the given parser multiple times and collects all results.
        /// </summary>
        /// <typeparam name="T">The type of results collected.</typeparam>
        /// <param name="parser">The parser to apply multiple times.</param>
        /// <param name="min">The minimum number of matches.</param>
        /// <param name="max">The maximum number of matches.</param>
        /// <returns>A parser applying the given parser multiple times.</returns>
        public static IParser<IList<T>> Multiple<T>(IParser<T> parser, ulong min, ulong max)
            => Multiple(parser, min, max, new VoidParser<object>());

        /// <summary>
        /// Creates a parser applying the given parser multiple times and collects all results.
        /// </summary>
        /// <typeparam name="T1">The type of results collected.</typeparam>
        /// <typeparam name="T2">The type of delimiters.</typeparam>
        /// <typeparam name="T3">The type of the terminator.</typeparam>
        /// <param name="parser">The parser to apply multiple times.</param>
        /// <param name="delimiter">The delimiter seperating the different elements.</param>
        /// <param name="min">The minimum number of matches.</param>
        /// <param name="max">The maximum number of matches.</param>
        /// <param name="terminator">The terminator indicating the end of the sequence.</param>
        /// <returns>A parser applying the given parser multiple times.</returns>
        public static IParser<IList<T1>> Multiple<T1, T2, T3>(IParser<T1> parser, IParser<T2> delimiter, ulong min, ulong max, IParser<T3> terminator)
        {
            if (max < min)
            {
                throw new ArgumentException($"Value of '{nameof(max)}' needs to be larger than or equal to value of '{nameof(min)}'.", nameof(max));
            }

            return new MultipleParser<T1, T2, T3>(parser, delimiter, terminator, min, max);
        }

        /// <summary>
        /// Creates a parser applying the given parser multiple times and collects all results.
        /// </summary>
        /// <typeparam name="T1">The type of results collected.</typeparam>
        /// <typeparam name="T2">The type of delimiters.</typeparam>
        /// <param name="parser">The parser to apply multiple times.</param>
        /// <param name="delimiter">The delimiter seperating the different elements.</param>
        /// <param name="min">The minimum number of matches.</param>
        /// <param name="max">The maximum number of matches.</param>
        /// <returns>A parser applying the given parser multiple times.</returns>
        public static IParser<IList<T1>> Multiple<T1, T2>(IParser<T1> parser, IParser<T2> delimiter, ulong min, ulong max)
            => Multiple(parser, delimiter, min, max, new VoidParser<T2>());

        /// <summary>
        /// Creates a parser applying the given parser multiple times and collects all results.
        /// </summary>
        /// <typeparam name="T">The type of results collected.</typeparam>
        /// <param name="parser">The parser to apply multiple times.</param>
        /// <returns>A parser applying the given parser multiple times.</returns>
        public static IParser<IList<T>> Many<T>(IParser<T> parser)
            => Many(parser, new VoidParser<object>());

        /// <summary>
        /// Creates a parser applying the given parser multiple times and collects all results.
        /// </summary>
        /// <typeparam name="T1">The type of results collected.</typeparam>
        /// <typeparam name="T2">The type of delimiters.</typeparam>
        /// <typeparam name="T3">The type of the terminator.</typeparam>
        /// <param name="parser">The parser to apply multiple times.</param>
        /// <param name="delimiter">The delimiter seperating the different elements.</param>
        /// <param name="terminator">The terminator indicating the end of the sequence.</param>
        /// <returns>A parser applying the given parser multiple times.</returns>
        public static IParser<IList<T1>> Many<T1, T2, T3>(IParser<T1> parser, IParser<T2> delimiter, IParser<T3> terminator)
              => Multiple(parser, delimiter, 0, ulong.MaxValue, terminator);

        /// <summary>
        /// Creates a parser applying the given parser multiple times and collects all results.
        /// </summary>
        /// <typeparam name="T1">The type of results collected.</typeparam>
        /// <typeparam name="T2">The type of delimiters.</typeparam>
        /// <param name="parser">The parser to apply multiple times.</param>
        /// <param name="delimiter">The delimiter seperating the different elements.</param>
        /// <returns>A parser applying the given parser multiple times.</returns>
        public static IParser<IList<T1>> Many<T1, T2>(IParser<T1> parser, IParser<T2> delimiter)
              => Multiple(parser, delimiter, 0, ulong.MaxValue, new VoidParser<T2>());

        /// <summary>
        /// Creates a parser which applies the given parser at least once and collects all results.
        /// </summary>
        /// <typeparam name="T">The result type of the parser.</typeparam>
        /// <param name="parser">The given parser.</param>
        /// <returns>A parser applying the given parser at least once and collecting all results.</returns>
        public static IParser<IList<T>> OneOrMore<T>(IParser<T> parser)
            => OneOrMore(parser, new VoidParser<object>());

        /// <summary>
        /// Creates a parser which applies the given parser at least once and collects all results.
        /// </summary>
        /// <typeparam name="T1">The type of results collected.</typeparam>
        /// <typeparam name="T2">The type of delimiters.</typeparam>
        /// <typeparam name="T3">The type of the terminator.</typeparam>
        /// <param name="parser">The parser to apply multiple times.</param>
        /// <param name="delimiter">The delimiter seperating the different elements.</param>
        /// <param name="terminator">The terminator indicating the end of the sequence.</param>
        /// <returns>A parser applying the given parser at least once and collecting all results.</returns>
        public static IParser<IList<T1>> OneOrMore<T1, T2, T3>(IParser<T1> parser, IParser<T2> delimiter, IParser<T3> terminator)
            => Multiple(parser, delimiter, 1, ulong.MaxValue, terminator);

        /// <summary>
        /// Creates a parser which applies the given parser at least once and collects all results.
        /// </summary>
        /// <typeparam name="T1">The type of results collected.</typeparam>
        /// <typeparam name="T2">The type of delimiters.</typeparam>
        /// <param name="parser">The parser to apply multiple times.</param>
        /// <param name="delimiter">The delimiter seperating the different elements.</param>
        /// <returns>A parser applying the given parser at least once and collecting all results.</returns>
        public static IParser<IList<T1>> OneOrMore<T1, T2>(IParser<T1> parser, IParser<T2> delimiter)
            => Multiple(parser, delimiter, 1, ulong.MaxValue, new VoidParser<T2>());

        /// <summary>
        /// Creates a parser that tries to apply the given parsers in order and returns the result of the first successful one.
        /// </summary>
        /// <typeparam name="T">The type of results of the given parsers.</typeparam>
        /// <param name="first">The first parser to try.</param>
        /// <param name="second">The second parser to try.</param>
        /// <param name="parsers">The other parsers to try.</param>
        /// <returns>A parser trying multiple parsers in order and returning the result of the first successful one.</returns>
        public static IParser<T> Or<T>(IParser<T> first, IParser<T> second, params IParser<T>[] parsers)
            => InnerOr(parsers.Prepend(second).Prepend(first));

        /// <summary>
        /// Creates a parser that fails if the specified parser succeeds.
        /// </summary>
        /// <typeparam name="T">The result type of the parser that should fail.</typeparam>
        /// <param name="not">The parser which, if it succeeds, causes the returned parser to fail.</param>
        /// <returns>A parser trying the given parser, and failing if it succeeds.</returns>
        public static IParser<T> Not<T>(IParser<T> not)
            => new NotParser<T>(not);

        /// <summary>
        /// Creates a parser that parses the given parser, except if the exclusion parser succeedds, in which case it fails.
        /// </summary>
        /// <typeparam name="T1">The result type of condition the parser.</typeparam>
        /// <typeparam name="T2">The result type of the nested parser.</typeparam>
        /// <param name="parser">The nested parser. This parser is executed if exclusion parser fails.</param>
        /// <param name="exclusion">The exclusion parser. If this parser succeeds, the expression fails. Otherwise, the value from the nested parser is produced.</param>
        /// <returns>A parser trying the given parser, running the nested parser if the condition fails, or failing if the condition succeeds.</returns>
        public static IParser<T2> Except<T1, T2>(this IParser<T2> parser, IParser<T1> exclusion)
            => Not(exclusion).Then(parser);

        /// <summary>
        /// Creates a parser that first applies the given parser and then applies a transformation on its result.
        /// </summary>
        /// <typeparam name="TInput">The result type of the given input parser.</typeparam>
        /// <typeparam name="TOutput">The result type of the transformation.</typeparam>
        /// <param name="parser">The given input parser.</param>
        /// <param name="transformation">The transformation to apply on the parser result.</param>
        /// <returns>A parser first applying the given parser and then applying a transformation on its result.</returns>
        public static IParser<TOutput> Transform<TInput, TOutput>(this IParser<TInput> parser, Func<TInput, TOutput> transformation)
            => new TransformParser<TInput, TOutput>(parser, transformation);

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
        public static IParser<(T1 First, T2 Second)> ThenAdd<T1, T2>(this IParser<T1> first, IParser<T2> second)
            => new ThenAddParser<T1, T2>(first, second);

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
            => first.ThenAdd<(T1, T2), T3>(second)
            .Transform((x, y) => (x.Item1, x.Item2, y));

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
            => first.ThenAdd<(T1, T2, T3), T4>(second)
            .Transform((x, y) => (x.Item1, x.Item2, x.Item3, y));

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
            => first.ThenAdd<(T1, T2, T3, T4), T5>(second)
            .Transform((x, y) => (x.Item1, x.Item2, x.Item3, x.Item4, y));

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
            => first.ThenAdd<(T1, T2, T3, T4, T5), T6>(second)
            .Transform((x, y) => (x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, y));

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
            => first.ThenAdd<(T1, T2, T3, T4, T5, T6), T7>(second)
            .Transform((x, y) => (x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, y));

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
            => first.ThenAdd<(T1, T2, T3, T4, T5, T6, T7), T8>(second)
            .Transform((x, y) => (x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, y));

        /// <summary>
        /// Creates a parser that applies two parsers and returns the result of the second one.
        /// </summary>
        /// <typeparam name="T1">The result type of the first parser.</typeparam>
        /// <typeparam name="T2">The result type of the second parser.</typeparam>
        /// <param name="first">The first parser.</param>
        /// <param name="second">The second parser.</param>
        /// <returns>A parser returning the result of the second parser.</returns>
        public static IParser<T2> Then<T1, T2>(this IParser<T1> first, IParser<T2> second)
            => first.ThenAdd(second).Transform((l, r) => r);

        /// <summary>
        /// Creates a parser that applies two parsers and returns the result of the first one.
        /// </summary>
        /// <typeparam name="T1">The result type of the first parser.</typeparam>
        /// <typeparam name="T2">The result type of the second parser.</typeparam>
        /// <param name="first">The first parser.</param>
        /// <param name="second">The second parser.</param>
        /// <returns>A parser returning the result of the first parser.</returns>
        public static IParser<T1> ThenSkip<T1, T2>(this IParser<T1> first, IParser<T2> second)
            => first.ThenAdd(second).Transform((l, r) => l);

        /// <summary>
        /// Creates a parser that applies the given parser but does not consume the input.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The given parser.</param>
        /// <returns>A parser applying the given parser that does not consume the input.</returns>
        public static IParser<T> Peek<T>(IParser<T> parser)
            => new PeekParser<T>(parser);

        /// <summary>
        /// Creates a parser that applies a parser and then applies a different parser depending on the result.
        /// </summary>
        /// <typeparam name="TCondition">The result type of the attempted parser.</typeparam>
        /// <typeparam name="TBranches">The result type of the branch parsers.</typeparam>
        /// <param name="conditionParser">The condition parser.</param>
        /// <param name="thenParser">The then branch parser.</param>
        /// <param name="elseParser">The else branch parser.</param>
        /// <returns>A parser applying a parser based on a condition.</returns>
        public static IParser<TBranches> If<TCondition, TBranches>(IParser<TCondition> conditionParser, IParser<TBranches> thenParser, IParser<TBranches> elseParser)
            => Or(conditionParser.Then(thenParser), elseParser);

        /// <summary>
        /// Creates a parser that tries to parse something, but still proceeds if it fails.
        /// </summary>
        /// <typeparam name="T">The result type of the parser.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <returns>A parser trying to apply a parser, but always proceeding.</returns>
        public static IParser<IOption<T>> Maybe<T>(IParser<T> parser)
            => Or(parser.Transform(x => (IOption<T>)new Some<T>(x)), Create((IOption<T>)new None<T>()));

        /// <summary>
        /// Creates a parser that tries to apply a given parser, but proceeds and returns a default value if it fails.
        /// </summary>
        /// <typeparam name="T">The result type of the parser.</typeparam>
        /// <param name="parser">The given parser.</param>
        /// <param name="defaultValue">The default value to return when the parser fails.</param>
        /// <returns>A parser applying a parser, but returning a default value if it fails.</returns>
        public static IParser<T> Maybe<T>(IParser<T> parser, T defaultValue)
            => Or(parser, Create(defaultValue));

        /// <summary>
        /// Creates a parser that always passes and creates an object.
        /// </summary>
        /// <typeparam name="T">The type of the parser result.</typeparam>
        /// <param name="value">The value to always return from the parser.</param>
        /// <returns>A parser always returning the object.</returns>
        public static IParser<T> Create<T>(T value)
            => new VoidParser<T>().Transform(x => value);

        /// <summary>
        /// Creates a parser that applies the given parser and then expects the input stream to end.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The given parser.</param>
        /// <returns>A parser applying the given parser and then expects the input stream to end.</returns>
        public static IParser<T> ThenEnd<T>(this IParser<T> parser)
            => parser.ThenSkip(End);

        /// <summary>
        /// Creates a parser that lazily applies a given parser allowing for recursion.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The given parser.</param>
        /// <returns>A parser that lazily applies a given parser.</returns>
        public static IParser<T> Lazy<T>(Func<IParser<T>> parser)
            => new LazyParser<T>(parser);

        /// <summary>
        /// Creates a parser that replaces the nested expected values with a given expected name.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The given parser.</param>
        /// <param name="name">The name.</param>
        /// <returns>A parser that replaces the nested expected values with a given expected name.</returns>
        public static IParser<T> WithName<T>(this IParser<T> parser, string name)
            => parser.WithNames(new string[] { name });

        /// <summary>
        /// Creates a parser that replaces the nested expected values with given expected names.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The given parser.</param>
        /// <param name="names">The names.</param>
        /// <returns>A parser that replaces the nested expected values with given expected names.</returns>
        public static IParser<T> WithNames<T>(this IParser<T> parser, IEnumerable<string> names)
            => new ExpectedParser<T>(parser, names);

        /// <summary>
        /// Creates a parser that replaces the nested expected values with given expected names.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The given parser.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="otherNames">The other names.</param>
        /// <returns>A parser that replaces the nested expected values with given expected names.</returns>
        public static IParser<T> WithNames<T>(this IParser<T> parser, string firstName, params string[] otherNames)
            => parser.WithNames(new string[] { firstName }.Concat(otherNames));

        private static IParser<T> InnerOr<T>(IEnumerable<IParser<T>> parsers)
        {
            if (parsers.Count() == 1)
            {
                return parsers.First();
            }

            return new OrParser<T>(parsers.First(), InnerOr(parsers.Skip(1)));
        }
    }
}
