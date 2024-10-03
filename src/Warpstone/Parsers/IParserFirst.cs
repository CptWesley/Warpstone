namespace Warpstone.Parsers;

/// <summary>
/// Trait interface for parsers with a <see cref="First"/> property.
/// </summary>
/// <typeparam name="T">The return type of the parser in the <see cref="First"/> property.</typeparam>
public interface IParserFirst<out T>
{
    /// <summary>
    /// Gets the parser that is executed first.
    /// </summary>
    public IParser<T> First { get; }
}
