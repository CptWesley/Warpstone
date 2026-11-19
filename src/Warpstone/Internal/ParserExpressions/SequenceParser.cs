using System;
using System.Collections.Generic;
using Warpstone.Internal.ParserImplementations;

namespace Warpstone.Internal.ParserExpressions
{
    /// <summary>
    /// A parser whose result can absorb a transformation, so that a sequence followed by a transformation runs as a single parser.
    /// </summary>
    /// <typeparam name="T">The result type of the parser.</typeparam>
    internal interface ISequenceParser<T> : IParser<T>
    {
        /// <summary>
        /// Creates a parser that parses the same sequence and transforms its result with <paramref name="map"/>.
        /// </summary>
        /// <typeparam name="TOut">The transformed result type.</typeparam>
        /// <param name="map">The transformation.</param>
        /// <returns>The new parser.</returns>
        public IParser<TOut> Map<TOut>(Func<T, TOut> map);
    }

    /// <summary>
    /// Represents a parser that parses two parsers in sequence and combines their results,
    /// without building an intermediate tuple.
    /// </summary>
    /// <typeparam name="TFirst">The result type of the first parser.</typeparam>
    /// <typeparam name="TSecond">The result type of the second parser.</typeparam>
    /// <typeparam name="TResult">The combined result type.</typeparam>
    internal sealed class SequenceParser<TFirst, TSecond, TResult> : ParserBase<TResult>, ISequenceParser<TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceParser{TFirst, TSecond, TResult}"/> class.
        /// </summary>
        /// <param name="first">The first parser.</param>
        /// <param name="second">The second parser.</param>
        /// <param name="combine">Combines the results of both parsers.</param>
        public SequenceParser(IParser<TFirst> first, IParser<TSecond> second, Func<TFirst, TSecond, TResult> combine)
        {
            First = first;
            Second = second;
            Combine = combine;
        }

        /// <summary>
        /// Gets the first parser.
        /// </summary>
        public IParser<TFirst> First { get; }

        /// <summary>
        /// Gets the second parser.
        /// </summary>
        public IParser<TSecond> Second { get; }

        /// <summary>
        /// Gets the function that combines the results of both parsers.
        /// </summary>
        public Func<TFirst, TSecond, TResult> Combine { get; }

        /// <inheritdoc />
        public IParser<TOut> Map<TOut>(Func<TResult, TOut> map)
        {
            var combine = Combine;
            return new SequenceParser<TFirst, TSecond, TOut>(First, Second, (first, second) => map(combine(first, second)));
        }

        /// <inheritdoc />
        public override IParserImplementation<TResult> CreateUninitializedImplementation()
            => new SequenceParserImpl<TFirst, TSecond, TResult>();

        /// <inheritdoc />
        protected override void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
        {
            First.PerformAnalysisStep(info, trace);
            Second.PerformAnalysisStep(info, trace);
        }

        /// <inheritdoc />
        public override bool AnalyzeLeftEdge(Func<IParser, bool> isNullable, ICollection<IParser> leftEdge)
        {
            leftEdge.Add(First);
            if (!isNullable(First))
            {
                return false;
            }

            leftEdge.Add(Second);
            return isNullable(Second);
        }
    }
}
