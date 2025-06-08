namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Represents a parser that lazily executed the given <paramref name="Element"/> parser.
/// </summary>
/// <typeparam name="T">The result type of the <paramref name="Element"/> parser.</typeparam>
/// <param name="Element">The lazily executed parser.</param>
internal sealed class LazyParser<T>(Lazy<IParser<T>> Element) : IParser<T>
{
    /// <inheritdoc />
    public Type ResultType => typeof(T);

    /// <summary>
    /// Initializes a new instance of the <see cref="LazyParser{T}"/> class.
    /// </summary>
    /// <param name="get">The generator for the lazy parser.</param>
    public LazyParser(Func<IParser<T>> get)
        : this(new Lazy<IParser<T>>(get))
    {
    }
}
