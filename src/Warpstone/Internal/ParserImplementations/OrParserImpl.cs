using System.Collections.Generic;
using Warpstone.Internal.ParserExpressions;

namespace Warpstone.Internal.ParserImplementations
{
    /// <summary>
    /// Represents a parser that parses either the provided first or second option.
    /// </summary>
    /// <typeparam name="T">The result type of the parsers.</typeparam>
    internal sealed class OrParserImpl<T> : ParserImplementationBase<OrParser<T>, T>
    {
        private IParserImplementation<T> first = default!;
        private IParserImplementation<T> second = default!;
        private Continuation continuation = default!;

        /// <inheritdoc />
        protected override void InitializeInternal(OrParser<T> parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
        {
            first = (IParserImplementation<T>)parserLookup[parser.First];
            second = (IParserImplementation<T>)parserLookup[parser.Second];
            continuation = new(second);
        }

        /// <inheritdoc />
        public override void Apply(IIterativeParseContext context, int position)
        {
            context.ExecutionStack.Push((position, continuation));
            context.ExecutionStack.Push((position, first));
        }

        /// <inheritdoc />
        public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
        {
            var left = first.Apply(context, position);

            // Both options report their own failures, so a failing second option's result is the result.
            return left.Success ? left : second.Apply(context, position);
        }

        private sealed class Continuation : ContinuationParserImplementationBase
        {
            private readonly IParserImplementation second;

            public Continuation(IParserImplementation second)
            {
                this.second = second;
            }

            /// <inheritdoc />
            public override void Apply(IIterativeParseContext context, int position)
            {
                if (context.ResultStack.Peek().Success)
                {
                    return;
                }

                context.ResultStack.Pop();
                context.ExecutionStack.Push((position, second));
            }
        }
    }
}
