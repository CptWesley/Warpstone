using System.Collections.Generic;
using Warpstone.Errors;
using Warpstone.Internal.ParserExpressions;

namespace Warpstone.Internal.ParserImplementations
{
    /// <summary>
    /// Parser used for detecting the end of the input stream.
    /// </summary>
    internal sealed class EndParserImpl : ParserImplementationBase<EndParser, string>
    {
        /// <summary>
        /// Singleton instance of the parser.
        /// </summary>
        public static readonly EndParserImpl Instance = new();

        private EndParserImpl()
        {
        }

        /// <inheritdoc />
        protected override void InitializeInternal(EndParser parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
        {
            // Do nothing.
        }

        /// <inheritdoc />
        public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
        {
            return context.Input.MatchEnd(position) == InputMatch.Success
                ? new(position, 0, string.Empty)
                : new(position, new UnexpectedTokenError(context, this, position, 1, "EOF"));
        }

        /// <inheritdoc />
        public override void Apply(IIterativeParseContext context, int position)
        {
            switch (context.Input.MatchEnd(position))
            {
                case InputMatch.Success:
                    context.ResultStack.Push(new(position, 0, string.Empty));
                    break;
                case InputMatch.Pending:
                    context.ExecutionStack.Push((position, this));
                    break;
                default:
                    context.ResultStack.Push(new(position, new UnexpectedTokenError(context, this, position, 1, "EOF")));
                    break;
            }
        }
    }
}
