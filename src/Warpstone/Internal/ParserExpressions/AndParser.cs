namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Represents a parser that performs two parse operations sequentially and combines the result.
/// </summary>
/// <typeparam name="TFirst">The result type of the <paramref name="First"/> parser.</typeparam>
/// <typeparam name="TSecond">The result type of the <paramref name="Second"/> parser.</typeparam>
/// <param name="First">The parser that is executed first.</param>
/// <param name="Second">The parser that is executed after the first one has succeeded.</param>
internal sealed class AndParser<TFirst, TSecond>(IParser<TFirst> First, IParser<TSecond> Second) : IParser<(TFirst First, TSecond Second)>
{
    /// <inheritdoc />
    public Type ResultType => typeof((TFirst, TSecond));
}
