using System;
using System.Collections.Generic;
using Warpstone.Internal.ParserImplementations;
using Warpstone.Internal.SourceGeneration;

namespace Warpstone.Internal.ParserExpressions
{
    /// <summary>
    /// Represents a parser that parses a repeated series of elements and aggregates the results.
    /// </summary>
    /// <typeparam name="TSource">The element type.</typeparam>
    /// <typeparam name="TAccumulator">The accumulator type.</typeparam>
    internal sealed class AggregateParser<TSource, TAccumulator> : ParserBase<TAccumulator>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateParser{TSource, TAccumulator}"/> class.
        /// </summary>
        /// <param name="element">The element parser.</param>
        /// <param name="delimiter">The optional delimiter parser.</param>
        /// <param name="minCount">The minimum number of parsed elements.</param>
        /// <param name="maxCount">The maximum number of parsed elements.</param>
        /// <param name="createSeed">The function to create the initial value of the accumulator.</param>
        /// <param name="accumulate">The accumulation function.</param>
        /// <param name="seedCode">The source of <paramref name="createSeed"/>, or <see langword="null"/> if it is unknown.</param>
        /// <param name="accumulateCode">The source of <paramref name="accumulate"/>, or <see langword="null"/> if it is unknown.</param>
        /// <param name="collectsList">Whether the accumulator is the list of elements, see <see cref="CollectsList"/>.</param>
#pragma warning disable S107 // Methods should not have too many parameters
        public AggregateParser(
            IParser<TSource> element,
            IParser? delimiter,
            int minCount,
            int maxCount,
            Func<TAccumulator> createSeed,
            Func<TAccumulator, TSource, TAccumulator> accumulate,
            CodeTemplate? seedCode,
            CodeTemplate? accumulateCode,
            bool collectsList = false)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            SeedCode = seedCode;
            AccumulateCode = accumulateCode;
            Element = element;
            Delimiter = delimiter;
            MinCount = minCount;
            MaxCount = maxCount;
            CreateSeed = createSeed;
            Accumulate = accumulate;
            CollectsList = collectsList;
        }

        /// <summary>
        /// The element parser.
        /// </summary>
        public IParser<TSource> Element { get; }

        /// <summary>
        /// The optional delimiter parser.
        /// </summary>
        public IParser? Delimiter { get; }

        /// <summary>
        /// The minimum number of parsed elements.
        /// </summary>
        public int MinCount { get; }

        /// <summary>
        /// The maximum number of parsed elements.
        /// </summary>
        public int MaxCount { get; }

        /// <summary>
        /// The function to create the initial value of the accumulator.
        /// </summary>
        public Func<TAccumulator> CreateSeed { get; }

        /// <summary>
        /// The accumulation function.
        /// </summary>
        public Func<TAccumulator, TSource, TAccumulator> Accumulate { get; }

        /// <summary>
        /// Gets the source of <see cref="CreateSeed"/>, or <see langword="null"/> if it is unknown.
        /// </summary>
        public CodeTemplate? SeedCode { get; }

        /// <summary>
        /// Gets the source of <see cref="Accumulate"/>, or <see langword="null"/> if it is unknown.
        /// </summary>
        public CodeTemplate? AccumulateCode { get; }

        /// <summary>
        /// Gets a value indicating whether the accumulator is the list of elements, as <see cref="Parsers.Many{TSource}(IParser{TSource})"/> builds it.
        /// The elements are then collected on a scratch stack of the parse and copied into an array of the exact size at the end,
        /// instead of calling <see cref="CreateSeed"/> and <see cref="Accumulate"/>.
        /// </summary>
        public bool CollectsList { get; }

        /// <inheritdoc />
        public override IParserImplementation<TAccumulator> CreateUninitializedImplementation()
            => new AggregateParserImpl<TSource, TAccumulator>();

        /// <inheritdoc />
        public override void EmitSource(SourceEmitter emitter)
        {
            var seed = emitter.Analyze(SeedCode, "seed", 0);
            var accumulate = emitter.Analyze(AccumulateCode, "accumulation", 2);

            // Only matching skips building the result, unless user code runs, which may throw.
            var accumulating = emitter.ValueNeeded || seed.UserCode || accumulate.UserCode;
            var stack = accumulating && CollectsList ? emitter.ScratchStack(typeof(TSource)) : null;
            var reset = stack is null ? null : $"{stack}Count = stackStart;";
            if (stack is not null)
            {
                emitter.Line($"var stackStart = {stack}Count;");
            }
            else if (accumulating)
            {
                emitter.Line($"var accumulator = {emitter.Expression(SeedCode, "seed")};");
            }

            emitter.Line("var count = 0;");
            emitter.Line("var next = pos;");
            emitter.Line("end = pos;");
            emitter.Open($"while (count < {MaxCount})");
            emitter.Line("var start = next;");
            if (Delimiter is not null)
            {
                emitter.Open($"if (count > 0 && !{emitter.Invoke(Delimiter, "next", "start", null)})");
                emitter.Open($"if (count >= {MinCount})");
                emitter.Line("break;");
                emitter.Close();
                FailAndReset(emitter, reset);
                emitter.Close();
            }

            emitter.Open($"if (!{emitter.Invoke(Element, "start", "var elementEnd", accumulating && accumulate.Uses[1] ? "var element" : null)})");
            emitter.Open($"if (count >= {MinCount})");
            emitter.Line("break;");
            emitter.Close();
            FailAndReset(emitter, reset);
            emitter.Close();
            if (stack is not null)
            {
                emitter.Open($"if ({stack}Count == {stack}.Length)");
                emitter.Line($"global::System.Array.Resize(ref {stack}, {stack}Count * 2);");
                emitter.Close();
                emitter.Line($"{stack}[{stack}Count++] = element;");
            }
            else if (accumulating)
            {
                emitter.Line($"accumulator = {emitter.Expression(AccumulateCode, "accumulation", "accumulator", "element")};");
            }

            emitter.Line("next = elementEnd;");
            emitter.Line("end = elementEnd;");
            emitter.Line("count++;");
            emitter.Close();
            if (stack is null)
            {
                emitter.Value("accumulator");
            }
            else
            {
                // Pop the elements into an array of the exact size.
                var type = emitter.TypeName(typeof(TSource));
                emitter.Open("if (count == 0)");
                emitter.Line($"value = global::System.Array.Empty<{type}>();");
                emitter.Close();
                emitter.Open("else");
                emitter.Line($"var items = new {type}[count];");
                emitter.Line($"global::System.Array.Copy({stack}, stackStart, items, 0, count);");
                emitter.Line(reset!);
                emitter.Line("value = items;");
                emitter.Close();
            }

            emitter.Line("return true;");
        }

        private static void FailAndReset(SourceEmitter emitter, string? reset)
        {
            if (reset is not null)
            {
                emitter.Line(reset);
            }

            emitter.Fail();
        }

        /// <inheritdoc />
        protected override void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
        {
            Element.PerformAnalysisStep(info, trace);
            Delimiter?.PerformAnalysisStep(info, trace);
        }

        /// <inheritdoc />
        public override bool AnalyzeLeftEdge(Func<IParser, bool> isNullable, ICollection<IParser> leftEdge)
        {
            leftEdge.Add(Element);
            var elementNullable = isNullable(Element);
            if (elementNullable && Delimiter is not null)
            {
                leftEdge.Add(Delimiter);
            }

            return MinCount == 0 || (elementNullable && (MinCount == 1 || Delimiter is null || isNullable(Delimiter)));
        }
    }
}
