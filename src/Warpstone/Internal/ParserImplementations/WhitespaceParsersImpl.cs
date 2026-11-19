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
            var content = context.Input.Content;
            var pos = position;

            while (pos < content.Length && char.IsWhiteSpace(content[pos]))
            {
                pos++;
            }

            if (pos == content.Length && ReaderParseInput.RequestMore(context.Input, pos + 1))
            {
                return Apply(context, position);
            }

            return pos == position
                ? new(position, new UnexpectedTokenError(context, this, position, 1, "whitespace"))
                : new(position, pos - position, content[position..pos]);
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

            if (pos == content.Length && ReaderParseInput.RequestMore(context.Input, pos + 1))
            {
                context.ExecutionStack.Push((position, this));
                return;
            }

            if (pos > position)
            {
                context.ResultStack.Push(new(position, pos - position, content[position..pos]));
            }
            else
            {
                context.ResultStack.Push(new(position, new UnexpectedTokenError(context, this, position, 1, "whitespace")));
            }
        }

        /// <inheritdoc />
        protected override void InitializeInternal(WhitespacesParser parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
        {
            // Do nothing.
        }
    }
}
