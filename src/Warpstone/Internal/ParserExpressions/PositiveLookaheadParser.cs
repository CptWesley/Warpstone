namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Parser which represents a positive lookahead (peek). Which does not consume any length of the input.
/// </summary>
/// <typeparam name="T">The type of the parser used to peek forward.</typeparam>
/// <param name="Parser">The parser used to peek forward.</param>
internal sealed class PositiveLookaheadParser<T>(IParser<T> Parser) : ParserBase<T>
{
}
