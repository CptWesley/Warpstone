namespace Warpstone.ParserImplementations;

/// <summary>
/// Represents a parser that always passes.
/// </summary>
/// <typeparam name="T">The type of the value that is always returned.</typeparam>
/// <param name="Value">The value that is always returned.</param>
internal sealed class CreateParser<T>(T Value) : IParserImplementation<T>
{
    /// <inheritdoc />
    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        return new(position, 0, Value);
    }

    /// <inheritdoc />
    public void Apply(IIterativeParseContext context, int position)
    {
        context.ResultStack.Push(new(position, 0, Value));
    }
}
