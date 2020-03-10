using System;
using System.Collections.Generic;
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
        /// Creates a parser that first tries to apply the first parser and if it fails, tries the second one.
        /// </summary>
        /// <typeparam name="T">The type of results of the given parsers.</typeparam>
        /// <param name="first">The first parser to try.</param>
        /// <param name="second">The second parser to try.</param>
        /// <returns>A Parser trying two different parsers and returning the result of the succesful one.</returns>
        public static Parser<T> Or<T>(Parser<T> first, Parser<T> second)
            => new OrParser<T>(first, second);

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
    }
}
