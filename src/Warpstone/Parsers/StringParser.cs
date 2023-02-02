﻿using System;
using System.Text.RegularExpressions;
using System.Threading;

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
    protected override IParseResult<string> InternalTryMatch(string input, int position, int maxLength, IMemoTable memoTable, CancellationToken cancellationToken)
    {
        if (maxLength < String.Length || string.Compare(input, position, String, 0, String.Length, StringComparison) != 0)
        {
            return new ParseResult<string>(this, new UnexpectedTokenError(new SourcePosition(input, position, 1), new string[] { $"'{String}'" }, GetFound(input, position)), EmptyResults);
        }

        return new ParseResult<string>(this, String, input, position, position + String.Length, EmptyResults);
    }

    /// <inheritdoc/>
    protected override string InternalToString(int depth)
        => $"String(\"{Regex.Escape(String.ToString())}\")";
}