namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Represents a parser that parses a character.
/// </summary>
/// <param name="Value">The character to be parsed.</param>
internal sealed class CharacterParser(char Value) : IParser<char>
{
    /// <inheritdoc />
    public Type ResultType => typeof(char);
}
