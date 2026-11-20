using System.Collections.Generic;
using Warpstone.Internal.ParserExpressions;

namespace Warpstone.Internal.ParserImplementations
{
    /// <summary>
    /// Represents a parser that parses a character.
    /// </summary>
    internal sealed class CharacterParserImpl : ParserImplementationBase<CharacterParser, char>
    {
        private readonly string expected;
        private readonly char character;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterParserImpl"/> class.
        /// </summary>
        /// <param name="c">The character to be parsed.</param>
        public CharacterParserImpl(char c)
        {
            character = c;
            expected = $"'{c}'";
        }

        /// <inheritdoc />
        public override FirstChars? GetFirstChars(ISet<IParserImplementation> visiting)
            => FirstChars.Leaf([character], this, expected);

        /// <inheritdoc />
        protected override void InitializeInternal(CharacterParser parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
        {
            // Do nothing.
        }

        /// <inheritdoc />
        public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
        {
            return context.Input.Match(position, character) == InputMatch.Success
                ? new(position, 1, character)
                : ReportExpected(context, position, expected);
        }

        /// <inheritdoc />
        public override void Apply(IIterativeParseContext context, int position)
        {
            switch (context.Input.Match(position, character))
            {
                case InputMatch.Success:
                    context.ResultStack.Push(new(position, 1, character));
                    break;
                case InputMatch.Pending:
                    context.ExecutionStack.Push((position, this));
                    break;
                default:
                    context.ResultStack.Push(ReportExpected(context, position, expected));
                    break;
            }
        }
    }
}
