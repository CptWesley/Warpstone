namespace Warpstone.ParserImplementations;

/// <summary>
/// Represents a parser that parses an exact string.
/// </summary>
/// <param name="Value">The string to be parsed.</param>
/// <param name="Culture">The culture used for comparing.</param>
/// <param name="Options">The options used for comparing.</param>
public sealed record StringParser(string Value, CultureInfo? Culture, CompareOptions Options) : IParser<string>
{
    private readonly bool useValue = Options is CompareOptions.Ordinal;

    /// <inheritdoc />
    public Type ResultType => typeof(string);

    /// <inheritdoc />
    public void Apply(IIterativeParseContext context, int position)
    {
        var input = context.Input;

        if (!Matches(input, position))
        {
            context.ResultStack.Push(new UnsafeParseResult(position));
        }
        else
        {
            context.ResultStack.Push(new(position, Value.Length, useValue ? Value : input.Substring(position, Value.Length)));
        }
    }

    /// <inheritdoc />
    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var input = context.Input;

        if (!Matches(input, position))
        {
            return new(position);
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
}
