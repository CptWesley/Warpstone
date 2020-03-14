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
        /// A parser skipping all whitespaces that are optional.
        /// </summary>
        public static readonly Parser<string> OptionalWhitespaces
            = Many(Whitespace).Transform(x => string.Join(string.Empty, x));

        /// <summary>
        /// A parser skipping all whitespaces that are mandatory.
        /// </summary>
        public static readonly Parser<string> Whitespaces
            = OneOrMore(Whitespace).Transform(x => string.Join(string.Empty, x));

        /// <summary>
        /// A parser matching any lowercase letter.
        /// </summary>
        public static readonly Parser<char> LowercaseLetter
            = Regex("[a-z]").Transform(x => x[0]);

        /// <summary>
        /// A parser matching any uppercase letter.
        /// </summary>
        public static readonly Parser<char> UppercaseLetter
            = Regex("[A-Z]").Transform(x => x[0]);

        /// <summary>
        /// A parser matching any letter.
        /// </summary>
        public static readonly Parser<char> Letter
            = Regex("[a-zA-Z]").Transform(x => x[0]);

        /// <summary>
        /// A parser matching a single digit.
        /// </summary>
        public static readonly Parser<char> Digit
            = Regex("[0-9]").Transform(x => x[0]);

        /// <summary>
        /// A parser matching a single alphanumeric character.
        /// </summary>
        public static readonly Parser<char> Alphanumeric
            = Regex("[a-zA-Z0-9]").Transform(x => x[0]);

        /// <summary>
        /// A parser matching the end of an input stream.
        /// </summary>
        public static readonly Parser<object> End = new EndParser();

        /// <summary>
        /// Creates a parser which matches a regular expression.
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <returns>A parser matching a regular expression.</returns>
        public static Parser<string> Regex(string pattern)
            => new RegexParser(pattern);

        /// <summary>
        /// Creates a parser that parses a string.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <returns>A parser parsing a string.</returns>
        public static Parser<string> String(string str)
            => Regex(System.Text.RegularExpressions.Regex.Escape(str));

        /// <summary>
        /// Creates a parser parsing the given character.
        /// </summary>
        /// <param name="c">The character to parse.</param>
        /// <returns>A parser parsing the given character.</returns>
        public static Parser<char> Char(char c)
            => String(c.ToString(CultureInfo.InvariantCulture)).Transform(x => x[0]);

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
        /// Creates a parser that first applies the given parser and then applies a transformation on its result.
        /// </summary>
        /// <typeparam name="T1">The first result type of the given input parser.</typeparam>
        /// <typeparam name="T2">The second result type of the given input parser.</typeparam>
        /// <typeparam name="TOutput">The result type of the transformation.</typeparam>
        /// <param name="parser">The given input parser.</param>
        /// <param name="transformation">The transformation to apply on the parser result.</param>
        /// <returns>A parser first applying the given parser and then applying a transformation on its result.</returns>
        public static Parser<TOutput> Transform<T1, T2, TOutput>(this Parser<(T1, T2)> parser, Func<T1, T2, TOutput> transformation)
            => parser.Transform(x => transformation(x.Item1, x.Item2));

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
        public static Parser<TOutput> Transform<T1, T2, T3, TOutput>(this Parser<(T1, T2, T3)> parser, Func<T1, T2, T3, TOutput> transformation)
            => parser.Transform(x => transformation(x.Item1, x.Item2, x.Item3));

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
        public static Parser<TOutput> Transform<T1, T2, T3, T4, TOutput>(this Parser<(T1, T2, T3, T4)> parser, Func<T1, T2, T3, T4, TOutput> transformation)
            => parser.Transform(x => transformation(x.Item1, x.Item2, x.Item3, x.Item4));

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
        public static Parser<TOutput> Transform<T1, T2, T3, T4, T5, TOutput>(this Parser<(T1, T2, T3, T4, T5)> parser, Func<T1, T2, T3, T4, T5, TOutput> transformation)
            => parser.Transform(x => transformation(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5));

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
        public static Parser<TOutput> Transform<T1, T2, T3, T4, T5, T6, TOutput>(this Parser<(T1, T2, T3, T4, T5, T6)> parser, Func<T1, T2, T3, T4, T5, T6, TOutput> transformation)
            => parser.Transform(x => transformation(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6));

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
        public static Parser<TOutput> Transform<T1, T2, T3, T4, T5, T6, T7, TOutput>(this Parser<(T1, T2, T3, T4, T5, T6, T7)> parser, Func<T1, T2, T3, T4, T5, T6, T7, TOutput> transformation)
            => parser.Transform(x => transformation(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7));

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
        public static Parser<TOutput> Transform<T1, T2, T3, T4, T5, T6, T7, T8, TOutput>(this Parser<(T1, T2, T3, T4, T5, T6, T7, T8)> parser, Func<T1, T2, T3, T4, T5, T6, T7, T8, TOutput> transformation)
            => parser.Transform(x => transformation(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7, x.Item8));

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
        public static Parser<(T1, T2, T3, T4)> ThenAdd<T1, T2, T3, T4>(this Parser<(T1, T2, T3)> first, Parser<T4> second)
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
        public static Parser<(T1, T2, T3, T4, T5)> ThenAdd<T1, T2, T3, T4, T5>(this Parser<(T1, T2, T3, T4)> first, Parser<T5> second)
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
        public static Parser<(T1, T2, T3, T4, T5, T6)> ThenAdd<T1, T2, T3, T4, T5, T6>(this Parser<(T1, T2, T3, T4, T5)> first, Parser<T6> second)
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
        public static Parser<(T1, T2, T3, T4, T5, T6, T7)> ThenAdd<T1, T2, T3, T4, T5, T6, T7>(this Parser<(T1, T2, T3, T4, T5, T6)> first, Parser<T7> second)
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
        public static Parser<(T1, T2, T3, T4, T5, T6, T7, T8)> ThenAdd<T1, T2, T3, T4, T5, T6, T7, T8>(this Parser<(T1, T2, T3, T4, T5, T6, T7)> first, Parser<T8> second)
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
        public static Parser<T2> Then<T1, T2>(this Parser<T1> first, Parser<T2> second)
            => first.ThenAdd(second).Transform((l, r) => r);

        /// <summary>
        /// Creates a parser that applies two parsers and returns the result of the first one.
        /// </summary>
        /// <typeparam name="T1">The result type of the first parser.</typeparam>
        /// <typeparam name="T2">The result type of the second parser.</typeparam>
        /// <param name="first">The first parser.</param>
        /// <param name="second">The second parser.</param>
        /// <returns>A parser returning the result of the first parser.</returns>
        public static Parser<T1> ThenSkip<T1, T2>(this Parser<T1> first, Parser<T2> second)
            => first.ThenAdd(second).Transform((l, r) => l);

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
            => Or(conditionParser.Then(thenParser), elseParser);

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

        /// <summary>
        /// Creates a parser that applies the given parser and then expects the input stream to end.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The given parser.</param>
        /// <returns>A parser applying the given parser and then expects the input stream to end.</returns>
        public static Parser<T> ThenEnd<T>(this Parser<T> parser)
            => parser.ThenSkip(End);

        /// <summary>
        /// Creates a parser that lazily applies a given parser allowing for recursion.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The given parser.</param>
        /// <returns>A parser that lazily applies a given parser.</returns>
        public static Parser<T> Lazy<T>(Func<Parser<T>> parser)
            => new LazyParser<T>(parser);

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
