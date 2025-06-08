namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Represents a parser that converts the value of the given <paramref name="Element"/> parser
/// using the provided <paramref name="Map"/> function.
/// </summary>
/// <typeparam name="TIn">The result type of the <paramref name="Element"/> parser.</typeparam>
/// <typeparam name="TOut">The result type of the <paramref name="Map"/> function.</typeparam>
/// <param name="Element">The input parser.</param>
/// <param name="Map">The map function.</param>
internal sealed class MapParser<TIn, TOut>(IParser<TIn> Element, Func<TIn, TOut> Map) : ParserBase<TOut>
    where TIn : struct
{
}
