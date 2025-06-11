using System;
using System.Collections.Generic;

namespace Warpstone.Internal.ParserExpressions
{
    /// <summary>
    /// Represents a parser that lazily executed the given <see name="Parser"/>.
    /// </summary>
    /// <typeparam name="T">The result type of the <see name="Parser"/>.</typeparam>
    internal sealed class LazyParser<T> : ParserBase<T>, ILazyParser<T>
    {
        private readonly Lazy<IParser<T>> typedLazy;
        private readonly Lazy<IParser> untypedLazy;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyParser{T}"/> class.
        /// </summary>
        /// <param name="parser">The lazily executed parser.</param>
        public LazyParser(Lazy<IParser<T>> parser)
        {
            typedLazy = parser;
            untypedLazy = new(() => typedLazy.Value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyParser{T}"/> class.
        /// </summary>
        /// <param name="get">The generator for the lazy parser.</param>
        public LazyParser(Func<IParser<T>> get)
            : this(new Lazy<IParser<T>>(get))
        {
        }

        /// <inheritdoc />
        public Lazy<IParser<T>> Parser => typedLazy;

        /// <inheritdoc />
        Lazy<IParser> ILazyParser.Parser => untypedLazy;

        /// <inheritdoc />
        public override IParserImplementation<T> CreateUninitializedImplementation()
            => throw new NotSupportedException("Lazy parsers should not occur in the parsers implementations constructed from the expressions.");

        /// <inheritdoc />
        protected override void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
        {
            Parser.Value.PerformAnalysisStep(info, trace);
        }
    }
}
