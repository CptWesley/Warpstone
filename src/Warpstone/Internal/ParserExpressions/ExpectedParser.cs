using System.Collections.Generic;
using Warpstone.Internal.ParserImplementations;

namespace Warpstone.Internal.ParserExpressions
{
    /// <summary>
    /// Represents a parser that can override the expected token message.
    /// </summary>
    /// <typeparam name="T">The result type of the wrapped parser.</typeparam>
    internal sealed class ExpectedParser<T> : ParserBase<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectedParser{T}"/> class.
        /// </summary>
        /// <param name="parser">The internal parser.</param>
        /// <param name="expected">The expected string.</param>
        public ExpectedParser(IParser<T> parser, string expected)
        {
            Parser = parser;
            Expected = expected;
        }

        /// <summary>
        /// The internal parser.
        /// </summary>
        public IParser<T> Parser { get; }

        /// <summary>
        /// The expected string.
        /// </summary>
        public string Expected { get; }

        /// <inheritdoc />
        public override IParserImplementation<T> CreateUninitializedImplementation()
            => new ExpectedParserImpl<T>();

        /// <inheritdoc />
        protected override void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
        {
            Parser.PerformAnalysisStep(info, trace);
        }
    }
}
