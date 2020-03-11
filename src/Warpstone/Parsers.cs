using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Warpstone.InternalParsers;

namespace Warpstone
{
    /// <summary>
    /// Static class providing simple parsers.
    /// </summary>
    [SuppressMessage("Maintainability", "CA1506", Justification = "Not exposed to end user.")]
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
        /// A parser matching any lowercase letter.
        /// </summary>
        [SuppressMessage("Style", "SA1117", Justification = "Used for compacting a huge number of calls.")]
        public static readonly Parser<char> LowercaseLetter
            = Or(Char('a'), Char('b'), Char('c'), Char('d'), Char('e'),
                Char('f'), Char('g'), Char('h'), Char('i'), Char('j'),
                Char('k'), Char('l'), Char('m'), Char('n'), Char('o'),
                Char('p'), Char('q'), Char('r'), Char('s'), Char('t'),
                Char('u'), Char('v'), Char('w'), Char('x'), Char('y'),
                Char('z'));

        /// <summary>
        /// A parser matching any uppercase letter.
        /// </summary>
        [SuppressMessage("Style", "SA1117", Justification = "Used for compacting a huge number of calls.")]
        public static readonly Parser<char> UppercaseLetter
            = Or(Char('A'), Char('B'), Char('C'), Char('D'), Char('E'),
                Char('F'), Char('G'), Char('H'), Char('I'), Char('J'),
                Char('K'), Char('L'), Char('M'), Char('N'), Char('O'),
                Char('P'), Char('Q'), Char('R'), Char('S'), Char('T'),
                Char('U'), Char('V'), Char('W'), Char('X'), Char('Y'),
                Char('Z'));

        /// <summary>
        /// A parser matching any letter.
        /// </summary>
        public static readonly Parser<char> Letter = Or(LowercaseLetter, UppercaseLetter);

        /// <summary>
        /// A parser matching a single digit.
        /// </summary>
        public static readonly Parser<char> Digit
            = Or(Char('0'), Char('1'), Char('2'), Char('3'), Char('4'), Char('5'), Char('6'), Char('7'), Char('8'), Char('9'));

        /// <summary>
        /// A parser matching a single alphanumeric character.
        /// </summary>
        public static readonly Parser<char> Alphanumeric = Or(Letter, Digit);

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
            => Many(parser, new VoidParser<object>());

        /// <summary>
        /// Creates a parser applying the given parser multiple times and collects all results.
        /// </summary>
        /// <typeparam name="T1">The type of results collected.</typeparam>
        /// <typeparam name="T2">The type of delimiters.</typeparam>
        /// <param name="parser">The parser to apply multiple times.</param>
        /// <param name="delimiter">The delimiter seperating the different elements.</param>
        /// <returns>A parser applying the given parser multiple times.</returns>
        public static Parser<IEnumerable<T1>> Many<T1, T2>(Parser<T1> parser, Parser<T2> delimiter)
            => Or(OneOrMore(parser, delimiter), Create<IEnumerable<T1>>(Array.Empty<T1>()));

        /// <summary>
        /// Creates a parser which applies the given parser at least once and collects all results.
        /// </summary>
        /// <typeparam name="T">The result type of the parser.</typeparam>
        /// <param name="parser">The given parser.</param>
        /// <returns>A parser applying the given parser at least once and collecting all results.</returns>
        public static Parser<IEnumerable<T>> OneOrMore<T>(Parser<T> parser)
            => OneOrMore(parser, new VoidParser<object>());

        /// <summary>
        /// Creates a parser which applies the given parser at least once and collects all results.
        /// </summary>
        /// <typeparam name="T1">The type of results collected.</typeparam>
        /// <typeparam name="T2">The type of delimiters.</typeparam>
        /// <param name="parser">The parser to apply multiple times.</param>
        /// <param name="delimiter">The delimiter seperating the different elements.</param>
        /// <returns>A parser applying the given parser at least once and collecting all results.</returns>
        public static Parser<IEnumerable<T1>> OneOrMore<T1, T2>(Parser<T1> parser, Parser<T2> delimiter)
            => new OneOrMoreParser<T1, T2>(parser, delimiter);

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
        public static Parser<(T1, T2)> ThenAdd<T1, T2>(this Parser<T1> first, Parser<T2> second)
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
        public static Parser<(T1, T2, T3)> ThenAdd<T1, T2, T3>(this Parser<(T1, T2)> first, Parser<T3> second)
            => first.ThenAdd<(T1, T2), T3>(second)
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
        public static Parser<(T1, T2, T3, T4)> ThenAdd<T1, T2, T3, T4>(this Parser<(T1, T2, T3)> first, Parser<T4> second)
            => first.ThenAdd<(T1, T2, T3), T4>(second)
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
        public static Parser<(T1, T2, T3, T4, T5)> ThenAdd<T1, T2, T3, T4, T5>(this Parser<(T1, T2, T3, T4)> first, Parser<T5> second)
            => first.ThenAdd<(T1, T2, T3, T4), T5>(second)
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
        public static Parser<(T1, T2, T3, T4, T5, T6)> ThenAdd<T1, T2, T3, T4, T5, T6>(this Parser<(T1, T2, T3, T4, T5)> first, Parser<T6> second)
            => first.ThenAdd<(T1, T2, T3, T4, T5), T6>(second)
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
        public static Parser<(T1, T2, T3, T4, T5, T6, T7)> ThenAdd<T1, T2, T3, T4, T5, T6, T7>(this Parser<(T1, T2, T3, T4, T5, T6)> first, Parser<T7> second)
            => first.ThenAdd<(T1, T2, T3, T4, T5, T6), T7>(second)
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
        public static Parser<(T1, T2, T3, T4, T5, T6, T7, T8)> ThenAdd<T1, T2, T3, T4, T5, T6, T7, T8>(this Parser<(T1, T2, T3, T4, T5, T6, T7)> first, Parser<T8> second)
            => first.ThenAdd<(T1, T2, T3, T4, T5, T6, T7), T8>(second)
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
            => first.ThenAdd(second).Transform(x => x.Item2);

        /// <summary>
        /// Creates a parser that applies two parsers and returns the result of the first one.
        /// </summary>
        /// <typeparam name="T1">The result type of the first parser.</typeparam>
        /// <typeparam name="T2">The result type of the second parser.</typeparam>
        /// <param name="first">The first parser.</param>
        /// <param name="second">The second parser.</param>
        /// <returns>A parser returning the result of the first parser.</returns>
        public static Parser<T1> ThenSkip<T1, T2>(this Parser<T1> first, Parser<T2> second)
            => first.ThenAdd(second).Transform(x => x.Item1);

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
                parser = parser.ThenAdd(Char(str[i])).Transform(x => x.Item1 + x.Item2);
            }

            return parser;
        }

        /// <summary>
        /// Creates a parser that applies the given parser but does not consume the input.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The given parser.</param>
        /// <returns>A parser applying the given parser that does not consume the input.</returns>
        public static Parser<T> Peek<T>(Parser<T> parser)
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
        public static Parser<TBranches> If<TCondition, TBranches>(Parser<TCondition> conditionParser, Parser<TBranches> thenParser, Parser<TBranches> elseParser)
            => new ConditionalParser<TCondition, TBranches>(conditionParser, thenParser, elseParser);

        /// <summary>
        /// Creates a parser that tries to parse something, but still proceeds if it fails.
        /// </summary>
        /// <typeparam name="T">The result type of the parser.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <returns>A parser trying to apply a parser, but always proceeding.</returns>
        public static Parser<IOption<T>> Maybe<T>(Parser<T> parser)
            => Or(parser.Transform(x => (IOption<T>)new Some<T>(x)), Create((IOption<T>)new None<T>()));

        /// <summary>
        /// Creates a parser that tries to apply a given parser, but proceeds and returns a default value if it fails.
        /// </summary>
        /// <typeparam name="T">The result type of the parser.</typeparam>
        /// <param name="parser">The given parser.</param>
        /// <param name="defaultValue">The default value to return when the parser fails.</param>
        /// <returns>A parser applying a parser, but returning a default value if it fails.</returns>
        public static Parser<T> Maybe<T>(Parser<T> parser, T defaultValue)
            => Or(parser, Create(defaultValue));

        /// <summary>
        /// Creates a parser that always passes and creates an object.
        /// </summary>
        /// <typeparam name="T">The type of the parser result.</typeparam>
        /// <param name="value">The value to always return from the parser.</param>
        /// <returns>A parser always returning the object.</returns>
        public static Parser<T> Create<T>(T value)
            => new VoidParser<T>().Transform(x => value);

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
