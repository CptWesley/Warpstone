using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Warpstone.InternalParsers;

namespace Warpstone
{
    /// <summary>
    /// Static class providing simple parsers.
    /// </summary>
    public static class Parsers
    {
        /// <summary>
        /// A parser parsing a newline character.
        /// </summary>
        public static readonly Parser<string> Newline = Or(String("\r\n"), String("\n"));

        /// <summary>
        /// A parser parsing a whitespace.
        /// </summary>
        public static readonly Parser<string> Whitespace = Or(Newline, String("\t"), String(" "));

        /// <summary>
        /// Creates a parser parsing the given character.
        /// </summary>
        /// <param name="c">The character to parse.</param>
        /// <returns>A parser parsing the given character.</returns>
        public static Parser<char> Char(char c)
            => new CharParser(c);

        /// <summary>
        /// Creates a parser applying the given parser multiple times and collects all results.
        /// </summary>
        /// <typeparam name="T">The type of results collected.</typeparam>
        /// <param name="parser">The parser to apply multiple times.</param>
        /// <returns>A parser applying the given parser multiple times.</returns>
        public static Parser<IEnumerable<T>> Many<T>(Parser<T> parser)
            => new ManyParser<T>(parser);

        /// <summary>
        /// Creates a parser that tries to apply the given parsers in order and returns the result of the first successful one.
        /// </summary>
        /// <typeparam name="T">The type of results of the given parsers.</typeparam>
        /// <param name="first">The first parser to try.</param>
        /// <param name="second">The second parser to try.</param>
        /// <param name="parsers">The other parsers to try.</param>
        /// <returns>A parser trying multiple parsers in order and returning the result of the first successful one.</returns>
        public static Parser<T> Or<T>(Parser<T> first, Parser<T> second, params Parser<T>[] parsers)
            => InnerOr(parsers.Prepend(second).Prepend(first));

        /// <summary>
        /// Creates a parser that first applies the given parser and then applies a transformation on its result.
        /// </summary>
        /// <typeparam name="TInput">The result type of the given input parser.</typeparam>
        /// <typeparam name="TOutput">The result type of the transformation.</typeparam>
        /// <param name="parser">The given input parser.</param>
        /// <param name="transformation">The transformation to apply on the parser result.</param>
        /// <returns>A parser first applying the given parser and then applying a transformation on its result.</returns>
        public static Parser<TOutput> Transform<TInput, TOutput>(this Parser<TInput> parser, Func<TInput, TOutput> transformation)
            => new TransformParser<TInput, TOutput>(parser, transformation);

        /// <summary>
        /// Creates a parser that applies two parsers and combines the results.
        /// </summary>
        /// <typeparam name="T1">The result type of the first parser.</typeparam>
        /// <typeparam name="T2">The result type of the second parser.</typeparam>
        /// <param name="first">The first parser.</param>
        /// <param name="second">The second parser.</param>
        /// <returns>A parser combining the results of both parsers.</returns>
        public static Parser<(T1, T2)> AndThen<T1, T2>(this Parser<T1> first, Parser<T2> second)
            => new AndThenParser<T1, T2>(first, second);

        /// <summary>
        /// Creates a parser that applies two parsers and combines the results.
        /// </summary>
        /// <typeparam name="T1">The first result type of the first parser.</typeparam>
        /// <typeparam name="T2">The second result type of the first parser.</typeparam>
        /// <typeparam name="T3">The result type of the second parser.</typeparam>
        /// <param name="first">The first parser.</param>
        /// <param name="second">The second parser.</param>
        /// <returns>A parser combining the results of both parsers.</returns>
        public static Parser<(T1, T2, T3)> AndThen<T1, T2, T3>(this Parser<(T1, T2)> first, Parser<T3> second)
            => first.AndThen<(T1, T2), T3>(second)
            .Transform(x => (x.Item1.Item1, x.Item1.Item2, x.Item2));

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
        public static Parser<(T1, T2, T3, T4)> AndThen<T1, T2, T3, T4>(this Parser<(T1, T2, T3)> first, Parser<T4> second)
            => first.AndThen<(T1, T2, T3), T4>(second)
            .Transform(x => (x.Item1.Item1, x.Item1.Item2, x.Item1.Item3, x.Item2));

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
        public static Parser<(T1, T2, T3, T4, T5)> AndThen<T1, T2, T3, T4, T5>(this Parser<(T1, T2, T3, T4)> first, Parser<T5> second)
            => first.AndThen<(T1, T2, T3, T4), T5>(second)
            .Transform(x => (x.Item1.Item1, x.Item1.Item2, x.Item1.Item3, x.Item1.Item4, x.Item2));

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
        public static Parser<(T1, T2, T3, T4, T5, T6)> AndThen<T1, T2, T3, T4, T5, T6>(this Parser<(T1, T2, T3, T4, T5)> first, Parser<T6> second)
            => first.AndThen<(T1, T2, T3, T4, T5), T6>(second)
            .Transform(x => (x.Item1.Item1, x.Item1.Item2, x.Item1.Item3, x.Item1.Item4, x.Item1.Item5, x.Item2));

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
        public static Parser<(T1, T2, T3, T4, T5, T6, T7)> AndThen<T1, T2, T3, T4, T5, T6, T7>(this Parser<(T1, T2, T3, T4, T5, T6)> first, Parser<T7> second)
            => first.AndThen<(T1, T2, T3, T4, T5, T6), T7>(second)
            .Transform(x => (x.Item1.Item1, x.Item1.Item2, x.Item1.Item3, x.Item1.Item4, x.Item1.Item5, x.Item1.Item6, x.Item2));

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
        public static Parser<(T1, T2, T3, T4, T5, T6, T7, T8)> AndThen<T1, T2, T3, T4, T5, T6, T7, T8>(this Parser<(T1, T2, T3, T4, T5, T6, T7)> first, Parser<T8> second)
            => first.AndThen<(T1, T2, T3, T4, T5, T6, T7), T8>(second)
            .Transform(x => (x.Item1.Item1, x.Item1.Item2, x.Item1.Item3, x.Item1.Item4, x.Item1.Item5, x.Item1.Item6, x.Item1.Item7, x.Item2));

        /// <summary>
        /// Creates a parser that applies two parsers and returns the result of the second one.
        /// </summary>
        /// <typeparam name="T1">The result type of the first parser.</typeparam>
        /// <typeparam name="T2">The result type of the second parser.</typeparam>
        /// <param name="first">The first parser.</param>
        /// <param name="second">The second parser.</param>
        /// <returns>A parser returning the result of the second parser.</returns>
        public static Parser<T2> Then<T1, T2>(this Parser<T1> first, Parser<T2> second)
            => first.AndThen(second).Transform(x => x.Item2);

        /// <summary>
        /// Creates a parser that applies two parsers and returns the result of the first one.
        /// </summary>
        /// <typeparam name="T1">The result type of the first parser.</typeparam>
        /// <typeparam name="T2">The result type of the second parser.</typeparam>
        /// <param name="first">The first parser.</param>
        /// <param name="second">The second parser.</param>
        /// <returns>A parser returning the result of the first parser.</returns>
        public static Parser<T1> Skip<T1, T2>(this Parser<T1> first, Parser<T2> second)
            => first.AndThen(second).Transform(x => x.Item1);

        /// <summary>
        /// Creates a parser that parses a string.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <returns>A parser parsing a string.</returns>
        public static Parser<string> String(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (str.Length <= 0)
            {
                throw new ArgumentException("Expected string to be at least of size 1.", nameof(str));
            }

            Parser<string> parser = Char(str[0]).Transform(x => x.ToString(CultureInfo.InvariantCulture));

            for (int i = 1; i < str.Length; i++)
            {
                parser = parser.AndThen(Char(str[i])).Transform(x => x.Item1 + x.Item2);
            }

            return parser;
        }

        private static Parser<T> InnerOr<T>(IEnumerable<Parser<T>> parsers)
        {
            if (parsers.Count() == 1)
            {
                return parsers.First();
            }

            return new OrParser<T>(parsers.First(), InnerOr(parsers.Skip(1)));
        }
    }
}
