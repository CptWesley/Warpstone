using System.Collections.Generic;
using Warpstone.Internal.ParserExpressions;

namespace Warpstone.Internal.ParserImplementations
{
    /// <summary>
    /// Represents a parser that parses (optional) whitespaces.
    /// </summary>
    internal sealed class OptionalWhitespacesParserImpl : ParserImplementationBase<OptionalWhitespacesParser, string>
    {
        /// <summary>
        /// Singleton instance of the parser.
        /// </summary>
        public static readonly OptionalWhitespacesParserImpl Instance = new();

        private OptionalWhitespacesParserImpl()
        {
        }

        /// <inheritdoc />
        public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
        {
            context.Input.MatchWhitespaces(position, out var match);
            return new(position, match!.Length, match);
        }

        /// <inheritdoc />
        public override void Apply(IIterativeParseContext context, int position)
        {
            if (context.Input.MatchWhitespaces(position, out var match) == InputMatch.Pending)
            {
                context.ExecutionStack.Push((position, this));
            }
            else
            {
                context.ResultStack.Push(new(position, match!.Length, match));
            }
        }

        /// <inheritdoc />
        protected override void InitializeInternal(OptionalWhitespacesParser parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
        {
            // Do nothing.
        }
    }
}
