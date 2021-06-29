﻿using System;
using System.Collections.Generic;
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
                    TOutput value = Transformation(result.Value);
                    if (value is IParsed parsed && parsed.Position == null)
                    {
                        parsed.Position = new SourcePosition(position, result.Position - 1);
                    }

                    return new ParseResult<TOutput>(value, position, result.Position);
                }
                catch (Exception e)
                {
                    return new ParseResult<TOutput>(position, result.Position, new TransformationError(new SourcePosition(position, result.Position - 1), e));
                }
            }

            return new ParseResult<TOutput>(position, result.Position, result.Error);
        }

        /// <inheritdoc/>
        public override void FillSyntaxHighlightingGraph(Dictionary<object, HighlightingNode> graph)
        {
            if (graph.ContainsKey(this))
            {
                return;
            }

            graph.Add(this, new HighlightingNode(string.Empty, Highlight.None, new object[] { Parser }));
            Parser.FillSyntaxHighlightingGraph(graph);
        }

        public override string ToRegex2(Dictionary<object, string> names)
            => Parser.ToRegex(names);
    }
}