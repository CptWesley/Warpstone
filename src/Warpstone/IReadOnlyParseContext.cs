namespace Warpstone;

/// <summary>
/// Provides read-only access to the parsing context.
/// </summary>
public interface IReadOnlyParseContext
{
    /// <summary>
    /// The used parser.
    /// </summary>
    public IParser Parser { get; }

    /// <summary>
    /// The provided input.
    /// </summary>
    public IParseInput Input { get; }

    /// <summary>
    /// The used memo-table.
    /// </summary>
    public IReadOnlyMemoTable MemoTable { get; }

    /// <summary>
    /// Indicates whether the parsing has finished.
    /// </summary>
    public bool Done { get; }

    /// <summary>
    /// The result of the parsing.
    /// Throws an <see cref="InvalidOperationException"/> if
    /// the parsing has not yet finished.
    /// </summary>
    public IParseResult Result { get; }
}

/// <summary>
/// Provides read-only access to the parsing context.
/// </summary>
/// <typeparam name="T">The output type.</typeparam>
public interface IReadOnlyParseContext<out T> : IReadOnlyParseContext
{
    /// <summary>
    /// The used parser.
    /// </summary>
    public new IParser<T> Parser { get; }

    /// <summary>
    /// The result of the parsing.
    /// Throws an <see cref="InvalidOperationException"/> if
    /// the parsing has not yet finished.
    /// </summary>
    public new IParseResult<T> Result { get; }
}
