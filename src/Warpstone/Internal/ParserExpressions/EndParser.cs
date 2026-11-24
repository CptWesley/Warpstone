using System;
using System.Collections.Generic;
using Warpstone.Internal.ParserImplementations;
using Warpstone.Internal.SourceGeneration;

namespace Warpstone.Internal.ParserExpressions
{
    /// <summary>
    /// Parser used for detecting the end of the input stream.
    /// </summary>
    internal sealed class EndParser : ParserBase<string>
    {
        private static readonly int baseHash = typeof(EndParser).GetHashCode() * 31;

        /// <summary>
        /// Singleton instance of the parser.
        /// </summary>
        public static readonly EndParser Instance = new();

        private EndParser()
        {
        }

        /// <inheritdoc />
        public override IParserImplementation<string> CreateUninitializedImplementation()
            => EndParserImpl.Instance;

        /// <inheritdoc />
        public override void EmitSource(SourceEmitter emitter)
        {
            emitter.Open("if (pos >= input.Length)");
            emitter.Line("end = pos;");
            emitter.Value("string.Empty");
            emitter.Line("return true;");
            emitter.Close();
            emitter.Fail("EOF");
        }

        /// <inheritdoc />
        protected override void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
        {
            // Do nothing.
        }

        /// <inheritdoc />
        public override bool AnalyzeLeftEdge(Func<IParser, bool> isNullable, ICollection<IParser> leftEdge)
        {
            return true;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is EndParser;

        /// <inheritdoc />
        public override int GetHashCode()
            => baseHash;
    }
}
