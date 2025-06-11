namespace Warpstone.Internal.ParserExpressions
{
    /// <summary>
    /// Represents a parser that parses a string.
    /// </summary>
    internal sealed class StringParser : ParserBase<string>
    {
        private static readonly int baseHash = typeof(StringParser).GetHashCode() * 31;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringParser"/> class.
        /// </summary>
        /// <param name="str">The string to be parsed.</param>
        /// <param name="culture">The culture used for comparing.</param>
        /// <param name="options">The options used for comparing.</param>
        public StringParser(string str, CultureInfo culture, CompareOptions options)
        {
            String = str;
            Culture = culture;
            Options = options;
        }

        /// <summary>
        /// The string to be parsed.
        /// </summary>
        public string String { get; }

        /// <summary>
        /// The culture used for comparing.
        /// </summary>
        public CultureInfo Culture { get; }

        /// <summary>
        /// The options used for comparing.
        /// </summary>
        public CompareOptions Options { get; }

        /// <inheritdoc />
        public override IParserImplementation<string> CreateUninitializedImplementation()
            => new StringParserImpl(String, Culture, Options);

        /// <inheritdoc />
        protected override void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
        {
            // Do nothing.
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is StringParser other
            && other.String == String
            && Equals(other.Culture, Culture)
            && other.Options == Options;

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hash = baseHash;
            hash = (hash * 31) + String.GetHashCode();
            hash = (hash * 31) + Culture.GetHashCode();
            hash = (hash * 31) + Options.GetHashCode();
            return hash;
        }
    }
}
