namespace Warpstone.ParserImplementations;

/// <summary>
/// Represents a parser that always fails.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed record FailParser<T> : IParser<T>
{
    /// <summary>
    /// The singleton instance of the parser.
    /// </summary>
    public static readonly FailParser<T> Instance = new();

    private FailParser()
    {
    }

    /// <inheritdoc />
    public Type ResultType => typeof(T);

    /// <inheritdoc />
    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        return new UnsafeParseResult(position, [new UnexpectedTokenError(context, this, position, 1, string.Empty)]);
    }

    /// <inheritdoc />
    public void Apply(IIterativeParseContext context, int position)
    {
        context.ResultStack.Push(new UnsafeParseResult(position, [new UnexpectedTokenError(context, this, position, 1, string.Empty)]));
    }
}
