namespace Warpstone
{
    /// <summary>
    /// Interface for all parser implementations.
    /// </summary>
    public interface IParser
    {
        /// <summary>
        /// Gets the result type of the parser.
        /// </summary>
        public Type ResultType { get; }

        /// <summary>
        /// Gets the parser implementation for the current parser expression with the given <paramref name="options"/>.
        /// </summary>
        /// <param name="options">The options used for finding the correct parser implementation.</param>
        /// <returns>The found parser implementation.</returns>
        public IParserImplementation GetImplementation(ParseOptions options);

        /// <summary>
        /// Performs the analysis of the parser expression.
        /// </summary>
        /// <returns>The found analysis info.</returns>
        public IReadOnlyParserAnalysisInfo Analyze();

        /// <summary>
        /// Performs an analysis step of the parser expression by updating the <paramref name="info"/> and <paramref name="trace"/> info.
        /// </summary>
        /// <param name="info">The resulting info.</param>
        /// <param name="trace">The trace to reach the current parser node.</param>
        public void PerformAnalysisStep(IParserAnalysisInfo info, IReadOnlyList<IParser> trace);

        /// <summary>
        /// Creates a new uninitialized instance of the <see cref="IParserImplementation"/> that corresponds
        /// to this <see cref="IParser"/> expression.
        /// </summary>
        /// <returns>The newly created uninitialized <see cref="IParserImplementation"/> instance.</returns>
        public IParserImplementation CreateUninitializedImplementation();
    }

    /// <summary>
    /// Interface for all typed parser implementations.
    /// </summary>
    /// <typeparam name="T">The result type being parsed.</typeparam>
    public interface IParser<out T> : IParser
    {
        /// <inheritdoc cref="IParser.GetImplementation(ParseOptions)" />
        public new IParserImplementation<T> GetImplementation(ParseOptions options);

        /// <inheritdoc cref="IParser.CreateUninitializedImplementation()" />
        public new IParserImplementation<T> CreateUninitializedImplementation();
    }
}
