using System;
using System.Diagnostics.CodeAnalysis;

namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// Parser which first applies a wrapped parser before applying a transformation on the result.
    /// </summary>
    /// <typeparam name="TInput">The type of the input.</typeparam>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    /// <seealso cref="Parser{TOutput}" />
    internal class TransformParser<TInput, TOutput> : Parser<TOutput>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformParser{TInput, TOutput}"/> class.
        /// </summary>
        /// <param name="parser">The wrapped parser.</param>
        /// <param name="transformation">The transformation applied to the result of the wrapped parser.</param>
        internal TransformParser(IParser<TInput> parser, Func<TInput, TOutput> transformation)
        {
            Parser = parser;
            Transformation = transformation;
        }

        /// <summary>
        /// Gets the wrapped parser.
        /// </summary>
        internal IParser<TInput> Parser { get; }

        /// <summary>
        /// Gets the transformation applied to the result of the wrapped parser.
        /// </summary>
        internal Func<TInput, TOutput> Transformation { get; }

        /// <inheritdoc/>
        [SuppressMessage("Microsoft.Design", "CA1031", Justification = "General exception catch needed for correct behaviour.")]
        public override IParseResult<TOutput> TryParse(string input, int position)
        {
            IParseResult<TInput> result = Parser.TryParse(input, position);
            if (result.Success)
            {
                try
                {
                    TOutput value = Transformation(result.Value!);
                    if (value is IParsed parsed && parsed.Position == default)
                    {
                        parsed.Position = new SourcePosition(input, position, result.Position.End - 1);
                    }

                    return new ParseResult<TOutput>(this, value, input, position, result.Position.End, new[] { result });
                }
                catch (Exception e)
                {
                    return new ParseResult<TOutput>(this, input, position, result.Position.End, new TransformationError(new SourcePosition(input, position, result.Position.End - 1), e), new[] { result });
                }
            }

            return new ParseResult<TOutput>(this, input, position, result.Position.End, result.Error, new[] { result });
        }

        /// <inheritdoc/>
        public override string ToString(int depth)
        {
            if (depth < 0)
            {
                return "...";
            }

            return $"Transform({Parser.ToString(depth - 1)})";
        }
    }
}