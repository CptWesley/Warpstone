namespace Warpstone.Internal.ParserImplementations;

/// <summary>
/// Parser used for detecting the end of the input stream.
/// </summary>
internal sealed class EndParserImpl : IParserImplementation<string>
{
    /// <summary>
    /// Singleton instance of the parser.
    /// </summary>
    public static readonly EndParserImpl Instance = new();

    private EndParserImpl()
    {
    }

    /// <inheritdoc />
    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        if (position >= context.Input.Content.Length)
        {
            return new(position, 0, string.Empty);
        }
        else
        {
            return new(position, [new UnexpectedTokenError(context, this, position, 1, "EOF")]);
        }
    }

    /// <inheritdoc />
    public void Apply(IIterativeParseContext context, int position)
    {
        if (position >= context.Input.Content.Length)
        {
            context.ResultStack.Push(new(position, 0, string.Empty));
        }
        else
        {
            context.ResultStack.Push(new(position, [new UnexpectedTokenError(context, this, position, 1, "EOF")]));
        }
    }
}
