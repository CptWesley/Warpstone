using System.Collections.Generic;
using Warpstone.Internal.ParserImplementations;

namespace Warpstone.Internal.ParserExpressions
{
    /// <summary>
    /// Represents a parser that parses whitespaces.
    /// </summary>
    internal sealed class WhitespacesParser : ParserBase<string>
    {
        /// <summary>
        /// Singleton instance of the parser.
        /// </summary>
        public static readonly WhitespacesParser Instance = new();

        private WhitespacesParser()
        {
        }

        /// <inheritdoc />
        public override IParserImplementation<string> CreateUninitializedImplementation()
            => WhitespaceParsersImpl.Instance;

        /// <inheritdoc />
        protected override void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
        {
            // Do nothing.
        }
    }
}
