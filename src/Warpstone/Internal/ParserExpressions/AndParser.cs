namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Represents a parser that performs two parse operations sequentially and combines the result.
/// </summary>
/// <typeparam name="TFirst">The result type of the <see name="First"/> parser.</typeparam>
/// <typeparam name="TSecond">The result type of the <see name="Second"/> parser.</typeparam>
internal sealed class AndParser<TFirst, TSecond> : ParserBase<(TFirst First, TSecond Second)>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AndParser{TFirst,TSecond}"/> class.
    /// </summary>
    /// <param name="first">The parser that is executed first.</param>
    /// <param name="second">The parser that is executed after the first one has succeeded.</param>
    public AndParser(IParser<TFirst> first, IParser<TSecond> second)
    {
        First = first;
        Second = second;
    }

    /// <summary>
    /// The parser that is executed first.
    /// </summary>
    public IParser<TFirst> First { get; }

    /// <summary>
    /// The parser that is executed after the first one has succeeded.
    /// </summary>
    public IParser<TSecond> Second { get; }
}
