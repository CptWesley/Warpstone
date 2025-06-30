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

        /// <inheritdoc />
        public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
        {
            var content = context.Input.Content;
            var pos = position;

            while (pos < content.Length && char.IsWhiteSpace(content[pos]))
            {
                pos++;
            }

            return new(position, pos - position, content[position..pos]);
        }

        /// <inheritdoc />
        public override void Apply(IIterativeParseContext context, int position)
        {
            var content = context.Input.Content;
            var pos = position;

            while (pos < content.Length && char.IsWhiteSpace(content[pos]))
            {
                pos++;
            }
            context.ResultStack.Push(new(position, pos - position, content[position..pos]));
        }

        /// <inheritdoc />
        protected override void InitializeInternal(OptionalWhitespacesParser parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
        {
            // Do nothing.
        }
    }
}
