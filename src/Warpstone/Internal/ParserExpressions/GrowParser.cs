using System;
using System.Collections.Generic;
using Warpstone.Internal.ParserImplementations;
using Warpstone.Internal.SourceGeneration;

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
        public override void EmitSource(SourceEmitter emitter)
        {
            var table = emitter.MemoTable(Parser);
            var entry = $"__Entry<{emitter.TypeName(typeof(T))}>";
            emitter.Open($"if ({table}.TryGetValue(pos, out var hit))");
            emitter.Line("end = hit.End;");
            emitter.Line("value = hit.Value;");
            emitter.Line("return hit.State == 3;");
            emitter.Close();

            // Seed growing: start from a failure, and run the parser again while its result keeps getting longer.
            emitter.Line($"{table}[pos] = new {entry}(2, pos, default!);");
            emitter.Line("var success = false;");
            emitter.Line("end = pos;");
            emitter.Line("value = default!;");
            emitter.Open("while (true)");
            emitter.Open($"if (!{emitter.Call(Parser)}(pos, out var grownEnd, out var grown) || (success && grownEnd <= end))");
            emitter.Line("break;");
            emitter.Close();
            emitter.Line("success = true;");
            emitter.Line("end = grownEnd;");
            emitter.Line("value = grown;");
            emitter.Line($"{table}[pos] = new {entry}(3, end, value);");
            emitter.Close();
            emitter.Line("return success;");
        }

        /// <inheritdoc />
        protected override void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
        {
            Parser.PerformAnalysisStep(info, trace);
        }

        /// <inheritdoc />
        public override bool AnalyzeLeftEdge(Func<IParser, bool> isNullable, ICollection<IParser> leftEdge)
        {
            leftEdge.Add(Parser);
            return isNullable(Parser);
        }
    }
}
