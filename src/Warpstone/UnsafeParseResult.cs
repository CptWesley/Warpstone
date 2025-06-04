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

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsafeParseResult"/> struct.
    /// </summary>
    /// <param name="position">The position where the error occurred.</param>
    public UnsafeParseResult(int position)
    {
        Position = position;
        Length = 0;
        Value = null;
        Success = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsafeParseResult"/> struct.
    /// </summary>
    /// <param name="position">The position where the result was obtained.</param>
    /// <param name="length">The length of the value in the input.</param>
    /// <param name="value">The found value.</param>
    public UnsafeParseResult(int position, int length, object? value)
    {
        Position = position;
        Length = length;
        Value = value;
        Success = true;
    }
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
