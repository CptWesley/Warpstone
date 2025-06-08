namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Represents a parser that can override the expected token message.
/// </summary>
/// <typeparam name="T">The result type of the wrapped parser.</typeparam>
/// <param name="Parser">The wrapped parser.</param>
/// <param name="Expected">The expected string.</param>
internal sealed class ExpectedParser<T>(IParser<T> Parser, string Expected) : ParserBase<T>
{
}
