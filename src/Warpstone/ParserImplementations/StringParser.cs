namespace Warpstone.ParserImplementations;

/// <summary>
/// Represents a parser that parses an exact string.
/// </summary>
/// <param name="Value">The string to be parsed.</param>
public sealed record StringParser(string Value) : IParser<string>
{
    /// <inheritdoc />
    public Type ResultType => typeof(string);

    /// <inheritdoc />
    public void Apply(IIterativeParseContext context, int position)
    {
        var endPos = position + Value.Length;

        if (endPos > context.Input.Length)
        {
            context.ResultStack.Push(new UnsafeParseResult(position));
            return;
        }

        for (var i = 0; i < Value.Length; i++)
        {
            if (Value[i] != context.Input[position + i])
            {
                context.ResultStack.Push(new UnsafeParseResult(position));
                return;
            }
        }

        context.ResultStack.Push(new UnsafeParseResult(position, Value.Length, Value));
    }

    /// <inheritdoc />
    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var input = context.Input;
        var endPos = position + Value.Length;

        if (endPos > input.Length)
        {
            return new UnsafeParseResult(position);
        }

        for (var i = 0; i < Value.Length; i++)
        {
            if (Value[i] != input[position + i])
            {
                return new UnsafeParseResult(position);
            }
        }

        return new UnsafeParseResult(position, Value.Length, Value);
    }
}
