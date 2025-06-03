using Legacy.Warpstone2.IterativeExecution;

namespace Legacy.Warpstone2.Parsers.Internal;

/// <summary>
/// Parser which parses a single character.
/// </summary>
internal sealed class CharacterParser : ParserBase<char>, IEquatable<CharacterParser>, IParserValue<char>
{
    private readonly string expected;

    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterParser"/> class.
    /// </summary>
    /// <param name="value">The expected character.</param>
    public CharacterParser(char value)
    {
        Character = value;
        expected = $"'{value}'";
    }

    /// <summary>
    /// The expected character.
    /// </summary>
    public char Character { get; }

    /// <inheritdoc />
    char IParserValue<char>.Value => Character;

    /// <inheritdoc />
    public override IIterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IIterativeStep> eval)
    {
        var input = context.Input.Content;

        if (position >= input.Length)
        {
            return Iterative.Done(this.Mismatch(context, position, 1, expected));
        }

        if (input[position] == Character)
        {
            return Iterative.Done(this.Match(context, position, 1, Character));
        }
        else
        {
            return Iterative.Done(this.Mismatch(context, position, 1, expected));
        }
    }

    /// <inheritdoc />
    protected override string InternalToString(int depth)
        => expected;

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is CharacterParser other
        && Equals(other);

    /// <inheritdoc />
    public bool Equals(CharacterParser? other)
        => other is not null
        && Character == other.Character;

    /// <inheritdoc />
    public override int GetHashCode()
        => (typeof(CharacterParser), Character).GetHashCode();
}
