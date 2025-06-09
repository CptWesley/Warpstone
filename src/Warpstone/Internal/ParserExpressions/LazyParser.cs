namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Represents a parser that lazily executed the given <see name="Parser"/>.
/// </summary>
/// <typeparam name="T">The result type of the <see name="Parser"/>.</typeparam>
internal sealed class LazyParser<T> : ParserBase<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LazyParser{T}"/> class.
    /// </summary>
    /// <param name="parser">The lazily executed parser.</param>
    public LazyParser(Lazy<IParser<T>> parser)
    {
        Parser = parser;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LazyParser{T}"/> class.
    /// </summary>
    /// <param name="get">The generator for the lazy parser.</param>
    public LazyParser(Func<IParser<T>> get)
        : this(new Lazy<IParser<T>>(get))
    {
    }

    /// <summary>
    /// The lazily executed parser.
    /// </summary>
    public Lazy<IParser<T>> Parser { get; }
}
