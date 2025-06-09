namespace Warpstone.Internal.ParserImplementations;

/// <summary>
/// Represents a parser that always fails.
/// </summary>
/// <typeparam name="T">The result type of the parser.</typeparam>
internal sealed class FailParserImpl<T> : IParserImplementation<T>
{
    /// <summary>
    /// The singleton instance of the parser.
    /// </summary>
    public static readonly FailParserImpl<T> Instance = new();

    private FailParserImpl()
    {
    }

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
