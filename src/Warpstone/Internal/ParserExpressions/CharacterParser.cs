namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Represents a parser that parses a character.
/// </summary>
internal sealed class CharacterParser : ParserBase<char>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterParser"/> class.
    /// </summary>
    /// <param name="character">The character to be parsed.</param>
    public CharacterParser(char character)
    {
        Character = character;
    }

    /// <summary>
    /// The character to be parsed.
    /// </summary>
    public char Character { get; }
}
