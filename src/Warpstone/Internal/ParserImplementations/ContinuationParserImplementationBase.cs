using System;
using System.Collections.Generic;

namespace Warpstone.Internal.ParserImplementations
{
    /// <summary>
    /// Provides a base implementation for continuations.
    /// </summary>
    internal abstract class ContinuationParserImplementationBase : IParserImplementation
    {
        /// <inheritdoc />
        public IParser ParserExpression => throw new NotSupportedException();

        /// <inheritdoc />
        public UnsafeParseResult Apply(IRecursiveParseContext context, int position) => throw new NotSupportedException();

        /// <inheritdoc />
        public void Initialize(IParser parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup) => throw new NotSupportedException();

        /// <inheritdoc />
        public abstract void Apply(IIterativeParseContext context, int position);
    }
}
