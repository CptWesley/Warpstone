using System.Text.RegularExpressions;
using System.Threading;
using Warpstone.ParseState;

namespace Warpstone.Parsers;

/// <summary>
/// Represents a parser which parser a single <see cref="char"/>.
/// </summary>
public class CharParser : Parser<char>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CharParser"/> class.
    /// </summary>
    /// <param name="character">The character to be parsed.</param>
    public CharParser(char character)
        => Character = character;

    /// <summary>
    /// Gets the character to be parsed.
    /// </summary>
    public char Character { get; }

    /// <inheritdoc/>
    public override IParseResult<char> Eval(IParseState state, int position, int maxLength, IRecursionParser recurse, CancellationToken cancellationToken)
    {
        string input = state.Unit.Input;
        if (maxLength <= 0 || position >= input.Length || input[position] != Character)
        {
            return new ParseResult<char>(this, new UnexpectedTokenError(new SourcePosition(input, position, 1), new string[] { $"'{Regex.Escape(Character.ToString())}'" }, GetFound(input, position, 1)), position);
        }

        return new ParseResult<char>(this, Character, new SourcePosition(input, position, 1), position + 1);
    }

    /// <inheritdoc/>
    protected override string InternalToString(int depth)
        => $"Character('{Regex.Escape(Character.ToString())}')";
}
