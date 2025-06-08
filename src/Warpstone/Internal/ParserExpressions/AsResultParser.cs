namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Represents a parser that unwraps the result as a <see cref="IParseResult{T}"/> during the parsing.
/// </summary>
/// <typeparam name="T">The type of the result value.</typeparam>
internal sealed class AsResultParser<T> : ParserBase<IParseResult<T>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsResultParser{T}"/> class.
    /// </summary>
    /// <param name="parser">The internal parser.</param>
    public AsResultParser(IParser<T> parser)
    {
        Parser = parser;
    }

    /// <summary>
    /// The internal parser.
    /// </summary>
    public IParser<T> Parser { get; }
}
