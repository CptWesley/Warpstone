//HintName: Warpstone.Sources.embedded.Internal.ParserImplementations.EndParserImpl.cs
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
            if (position >= context.Input.Content.Length)
            {
                return new(position, 0, string.Empty);
            }
            else
            {
                return new(position, new UnexpectedTokenError(context, this, position, 1, "EOF"));
            }
        }

        /// <inheritdoc />
        public override void Apply(IIterativeParseContext context, int position)
        {
            if (position >= context.Input.Content.Length)
            {
                context.ResultStack.Push(new(position, 0, string.Empty));
            }
            else
            {
                context.ResultStack.Push(new(position, new UnexpectedTokenError(context, this, position, 1, "EOF")));
            }
        }
    }
}
