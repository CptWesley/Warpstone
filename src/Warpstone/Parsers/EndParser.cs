using System.Threading;
using Warpstone.ParseState;

namespace Warpstone.Parsers;

/// <summary>
/// Provides a parser which only matches the last input in the string.
/// </summary>
public class EndParser : Parser<string>
{
    /// <inheritdoc/>
    protected override IParseResult<string> InternalTryMatch(IParseState state, int position, int maxLength, CancellationToken cancellationToken)
    {
        string input = state.Unit.Input;
        if (maxLength <= 0)
        {
            return new ParseResult<string>(this, string.Empty, input, position, 0, EmptyResults);
        }

        return new ParseResult<string>(this, new UnexpectedTokenError(new SourcePosition(input, position, 1), new string[] { string.Empty }, GetFound(input, position)), EmptyResults);
    }

    /// <inheritdoc/>
    protected override string InternalToString(int depth)
        => "EOF()";
}
