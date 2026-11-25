using System;
using System.Collections.Generic;
using Warpstone.Internal.ParserImplementations;
using Warpstone.Internal.SourceGeneration;

namespace Warpstone.Internal.ParserExpressions
{
    /// <summary>
    /// Represents a parser that applies the <see name="Source"/> parser and then the parser
    /// that the <see name="Select"/> function picks for its result.
    /// </summary>
    /// <typeparam name="TIn">The result type of the <see name="Source"/> parser.</typeparam>
    /// <typeparam name="TSelected">The result type of the selected parser.</typeparam>
    /// <typeparam name="TOut">The result type of the <see name="Combine"/> function.</typeparam>
    internal sealed class BindParser<TIn, TSelected, TOut> : ParserBase<TOut>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindParser{TIn,TSelected,TOut}"/> class.
        /// </summary>
        /// <param name="source">The parser that is applied first.</param>
        /// <param name="select">The function that picks the parser to apply next.</param>
        /// <param name="combine">
        /// The function that combines both results, or <see langword="null"/> to return the result of the selected parser,
        /// which then must be of type <typeparamref name="TOut"/>.
        /// </param>
        public BindParser(IParser<TIn> source, Func<TIn, IParser<TSelected>> select, Func<TIn, TSelected, TOut>? combine)
        {
            Source = source;
            Select = select;
            Combine = combine;
        }

        /// <summary>
        /// The parser that is applied first.
        /// </summary>
        public IParser<TIn> Source { get; }

        /// <summary>
        /// The function that picks the parser to apply next.
        /// </summary>
        public Func<TIn, IParser<TSelected>> Select { get; }

        /// <summary>
        /// The function that combines both results, or <see langword="null"/> to return the result of the selected parser.
        /// </summary>
        public Func<TIn, TSelected, TOut>? Combine { get; }

        /// <summary>
        /// The memo table key that marks this parser as running a selected parser at a position where the source consumed nothing.
        /// Every implementation of this expression shares it, so re-entry is also found through the separate implementation of a selected parser.
        /// </summary>
        public IParserImplementation ReentryKey { get; } = new ReentryMarker();

        /// <inheritdoc />
        public override IParserImplementation<TOut> CreateUninitializedImplementation()
            => new BindParserImpl<TIn, TSelected, TOut>();

        /// <inheritdoc />
        public override void EmitSource(SourceEmitter emitter)
        {
            throw emitter.Unsupported("it creates parsers while parsing. Pass a parser instead of a function to Then, ThenAdd or ThenSkip");
        }

        /// <inheritdoc />
        protected override void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
        {
            // The selected parsers are only known while parsing and may recurse into the grammar,
            // so Auto mode must be ready to fall back to iterative parsing.
            info.HasRecursiveParsers = true;
            Source.PerformAnalysisStep(info, trace);
        }

        /// <inheritdoc />
        public override bool AnalyzeLeftEdge(Func<IParser, bool> isNullable, ICollection<IParser> leftEdge)
        {
            // A selected parser might consume nothing, so a nullable source makes this parser possibly nullable.
            leftEdge.Add(Source);
            return isNullable(Source);
        }

        private sealed class ReentryMarker : ContinuationParserImplementationBase
        {
            /// <inheritdoc />
            public override void Apply(IIterativeParseContext context, int position)
                => throw new NotSupportedException();
        }
    }
}
