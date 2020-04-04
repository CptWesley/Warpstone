using System;

namespace Warpstone.InternalParsers
{
    /// <summary>
    /// Parser which first applies a wrapped parser before applying a transformation on the result.
    /// </summary>
    /// <typeparam name="TInput">The type of the input.</typeparam>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    /// <seealso cref="Warpstone.Parser{TOutput}" />
    internal class TransformParser<TInput, TOutput> : Parser<TOutput>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformParser{TInput, TOutput}"/> class.
        /// </summary>
        /// <param name="parser">The wrapped parser.</param>
        /// <param name="transformation">The transformation applied to the result of the wrapped parser.</param>
        internal TransformParser(Parser<TInput> parser, Func<TInput, TOutput> transformation)
        {
            Parser = parser;
            Transformation = transformation;
        }

        /// <summary>
        /// Gets the wrapped parser.
        /// </summary>
        internal Parser<TInput> Parser { get; }

        /// <summary>
        /// Gets the transformation applied to the result of the wrapped parser.
        /// </summary>
        internal Func<TInput, TOutput> Transformation { get; }

        /// <inheritdoc/>
        internal override ParseResult<TOutput> TryParse(string input, int position)
        {
            ParseResult<TInput> result = Parser.TryParse(input, position);
            if (result.Success)
            {
                TOutput value = Transformation(result.Value);
                if (value is IParsed parsed)
                {
                    parsed.Position = new SourcePosition(position, result.Position - position);
                }

                return new ParseResult<TOutput>(value, position, result.Position);
            }

            return new ParseResult<TOutput>(position, result.Position, result.Expected);
        }
    }
}