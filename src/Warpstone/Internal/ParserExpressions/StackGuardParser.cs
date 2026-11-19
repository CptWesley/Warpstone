using System;
using System.Collections.Generic;
using Warpstone.Internal.ParserImplementations;

namespace Warpstone.Internal.ParserExpressions
{
    /// <summary>
    /// Represents a parser which checks that enough stack space is left before running the given parser
    /// in recursive mode. It is placed on the cycles of recursive grammars, so a too deep recursion
    /// throws an <see cref="InsufficientExecutionStackException"/> instead of overflowing the stack.
    /// </summary>
    /// <typeparam name="T">The return type of the guarded parser.</typeparam>
    internal sealed class StackGuardParser<T> : ParserBase<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StackGuardParser{T}"/> class.
        /// </summary>
        /// <param name="parser">The parser to be guarded.</param>
        public StackGuardParser(IParser<T> parser)
        {
            Parser = parser;
        }

        /// <summary>
        /// The parser to be guarded.
        /// </summary>
        public IParser<T> Parser { get; }

        /// <inheritdoc />
        public override IParserImplementation<T> CreateUninitializedImplementation()
            => new StackGuardParserImpl<T>();

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
