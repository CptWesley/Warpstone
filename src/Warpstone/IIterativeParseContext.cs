namespace Warpstone;

/// <summary>
/// Represents the context necessary for parsing iteratively.
/// </summary>
public interface IIterativeParseContext
{
    /// <summary>
    /// The input string.
    /// </summary>
    public string Input { get; }

    /// <summary>
    /// The memo table.
    /// </summary>
    public IMemoTable MemoTable { get; }

    /// <summary>
    /// The result stack.
    /// </summary>
    public Stack<UnsafeParseResult> ResultStack { get; }

    /// <summary>
    /// The execution stack.
    /// </summary>
    public Stack<(int Position, IParser Parser)> ExecutionStack { get; }
}
