using Warpstone;

namespace ClausewitzLsp.Core.Parsing;

/// <summary>
/// Base class for all parsed units.
/// </summary>
/// <typeparam name="T">The actual type that was parsed.</typeparam>
public abstract record ParseOption<T> : IParsed
{
    /// <summary>
    /// Gets or sets position of the result.
    /// </summary>
    public SourcePosition Position { get; set; } = default!;
}

/// <summary>
/// A parse result.
/// </summary>
/// <typeparam name="T">The actual type that was parsed.</typeparam>
/// <param name="Result">The parsed result.</param>
public record ParseSome<T>(T Result) : ParseOption<T>
{
    /// <inheritdoc/>
    public override string ToString()
        => Result is null ? "<null>" : Result.ToString()!;
}

/// <summary>
/// A parse failure.
/// </summary>
/// <typeparam name="T">The actual type that was parsed.</typeparam>
/// <param name="Error">The parse error.</param>
public record ParseNone<T>(IParseError Error) : ParseOption<T>
{
    /// <inheritdoc/>
    public override string ToString()
        => $"<{Error.GetMessage()}>";
}