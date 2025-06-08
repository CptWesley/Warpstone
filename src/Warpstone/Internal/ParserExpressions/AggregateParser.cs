namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Represents a parser that parses a repeated series of elements and aggregates the results.
/// </summary>
/// <typeparam name="TSource">The element type.</typeparam>
/// <typeparam name="TAccumulator">The accumulator type.</typeparam>
/// <param name="Element">The element parser.</param>
/// <param name="Delimiter">The optional delimiter parser.</param>
/// <param name="MinCount">The minimum number of parsed elements.</param>
/// <param name="MaxCount">The maximum number of parsed elements.</param>
/// <param name="CreateSeed">The function to create the initial value of the accumulator.</param>
/// <param name="Accumulate">The accumulation function.</param>
internal sealed class AggregateParser<TSource, TAccumulator>(
    IParser<TSource> Element,
    IParser? Delimiter,
    int MinCount,
    int MaxCount,
    Func<TAccumulator> CreateSeed,
    Func<TAccumulator, TSource, TAccumulator> Accumulate) : IParser<TAccumulator>
{
    /// <inheritdoc />
    public Type ResultType => typeof(TAccumulator);
}
