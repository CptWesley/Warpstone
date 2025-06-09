namespace Warpstone.Internal.ParserImplementations;

/// <summary>
/// Represents a parser that parses a string.
/// </summary>
/// <param name="Value">The string to be parsed.</param>
/// <param name="Culture">The culture used for comparing.</param>
/// <param name="Options">The options used for comparing.</param>
internal sealed class StringParserImpl(string Value, CultureInfo Culture, CompareOptions Options) : IParserImplementation<string>
{
    private readonly string expected = @$"""{Value}""";
    private readonly bool useValue = Options is CompareOptions.Ordinal;

    /// <inheritdoc />
    public void Apply(IIterativeParseContext context, int position)
    {
        var input = context.Input.Content;

        if (!Matches(input, position))
        {
            context.ResultStack.Push(new UnsafeParseResult(position, [new UnexpectedTokenError(context, this, position, 1, expected)]));
        }
        else
        {
            context.ResultStack.Push(new(position, Value.Length, useValue ? Value : input.Substring(position, Value.Length)));
        }
    }

    /// <inheritdoc />
    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var input = context.Input.Content;

        if (!Matches(input, position))
        {
            return new(position, [new UnexpectedTokenError(context, this, position, 1, expected)]);
        }
        else
        {
            return new(position, Value.Length, useValue ? Value : input.Substring(position, Value.Length));
        }
    }

    private bool Matches(string input, int position)
    {
        var endPos = position + Value.Length;

        if (endPos > input.Length)
        {
            return false;
        }

        var result = string.Compare(input, position, Value, 0, Value.Length, Culture, Options);
        return result == 0;
    }

    /// <inheritdoc />
    public override string ToString()
        => $"StringParser({expected})";
}
