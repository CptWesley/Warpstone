namespace Warpstone.ParserImplementations;

/// <summary>
/// Represents a parser that parses a character.
/// </summary>
/// <param name="Value">The character to be parsed.</param>
internal sealed class CharacterParser(char Value) : IParser<char>
{
    private readonly string expected = $"'{Value}'";

    /// <inheritdoc />
    public Type ResultType => typeof(char);

    /// <inheritdoc />
    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var input = context.Input.Content;
        if (position >= input.Length || input[position] != Value)
        {
            return new(position, [new UnexpectedTokenError(context, this, position, 1, expected)]);
        }
        else
        {
            return new(position, 1, Value);
        }
    }

    /// <inheritdoc />
    public void Apply(IIterativeParseContext context, int position)
    {
        var input = context.Input.Content;
        if (position >= input.Length || input[position] != Value)
        {
            context.ResultStack.Push(new(position, [new UnexpectedTokenError(context, this, position, 1, expected)]));
        }
        else
        {
            context.ResultStack.Push(new(position, 1, Value));
        }
    }

    /// <inheritdoc />
    public override string ToString()
        => $"CharacterParser({expected})";
}
