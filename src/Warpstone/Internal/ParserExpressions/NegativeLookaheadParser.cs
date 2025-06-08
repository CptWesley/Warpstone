namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Parser which represents a positive lookahead (not). Which does not consume any length of the input.
/// </summary>
/// <typeparam name="T">The type of the parser used to peek forward.</typeparam>
/// <param name="Parser">The parser used to peek forward.</param>
public sealed class NegativeLookaheadParser<T>(IParser<T> Parser) : IParser<T?>
{
    /// <inheritdoc />
    public Type ResultType => typeof(T);
}
