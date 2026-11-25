using System;
using System.Collections.Generic;
using Warpstone.Internal.ParserImplementations;
using Warpstone.Internal.SourceGeneration;

namespace Warpstone.Internal.ParserExpressions
{
    /// <summary>
    /// Represents one parser of a left-recursive component that has no single leader (see <see cref="LeftRecursionAnalysis"/>).
    /// All parsers of the component grow together, following Umeda &amp; Maeda (JIP vol. 29, 2021).
    /// Only created by automatic growing recursion.
    /// </summary>
    /// <typeparam name="T">The return type of the wrapped parser.</typeparam>
    internal sealed class CycleGrowParser<T> : ParserBase<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CycleGrowParser{T}"/> class.
        /// </summary>
        /// <param name="parser">The wrapped parser.</param>
        /// <param name="group">The group shared by all parsers of the component.</param>
        public CycleGrowParser(IParser<T> parser, CycleGrowGroup group)
        {
            Parser = parser;
            Group = group;
        }

        /// <summary>
        /// The wrapped parser.
        /// </summary>
        public IParser<T> Parser { get; }

        /// <summary>
        /// The group shared by all parsers of the component.
        /// </summary>
        public CycleGrowGroup Group { get; }

        /// <inheritdoc />
        public override IParserImplementation<T> CreateUninitializedImplementation()
            => new CycleGrowParserImpl<T>();

        /// <inheritdoc />
        public override void EmitSource(SourceEmitter emitter)
        {
            var table = emitter.MemoTable(Parser);
            var group = emitter.CycleGroup(Group);
            var id = emitter.CycleId(this);
            var inner = emitter.Call(Parser);
            var entry = $"__Entry<{emitter.TypeName(typeof(T))}>";
            var limits = $"new global::System.Collections.Generic.HashSet<int> {{ {id} }}";

            emitter.Open($"if (!{group}.TryGetValue(pos, out var state))");
            emitter.Line("state = new __CycleState();");
            emitter.Line($"{group}[pos] = state;");
            emitter.Close();

            // Growing another member of the group: run once and keep the best result.
            emitter.Open($"if (state.Limits != null && state.Limits.Add({id}))");
            emitter.Line($"var success = {inner}(pos, out end, out value);");
            emitter.Open($"if ({table}.TryGetValue(pos, out var best) && !Improved(best, success, end))");
            emitter.Line("end = best.End;");
            emitter.Line("value = best.Value;");
            emitter.Line("return best.State == 3;");
            emitter.Close();
            emitter.Line($"{table}[pos] = new {entry}((byte)(success ? 3 : 2), end, value);");
            emitter.Line("return success;");
            emitter.Close();

            emitter.Open($"if ({table}.TryGetValue(pos, out var memo))");
            emitter.Open($"if (state.InProgress.Contains({id}))");
            emitter.Line($"state.GrowRequested.Add({id});");
            emitter.Close();
            emitter.Line("end = memo.End;");
            emitter.Line("value = memo.Value;");
            emitter.Line("return memo.State == 3;");
            emitter.Close();

            // Plant a failing seed; a recursive call that hits it asks this parser to grow.
            emitter.Line($"state.InProgress.Add({id});");
            emitter.Line($"{table}[pos] = new {entry}(2, pos, default!);");
            emitter.Line($"var seed = {inner}(pos, out end, out value);");
            emitter.Line($"{table}[pos] = new {entry}((byte)(seed ? 3 : 2), end, value);");
            emitter.Line($"state.InProgress.Remove({id});");
            emitter.Open($"if (!state.GrowRequested.Remove({id}))");
            emitter.Line("return seed;");
            emitter.Close();

            emitter.Line("state.Suspended.Push(state.Limits);");
            emitter.Line($"state.Limits = {limits};");
            emitter.Open("while (true)");
            emitter.Line($"var grown = {inner}(pos, out var grownEnd, out var grownValue);");
            emitter.Open($"if (!Improved({table}[pos], grown, grownEnd))");
            emitter.Line("state.Limits = state.Suspended.Pop();");
            emitter.Line("break;");
            emitter.Close();
            emitter.Line($"{table}[pos] = new {entry}(3, grownEnd, grownValue);");
            emitter.Line($"state.Limits = {limits};");
            emitter.Close();

            emitter.Line($"var result = {table}[pos];");
            emitter.Line("end = result.End;");
            emitter.Line("value = result.Value;");
            emitter.Line("return result.State == 3;");
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
