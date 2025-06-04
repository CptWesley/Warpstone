namespace Warpstone.ParserImplementations;

/// <summary>
/// Parser used for detecting the end of the input stream.
/// </summary>
public sealed record EndParser : IParser<string>
{
    /// <summary>
    /// Singleton instance of the parser.
    /// </summary>
    public static readonly EndParser Instance = new();

    private EndParser()
    {
    }

    /// <inheritdoc />
    public Type ResultType => typeof(string);

    /// <inheritdoc />
    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        if (position >= context.Input.Length)
        {
            return new(position, 0, string.Empty);
        }
        else
        {
            return new(position);
        }
    }

    /// <inheritdoc />
    public void Apply(IIterativeParseContext context, int position)
    {
        if (position >= context.Input.Length)
        {
            context.ResultStack.Push(new(position, 0, string.Empty));
        }
        else
        {
            context.ResultStack.Push(new(position));
        }
    }
}
