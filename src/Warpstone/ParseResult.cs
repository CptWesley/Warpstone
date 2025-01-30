namespace Warpstone;

/// <summary>
/// Represents the result of parsing.
/// </summary>
/// <typeparam name="T">The type of the values obtained in the results.</typeparam>
public sealed class ParseResult<T> : IParseResult<T>
{
    private readonly T? value;

    public ParseResult(
        IReadOnlyParseContext context,
        int position,
        int length,
        bool success,
        T? value,
        IReadOnlyList<IParseError> errors)
    {
        Context = context;
        Position = position;
        Length = length;
        Success = success;
        this.value = value;
        Errors = errors;
    }

    /// <inheritdoc />
    public T Value
    {
        get
        {
            ThrowIfUnsuccessful();
            return value!;
        }
    }

    /// <inheritdoc />
    public int Position { get; }

    /// <inheritdoc />
    public int NextPosition => Position + Length;

    /// <inheritdoc />
    public ParseInputPosition InputStartPosition => Context.Input.GetPosition(Position);

    /// <inheritdoc />
    public ParseInputPosition InputEndPosition => Context.Input.GetPosition(Length == 0 ? Position : NextPosition - 1);

    /// <inheritdoc />
    public bool Success { get; }

    /// <inheritdoc />
    public int Length { get; }

    /// <inheritdoc />
    public IReadOnlyList<IParseError> Errors { get; }

    /// <inheritdoc />
    object? IParseResult.Value => Value;

    /// <inheritdoc />
    public IReadOnlyParseContext Context { get; }

    /// <inheritdoc />
    public override string ToString()
        => Success
        ? Value?.ToString() ?? string.Empty
        : $"Error([{string.Join(", ", Errors)}])";

    /// <inheritdoc />
    public void ThrowIfUnsuccessful()
    {
        if (Success)
        {
            return;
        }

        var throwables = Errors.OfType<Exception>().ToArray();

        throw throwables.Length switch
        {
            0 => new InvalidOperationException("Can not read property Value of non-successful ParseResult."),
            1 => throwables[0],
            _ => new AggregateException(throwables),
        };
    }
}
