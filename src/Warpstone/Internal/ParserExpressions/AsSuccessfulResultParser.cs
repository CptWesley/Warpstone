using System.Collections.Generic;
using Warpstone.Internal.ParserImplementations;

namespace Warpstone.Internal.ParserExpressions
{
    /// <summary>
    /// Represents a parser that unwraps the result as a <see cref="IParseResult{T}"/> during the parsing;
    /// only if the parse result was successful, otherwise the parsing fails.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    internal sealed class AsSuccessfulResultParser<T> : ParserBase<IParseResult<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsSuccessfulResultParser{T}"/> class.
        /// </summary>
        /// <param name="parser">The internal parser.</param>
        public AsSuccessfulResultParser(IParser<T> parser)
        {
            Parser = parser;
        }

        /// <summary>
        /// The internal parser.
        /// </summary>
        public IParser<T> Parser { get; }

        /// <inheritdoc />
        public override IParserImplementation<IParseResult<T>> CreateUninitializedImplementation()
            => new AsSuccessfulResultParserImpl<T>();

        /// <inheritdoc />
        protected override void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
        {
            Parser.PerformAnalysisStep(info, trace);
        }
    }
}
