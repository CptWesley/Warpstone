using System.Collections.Generic;
using Warpstone.Errors;
using Warpstone.Internal.ParserExpressions;

namespace Warpstone.Internal.ParserImplementations
{
    /// <summary>
    /// Represents a parser that parses whitespace.
    /// </summary>
    internal sealed class WhitespaceParsersImpl : ParserImplementationBase<WhitespacesParser, string>
    {
        /// <summary>
        /// Singleton instance of the parser.
        /// </summary>
        public static readonly WhitespaceParsersImpl Instance = new();

        private WhitespaceParsersImpl()
        {
        }

        /// <inheritdoc />
        public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
        {
            return context.Input.MatchWhitespaces(position, out var match) == InputMatch.Success
                ? new(position, match!.Length, match)
                : new(position, new UnexpectedTokenError(context, this, position, 1, "whitespace"));
        }

        /// <inheritdoc />
        public override void Apply(IIterativeParseContext context, int position)
        {
            switch (context.Input.MatchWhitespaces(position, out var match))
            {
                case InputMatch.Success:
                    context.ResultStack.Push(new(position, match!.Length, match));
                    break;
                case InputMatch.Pending:
                    context.ExecutionStack.Push((position, this));
                    break;
                default:
                    context.ResultStack.Push(new(position, new UnexpectedTokenError(context, this, position, 1, "whitespace")));
                    break;
            }
        }

        /// <inheritdoc />
        protected override void InitializeInternal(WhitespacesParser parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
        {
            // Do nothing.
        }
    }
}
