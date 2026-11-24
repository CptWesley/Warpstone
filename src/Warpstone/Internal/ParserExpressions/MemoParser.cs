using System;
using System.Collections.Generic;
using Warpstone.Internal.ParserImplementations;
using Warpstone.Internal.SourceGeneration;

namespace Warpstone.Internal.ParserExpressions
{
    /// <summary>
    /// Represents a parser which caches the result for a given parser at a given position
    /// so that it can be reused later if the same parser is executed at the same position again.
    /// </summary>
    /// <typeparam name="T">The return type of the cached parser.</typeparam>
    internal sealed class MemoParser<T> : ParserBase<T>, IMemoParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemoParser{T}"/> class.
        /// </summary>
        /// <param name="parser">The parser to be cached.</param>
        public MemoParser(IParser<T> parser)
        {
            Parser = parser;
        }

        /// <summary>
        /// The parser to be cached.
        /// </summary>
        public IParser<T> Parser { get; }

        /// <inheritdoc />
        IParser IMemoParser.Parser => Parser;

        /// <inheritdoc />
        public override IParserImplementation<T> CreateUninitializedImplementation()
            => new MemoParserImpl<T>();

        /// <inheritdoc />
        public override void EmitSource(SourceEmitter emitter)
        {
            var table = emitter.MemoTable(Parser);
            var entry = $"__Entry<{emitter.TypeName(typeof(T))}>";
            emitter.Open($"if ({table}.TryGetValue(pos, out var hit))");
            emitter.Open("if (hit.State == 1)");
            emitter.Line("Error(pos, new global::System.InvalidOperationException(\"Infinite recursion at position \" + pos + \".\"));");
            emitter.Fail();
            emitter.Close();
            emitter.Line("end = hit.End;");
            emitter.Line("value = hit.Value;");
            emitter.Line("return hit.State == 3;");
            emitter.Close();
            emitter.Line($"{table}[pos] = new {entry}(1, pos, default!);");
            emitter.Line($"var success = {emitter.Call(Parser)}(pos, out end, out value);");
            emitter.Line($"{table}[pos] = new {entry}((byte)(success ? 3 : 2), end, value);");
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

    /// <summary>
    /// Untyped access to the parser cached by a <see cref="MemoParser{T}"/>.
    /// </summary>
    internal interface IMemoParser : IParser
    {
        /// <summary>
        /// The parser to be cached.
        /// </summary>
        public IParser Parser { get; }
    }
}
