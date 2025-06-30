using System.Collections.Generic;
using Warpstone.Internal.ParserImplementations;

namespace Warpstone.Internal.ParserExpressions
{
    /// <summary>
    /// Represents a parser that parses (optional) whitespaces.
    /// </summary>
    internal sealed class OptionalWhitespacesParser : ParserBase<string>
    {
        /// <summary>
        /// Singleton instance of the parser.
        /// </summary>
        public static readonly OptionalWhitespacesParser Instance = new();

        /// <inheritdoc />
        public override IParserImplementation<string> CreateUninitializedImplementation()
            => OptionalWhitespacesParserImpl.Instance;

        /// <inheritdoc />
        protected override void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
        {
            // Do nothing.
        }
    }
}
