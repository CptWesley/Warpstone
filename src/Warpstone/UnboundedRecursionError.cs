namespace Warpstone;

/// <summary>
/// Represents an error when unresolvable recursion was encountered.
/// </summary>
public class UnboundedRecursionError : ParseError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnboundedRecursionError"/> class.
    /// </summary>
    /// <param name="position">The start position.</param>
    public UnboundedRecursionError(SourcePosition position)
        : base(position)
    {
    }

    /// <inheritdoc/>
    protected override string GetSimpleMessage()
        => "Encountered unbounded recursion.";
}
