namespace Legacy.Warpstone2.Parsers;

/// <summary>
/// Trait interface for parsers with a <see cref="Value"/> property.
/// </summary>
/// <typeparam name="T">The type of the <see cref="Value"/> property.</typeparam>
public interface IParserValue<out T>
{
    /// <summary>
    /// Gets the value of the parser.
    /// </summary>
    public T Value { get; }
}
