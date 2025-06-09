using Warpstone.Internal.ParserExpressions;

namespace Warpstone.Internal.ParserImplementations;

/// <summary>
/// Represents a parser that parses a character.
/// </summary>
internal sealed class CharacterParserImpl : ParserImplementationBase<CharacterParser, char>
{
    private readonly string expected;

    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterParserImpl"/> class.
    /// </summary>
    /// <param name="c">The character to be parsed.</param>
    public CharacterParserImpl(char c)
    {
        Character = c;
        expected = $"'{c}'";
    }

    /// <summary>
    /// The character to be parsed.
    /// </summary>
    public char Character { get; }

    /// <inheritdoc />
    protected override void InitializeInternal(CharacterParser parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
    {
        // Do nothing.
    }

    /// <inheritdoc />
    public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var input = context.Input.Content;
        if (position >= input.Length || input[position] != Character)
        {
            return new(position, [new UnexpectedTokenError(context, this, position, 1, expected)]);
        }
        else
        {
            return new(position, 1, Character);
        }
    }

    /// <inheritdoc />
    public override void Apply(IIterativeParseContext context, int position)
    {
        var input = context.Input.Content;
        if (position >= input.Length || input[position] != Character)
        {
            context.ResultStack.Push(new(position, [new UnexpectedTokenError(context, this, position, 1, expected)]));
        }
        else
        {
            context.ResultStack.Push(new(position, 1, Character));
        }
    }
}
