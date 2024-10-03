namespace Warpstone.Parsers;

/// <summary>
/// Trait interface for parsers with a <see cref="Second"/> property.
/// </summary>
/// <typeparam name="T">The return type of the parser in the <see cref="Second"/> property.</typeparam>
public interface IParserSecond<out T>
{
    /// <summary>
    /// Gets the parser that is executed second.
    /// </summary>
    public IParser<T> Second { get; }
}

/// <summary>
/// Trait interface for parsers with a <see cref="IParserFirst{TFirst}.First"/>
/// and <see cref="IParserSecond{TSecond}.Second"/> property.
/// </summary>
/// <typeparam name="TFirst">
/// The return type of the parser in the
/// <see cref="IParserFirst{TFirst}.First"/> property.
/// </typeparam>
/// <typeparam name="TSecond">
/// The return type of the parser in the
/// <see cref="IParserSecond{TSecond}.Second"/> property.
/// </typeparam>
public interface IParserSecond<out TFirst, out TSecond> : IParserFirst<TFirst>, IParserSecond<TSecond>
{
}
