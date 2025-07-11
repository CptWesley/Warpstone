//HintName: Warpstone.Sources.embedded.Internal.ParserExpressions.NegativeLookaheadParser.cs
// <auto-generated/>
#pragma warning disable
#nullable enable annotations

using System.Collections.Generic;
using Warpstone.Internal.ParserImplementations;

namespace Warpstone.Internal.ParserExpressions
{
    /// <summary>
    /// Parser which represents a negative lookahead (not). Which does not consume any length of the input.
    /// </summary>
    /// <typeparam name="T">The type of the parser used to peek forward.</typeparam>
    internal sealed class NegativeLookaheadParser<T> : ParserBase<T?>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NegativeLookaheadParser{T}"/> class.
        /// </summary>
        /// <param name="parser">The parser used to peek forward.</param>
        public NegativeLookaheadParser(IParser<T> parser)
        {
            Parser = parser;
        }

        /// <summary>
        /// The parser used to peek forward.
        /// </summary>
        public IParser<T> Parser { get; }

        /// <inheritdoc />
        public override IParserImplementation<T?> CreateUninitializedImplementation()
            => new NegativeLookaheadParserImpl<T>();

        /// <inheritdoc />
        protected override void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
        {
            Parser.PerformAnalysisStep(info, trace);
        }
    }
}

