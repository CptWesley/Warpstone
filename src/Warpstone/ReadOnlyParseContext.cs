namespace Warpstone;

/// <summary>
/// Used for wrapping parse contexts to prevent casting.
/// </summary>
public class ReadOnlyParseContext : IReadOnlyParseContext
{
    private readonly IReadOnlyParseContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyParseContext"/> class.
    /// </summary>
    /// <param name="context">The wrapped parse context.</param>
    public ReadOnlyParseContext(IReadOnlyParseContext context)
    {
        this.context = context;
    }

    /// <inheritdoc />
    public IParser Parser => context.Parser;

    /// <inheritdoc />
    public IParseInput Input => context.Input;

    /// <inheritdoc />
    public IReadOnlyMemoTable MemoTable => context.MemoTable;

    /// <inheritdoc />
    public bool Done => context.Done;

    /// <inheritdoc />
    public IParseResult Result => context.Result;

    /// <inheritdoc />
    public IReadOnlyParseStack Stack => context.Stack;
}

/// <summary>
/// Used for wrapping parse contexts to prevent casting.
/// </summary>
/// <typeparam name="T">The type of the parse result.</typeparam>
public class ReadOnlyParseContext<T> : ReadOnlyParseContext, IReadOnlyParseContext<T>
{
    private readonly IReadOnlyParseContext<T> context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyParseContext{T}"/> class.
    /// </summary>
    /// <param name="context">The wrapped parse context.</param>
    public ReadOnlyParseContext(IReadOnlyParseContext<T> context)
        : base(context)
    {
        this.context = context;
    }

    /// <inheritdoc />
    public new IParser<T> Parser => context.Parser;

    /// <inheritdoc />
    public new IParseResult<T> Result => context.Result;
}
