using System;
using System.Text.RegularExpressions;
using System.Threading;
using Warpstone.ParseState;

namespace Warpstone.Parsers;

/// <summary>
/// Represents a parser which parser a single <see cref="string"/>.
/// </summary>
public class StringParser : Parser<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringParser"/> class.
    /// </summary>
    /// <param name="str">The string to be parsed.</param>
    /// <param name="stringComparison">The string comparison options to be used.</param>
    public StringParser(string str, StringComparison stringComparison)
        => (String, StringComparison) = (str, stringComparison);

    /// <summary>
    /// Gets the string to be parsed.
    /// </summary>
    public string String { get; }

    /// <summary>
    /// Gets the string comparison to be used.
    /// </summary>
    public StringComparison StringComparison { get; }

    /// <inheritdoc/>
    public override IParseResult<string> Eval(IParseState state, int position, int maxLength, IRecursionParser recurse, CancellationToken cancellationToken)
    {
        string input = state.Unit.Input;
        if (maxLength < String.Length || string.Compare(input, position, String, 0, String.Length, StringComparison) != 0)
        {
            return new ParseResult<string>(this, new UnexpectedTokenError(new SourcePosition(input, position, 0), new string[] { $"'{String}'" }, GetFound(input, position, String.Length)), position);
        }

        return new ParseResult<string>(this, String, new SourcePosition(input, position, String.Length), position + String.Length);
    }

    /// <inheritdoc/>
    protected override string InternalToString(int depth)
        => $"String(\"{Regex.Escape(String.ToString())}\")";
}
