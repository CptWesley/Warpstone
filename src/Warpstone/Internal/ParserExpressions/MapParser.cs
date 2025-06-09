namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Represents a parser that converts the value of the given <see name="Element"/> parser
/// using the provided <see name="Map"/> function.
/// </summary>
/// <typeparam name="TIn">The result type of the <see name="Element"/> parser.</typeparam>
/// <typeparam name="TOut">The result type of the <see name="Map"/> function.</typeparam>
internal sealed class MapParser<TIn, TOut> : ParserBase<TOut>
    where TIn : struct
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapParser{TIn,TOut}"/> class.
    /// </summary>
    /// <param name="element">The input parser.</param>
    /// <param name="map">The map function.</param>
    public MapParser(IParser<TIn> element, Func<TIn, TOut> map)
    {
        Element = element;
        Map = map;
    }

    /// <summary>
    /// The input parser.
    /// </summary>
    public IParser<TIn> Element { get; }

    /// <summary>
    /// The map function.
    /// </summary>
    public Func<TIn, TOut> Map { get; }
}
