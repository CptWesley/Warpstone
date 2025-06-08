namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Represents a parser that unwraps the result as a <see cref="IParseResult{T}"/> during the parsing.
/// </summary>
/// <typeparam name="T">The type of the result value.</typeparam>
/// <param name="Parser">The internal parser.</param>
internal sealed class AsResultParser<T>(IParser<T> Parser) : IParser<IParseResult<T>>
{
    /// <inheritdoc />
    public Type ResultType => typeof(IParseResult<T>);
}
