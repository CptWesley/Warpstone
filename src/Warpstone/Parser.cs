using System;
using System.Collections.Generic;

namespace Warpstone
{
    /// <summary>
    /// Parser class for parsing textual input.
    /// </summary>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    public abstract class Parser<TOutput>
    {
        public ParseResult<TOutput> Parse(string input)
            => Parse(input, 0);

        internal abstract ParseResult<TOutput> Parse(string input, int position);
    }

    public static class Parser
    {
        public static Parser<char> Char(char c)
            => new CharParser(c);

        public static Parser<IEnumerable<T>> Many<T>(Parser<T> parser)
            => new ManyParser<T>(parser);

        public static Parser<T> Or<T>(Parser<T> first, Parser<T> second)
            => new OrParser<T>(first, second);

        public static Parser<TOutput> Transform<TInput, TOutput>(this Parser<TInput> parser, Func<TInput, TOutput> transformation)
            => new TransformParser<TInput, TOutput>(parser, transformation);
    }

    internal class CharParser : Parser<char>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharParser"/> class.
        /// </summary>
        /// <param name="c">The character expecting in the parser.</param>
        internal CharParser(char c)
            => Character = c;

        /// <summary>
        /// Gets the expected character.
        /// </summary>
        internal char Character { get; }

        /// <inheritdoc/>
        internal override ParseResult<char> Parse(string input, int position)
        {
            if (position < input.Length && input[position] == Character)
            {
                return new ParseResult<char>(input[position], position + 1);
            }

            return new ParseResult<char>();
        }
    }

    internal class ManyParser<T> : Parser<IEnumerable<T>>
    {
        internal ManyParser(Parser<T> parser)
            => Parser = parser;

        internal Parser<T> Parser { get; }

        /// <inheritdoc/>
        internal override ParseResult<IEnumerable<T>> Parse(string input, int position)
        {
            List<T> elements = new List<T>();
            int newPosition = position;
            ParseResult<T> result;
            while (true)
            {
                result = Parser.Parse(input, newPosition);
                if (!result.Success)
                {
                    break;
                }

                newPosition = result.Position;
                elements.Add(result.Value);
            }

            return new ParseResult<IEnumerable<T>>(elements, newPosition + 1);
        }
    }

    internal class OrParser<T> : Parser<T>
    {
        internal OrParser(Parser<T> first, Parser<T> second)
        {
            First = first;
            Second = second;
        }

        internal Parser<T> First { get; }
        internal Parser<T> Second { get; }

        internal override ParseResult<T> Parse(string input, int position)
        {
            ParseResult<T> firstResult = First.Parse(input, position);
            if (firstResult.Success)
            {
                return firstResult;
            }

            return Second.Parse(input, position);
        }
    }

    internal class TransformParser<TInput, TOutput> : Parser<TOutput>
    {
        internal TransformParser(Parser<TInput> parser, Func<TInput, TOutput> transformation)
        {
            Parser = parser;
            Transformation = transformation;
        }

        internal Parser<TInput> Parser { get; }

        internal Func<TInput, TOutput> Transformation { get; }

        internal override ParseResult<TOutput> Parse(string input, int position)
        {
            ParseResult<TInput> result = Parser.Parse(input, position);
            if (result.Success)
            {
                return new ParseResult<TOutput>(Transformation(result.Value), result.Position);
            }

            return new ParseResult<TOutput>();
        }
    }
}
