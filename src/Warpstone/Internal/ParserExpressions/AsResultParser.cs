using System;
using System.Collections.Generic;
using Warpstone.Internal.ParserImplementations;
using Warpstone.Internal.SourceGeneration;

namespace Warpstone.Internal.ParserExpressions
{
    /// <summary>
    /// Represents a parser that unwraps the result as a <see cref="IParseResult{T}"/> during the parsing.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    internal sealed class AsResultParser<T> : ParserBase<IParseResult<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsResultParser{T}"/> class.
        /// </summary>
        /// <param name="parser">The internal parser.</param>
        public AsResultParser(IParser<T> parser)
        {
            Parser = parser;
        }

        /// <summary>
        /// The internal parser.
        /// </summary>
        public IParser<T> Parser { get; }

        /// <inheritdoc />
        public override IParserImplementation<IParseResult<T>> CreateUninitializedImplementation()
            => new AsResultParserImpl<T>();

        /// <inheritdoc />
        public override void EmitSource(SourceEmitter emitter)
        {
            throw emitter.Unsupported("it returns a Warpstone parse result");
        }

        /// <inheritdoc />
        protected override void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
        {
            Parser.PerformAnalysisStep(info, trace);
        }

        /// <inheritdoc />
        public override bool AnalyzeLeftEdge(Func<IParser, bool> isNullable, ICollection<IParser> leftEdge)
        {
            leftEdge.Add(Parser);
            return true;
        }
    }
}
