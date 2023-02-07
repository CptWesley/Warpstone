using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Threading;
using Warpstone.ParseState;

namespace Warpstone.Parsers;

/// <summary>
/// Represents a parser which parser a regular expression pattern.
/// </summary>
public class RegexParser : Parser<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegexParser"/> class.
    /// </summary>
    /// <param name="pattern">The regular expression pattern to be parsed.</param>
    /// <param name="compiled">Indicates whether or not the regular expressions should be compiled.</param>
    public RegexParser([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, bool compiled)
    {
        Pattern = pattern;
        RegexOptions options = RegexOptions.ExplicitCapture;
        if (compiled)
        {
            options |= RegexOptions.Compiled;
        }

        Regex = new Regex(@$"\G({pattern})", options);
    }

    /// <summary>
    /// Gets the string to be parsed.
    /// </summary>
    public string Pattern { get; }

    /// <summary>
    /// Gets the regular expression instance.
    /// </summary>
    public Regex Regex { get; }

    /// <inheritdoc/>
    public override IParseResult<string> Eval(IParseState state, int position, int maxLength, IRecursionParser recurse, CancellationToken cancellationToken)
    {
        string input = state.Unit.Input;

        if (position > input.Length)
        {
            return new ParseResult<string>(this, new UnexpectedTokenError(new SourcePosition(input, position, 0), new string[] { $"'{Pattern}'" }, GetFound(input, position, 1)), position);
        }

        Match match = Regex.Match(input, position, maxLength);

        if (!match.Success || match.Index != position)
        {
            return new ParseResult<string>(this, new UnexpectedTokenError(new SourcePosition(input, position, 0), new string[] { $"'{Pattern}'" }, GetFound(input, position, 1)), position);
        }

        return new ParseResult<string>(this, match.Value, new SourcePosition(input, position, match.Length), position + match.Length);
    }

    /// <inheritdoc/>
    protected override string InternalToString(int depth)
        => $"Regex(\"{Regex.Escape(Pattern)}\")";
}
