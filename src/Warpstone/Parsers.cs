using System;
using System.Collections.Generic;
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
