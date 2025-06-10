namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Represents a parser that parses a character.
/// </summary>
internal sealed class CharacterParser : ParserBase<char>
{
    private static readonly int baseHash = typeof(CharacterParser).GetHashCode() * 31;

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

    /// <inheritdoc />
    public override IParserImplementation<char> CreateUninitializedImplementation()
        => new CharacterParserImpl(Character);

    /// <inheritdoc />
    protected override void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
    {
        // Do nothing.
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is CharacterParser other
        && other.Character == Character;

    /// <inheritdoc />
    public override int GetHashCode()
        => baseHash + Character.GetHashCode();
}
