namespace Warpstone.Parsers;

/// <summary>
/// Trait interface for parsers with a <see cref="Third"/> property.
/// </summary>
/// <typeparam name="T">The return type of the parser in the <see cref="Second"/> property.</typeparam>
public interface IParserThird<out T>
{
    /// <summary>
    /// Gets the parser that is executed third.
    /// </summary>
    public IParser<T> Third { get; }
}

/// <summary>
/// Trait interface for parsers with a <see cref="IParserFirst{TFirst}.First"/>,
/// <see cref="IParserSecond{TSecond}.Second"/> and
/// <see cref="IParserThird{TThird}.Third"/> property.
/// </summary>
/// <typeparam name="TFirst">
/// The return type of the parser in the
/// <see cref="IParserFirst{TFirst}.First"/> property.
/// </typeparam>
/// <typeparam name="TSecond">
/// The return type of the parser in the
/// <see cref="IParserSecond{TSecond}.Second"/> property.
/// </typeparam>
/// <typeparam name="TThird">
/// The return type of the parser in the
/// <see cref="IParserThird{TThird}.Third"/> property.
/// </typeparam>
public interface IParserThird<out TFirst, out TSecond, out TThird> : IParserSecond<TFirst, TSecond>, IParserThird<TThird>
{
}
