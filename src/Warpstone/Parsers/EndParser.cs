using System.Threading;
using Warpstone.ParseState;

namespace Warpstone.Parsers;

/// <summary>
/// Provides a parser which only matches the last input in the string.
/// </summary>
public class EndParser : Parser<string>
{
    /// <inheritdoc/>
    public override IParseResult<string> Eval(IParseState state, int position, int maxLength, IRecursionParser recurse, CancellationToken cancellationToken)
    {
        string input = state.Unit.Input;
        if (maxLength <= 0)
        {
            return new ParseResult<string>(this, string.Empty, new SourcePosition(input, position, 0), position);
        }

        return new ParseResult<string>(this, new UnexpectedTokenError(new SourcePosition(input, position, 1), new string[] { string.Empty }, GetFound(input, position, 1)), position);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is EndParser;

    /// <inheritdoc/>
    public override int GetHashCode()
        => 9237671646.GetHashCode();

    /// <inheritdoc/>
    protected override string InternalToString(int depth)
        => "EOF()";
}
