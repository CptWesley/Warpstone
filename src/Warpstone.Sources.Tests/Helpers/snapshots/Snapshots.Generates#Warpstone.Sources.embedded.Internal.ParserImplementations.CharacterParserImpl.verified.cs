//HintName: Warpstone.Sources.embedded.Internal.ParserImplementations.CharacterParserImpl.cs
using System.Collections.Generic;
using Warpstone.Errors;
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
        protected override void InitializeInternal(CharacterParser parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
        {
            // Do nothing.
        }

        /// <inheritdoc />
        public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
        {
            var input = context.Input.Content;
            if (position >= input.Length || input[position] != character)
            {
                return new(position, new UnexpectedTokenError(context, this, position, 1, expected));
            }
            else
            {
                return new(position, 1, character);
            }
        }

        /// <inheritdoc />
        public override void Apply(IIterativeParseContext context, int position)
        {
            var input = context.Input.Content;
            if (position >= input.Length || input[position] != character)
            {
                context.ResultStack.Push(new(position, new UnexpectedTokenError(context, this, position, 1, expected)));
            }
            else
            {
                context.ResultStack.Push(new(position, 1, character));
            }
        }
    }
}
