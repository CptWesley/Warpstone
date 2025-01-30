namespace Warpstone;

/// <summary>
/// Represents an internal untyped, unsafe parse result.
/// </summary>
public readonly struct UnsafeParseResult
{
    /// <summary>
    /// The position of the result.
    /// </summary>
    public readonly int Position;

    /// <summary>
    /// The length of the result.
    /// </summary>
    public readonly int Length;

    /// <summary>
    /// The next position to parse.
    /// </summary>
    public int NextPosition => Position + Length;

    /// <summary>
    /// Indicates whether or not the result was succesful.
    /// </summary>
    public readonly bool Success;

    /// <summary>
    /// The parsed value if successful; an error otherwise.
    /// </summary>
    public readonly object? Value;
}

/// <summary>
/// Provides extension methods for dealing with <see cref="UnsafeParseResult"/> instances.
/// </summary>
public static class UnsafeParseResultExtensions
{
    /// <summary>
    /// Converts the given unsafe <paramref name="result"/> to a safe variant.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The unsafe result.</param>
    /// <param name="context">The parsing context.</param>
    /// <returns>The safe result.</returns>
    public static ParseResult<T> AsSafe<T>(this in UnsafeParseResult result, IParseContext<T> context)
    {
        if (result.Success)
        {
            return new ParseResult<T>(
                context: context,
                position: result.Position,
                length: result.Length,
                success: result.Success,
                value: (T)result.Value!,
                errors: Array.Empty<IParseError>());
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}
