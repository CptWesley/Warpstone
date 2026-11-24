using System;
using System.Collections.Generic;
using Warpstone.Internal.ParserImplementations;
using Warpstone.Internal.SourceGeneration;

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
        public override void EmitSource(SourceEmitter emitter)
        {
            emitter.Line("var next = pos;");
            emitter.Open("while (next < input.Length && char.IsWhiteSpace(input[next]))");
            emitter.Line("next++;");
            emitter.Close();
            emitter.Open("if (next > pos)");
            emitter.Line("end = next;");
            emitter.Value("input.Substring(pos, next - pos)");
            emitter.Line("return true;");
            emitter.Close();
            emitter.Fail("whitespace");
        }

        /// <inheritdoc />
        protected override void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
        {
            // Do nothing.
        }

        /// <inheritdoc />
        public override bool AnalyzeLeftEdge(Func<IParser, bool> isNullable, ICollection<IParser> leftEdge)
        {
            return false;
        }
    }
}
