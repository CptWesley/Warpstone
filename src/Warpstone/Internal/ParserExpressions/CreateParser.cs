using System;
using System.Collections.Generic;
using Warpstone.Internal.ParserImplementations;
using Warpstone.Internal.SourceGeneration;

namespace Warpstone.Internal.ParserExpressions
{
    /// <summary>
    /// Represents a parser that always passes.
    /// </summary>
    /// <typeparam name="T">The type of the value that is always returned.</typeparam>
    internal sealed class CreateParser<T> : ParserBase<T>
    {
        private static readonly int baseHash = typeof(CreateParser<T>).GetHashCode() * 31;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateParser{T}"/> class.
        /// </summary>
        /// <param name="value">The value that is always returned.</param>
        public CreateParser(T value)
        {
            Value = value;
        }

        /// <summary>
        /// The value that is always returned.
        /// </summary>
        public T Value { get; }

        /// <inheritdoc />
        public override IParserImplementation<T> CreateUninitializedImplementation()
            => new CreateParserImpl<T>(Value);

        /// <inheritdoc />
        public override void EmitSource(SourceEmitter emitter)
        {
            emitter.Line("end = pos;");
            emitter.Line($"value = {emitter.Literal(Value, typeof(T))};");
            emitter.Line("return true;");
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
            => obj is CreateParser<T> other
            && Equals(other.Value, Value);

        /// <inheritdoc />
        public override int GetHashCode()
            => baseHash + (Value?.GetHashCode() ?? 0);
    }
}
