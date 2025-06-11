using System.Collections.Generic;
using Warpstone.Errors;
using Warpstone.Internal.ParserExpressions;

namespace Warpstone.Internal.ParserImplementations
{
    /// <summary>
    /// Represents a parser that always fails.
    /// </summary>
    /// <typeparam name="T">The result type of the parser.</typeparam>
    internal sealed class FailParserImpl<T> : ParserImplementationBase<FailParser<T>, T>
    {
        /// <summary>
        /// The singleton instance of the parser.
        /// </summary>
        public static readonly FailParserImpl<T> Instance = new();

        private FailParserImpl()
        {
        }

        /// <inheritdoc />
        protected override void InitializeInternal(FailParser<T> parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
        {
            // Do nothing.
        }

        /// <inheritdoc />
        public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
        {
            return new UnsafeParseResult(position, new UnexpectedTokenError(context, this, position, 1, string.Empty));
        }

        /// <inheritdoc />
        public override void Apply(IIterativeParseContext context, int position)
        {
            context.ResultStack.Push(new UnsafeParseResult(position, new UnexpectedTokenError(context, this, position, 1, string.Empty)));
        }
    }
}
