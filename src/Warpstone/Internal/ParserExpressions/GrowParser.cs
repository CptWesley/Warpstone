namespace Warpstone.Internal.ParserExpressions
{
    /// <summary>
    /// Represents a parser which caches the result for a given parser at a given position
    /// so that it can be reused later if the same parser is executed at the same position again.
    /// This variant of the <see cref="MemoParser{T}"/> keeps growing the result while possible.
    /// </summary>
    /// <typeparam name="T">The return type of the cached parser.</typeparam>
    internal sealed class GrowParser<T> : ParserBase<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrowParser{T}"/> class.
        /// </summary>
        /// <param name="parser">The parser to be cached.</param>
        public GrowParser(IParser<T> parser)
        {
            Parser = parser;
        }

        /// <summary>
        /// The parser to be cached.
        /// </summary>
        public IParser<T> Parser { get; }

        /// <inheritdoc />
        public override IParserImplementation<T> CreateUninitializedImplementation()
            => new GrowParserImpl<T>();

        /// <inheritdoc />
        protected override void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
        {
            Parser.PerformAnalysisStep(info, trace);
        }
    }
}
