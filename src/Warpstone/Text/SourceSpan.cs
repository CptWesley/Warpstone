using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace Warpstone.Text;

/// <summary>Represents a span of a source text.</summary>
/// <param name="source">
/// The full source text.
/// </param>
/// <param name="textSpan">
/// The (selected) span of the source text.
/// </param>
public readonly struct SourceSpan(Source source, TextSpan textSpan) : IEquatable<SourceSpan>
{
    /// <remarks>Syntactic sugar that allows the use of NoMatch over null.</remarks>
    private static readonly TextSpan? NoMatch = null;

    /// <summary>The underlying source.</summary>
    public readonly Source Source = source;

    /// <summary>The (selected) text span.</summary>
    public readonly TextSpan TextSpan = textSpan;

    /// <summary>Gets the first character of the source span.</summary>
    public char First => Source[Start];

    /// <summary>The (selected) source text.</summary>
    public string Text => Source.ToString(TextSpan);

    /// <summary>A character at the specified position of the source.</summary>
    /// <param name="index">The index of the character.</param>
    public char this[int index] => Source.Text[Start + index];

    /// <summary>The start position.</summary>
    public int Start => TextSpan.Start;

    /// <summary>The end position.</summary>
    public int End => TextSpan.End;

    /// <summary>The length of the span.</summary>
    public int Length => TextSpan.Length;

    /// <summary>Indicates if the span is empty.</summary>
    public bool IsEmpty => TextSpan.IsEmpty;

    /// <summary>Indicates if the span is not empty.</summary>
    public bool HasValue => !TextSpan.IsEmpty;

    /// <summary>Exposes the text as <see cref="ReadOnlySpan{T}"/>.</summary>
    /// <returns>
    /// An read-only span of characters.
    /// </returns>
    [Pure]
    public ReadOnlySpan<char> AsSpan() => Source.Text.AsSpan(TextSpan.Start, TextSpan.Length);

    /// <summary>Trims the source span.</summary>
    /// <param name="span">
    /// The span to trim to.
    /// </param>
    /// <returns>
    /// A trimmed source span.
    /// </returns>
    public SourceSpan Trim(TextSpan span) => new(Source, span);

    /// <summary>Takes the number of specified characters from the start of this source span.</summary>
    /// <param name="length">The length of the source span.</param>
    /// <returns>
    /// A trimmed source span.
    /// </returns>
    [Pure]
    public SourceSpan Take(int length) => new(Source, new(Start, length));

    /// <summary>Trims the source span from the left.</summary>
    /// <param name="left">
    /// The number of characters to trim.
    /// </param>
    /// <returns>
    /// A trimmed source span.
    /// </returns>
    [Pure]
    public SourceSpan Skip(int left) => new(Source, new(Start + left, Length - left));

    /// <summary>Indicates that the text span starts with the specified character.</summary>
    /// <param name="ch">
    /// The character to match.
    /// </param>
    /// <returns>
    /// Null if no match, otherwise the matching text span.
    /// </returns>
    [Pure]
    public TextSpan? StartsWith(char ch)
    {
        var result = !TextSpan.IsEmpty && Source.Text[TextSpan.Start] == ch
            ? new TextSpan(TextSpan.Start, 1)
            : NoMatch;

        return result;
    }

    /// <summary>Indicates that the text span starts with the specified string.</summary>
    /// <param name="str">
    /// The string to match.
    /// </param>
    /// <returns>
    /// Null if no match, otherwise the matching text span.
    /// </returns>
    [Pure]
    public TextSpan? StartsWith(string str)
    {
        if (TextSpan.Length >= str.Length)
        {
            var pos = TextSpan.Start;

            for (var i = 0; i < str.Length; i++)
            {
                if (Source[pos++] != str[i])
                {
                    return NoMatch;
                }
            }

            return new TextSpan(TextSpan.Start, str.Length);
        }
        else
        {
            return NoMatch;
        }
    }

    /// <summary>Matches the predicate.</summary>
    /// <param name="match">
    /// The required match.
    /// </param>
    /// <returns>
    /// Null if no match, otherwise the matching text span.
    /// </returns>
    [Pure]
    public TextSpan? Predicate(Predicate<char> match)
    {
        if (IsEmpty)
        {
            return NoMatch;
        }

        var len = -1;
        var i = Start;

        while (++len < Length)
        {
            if (!match(Source[i++]))
            {
                return (len == 0)
                    ? NoMatch
                    : new(TextSpan.Start, len);
            }
        }

        return TextSpan;
    }

    /// <inheritdoc cref="Match(Regex)" />
    [Pure]
    public TextSpan? Match([StringSyntax(StringSyntaxAttribute.Regex)] string pattern) => Match(new Regex(pattern, RegexOptions.CultureInvariant));

    /// <summary>Reports a <see cref="Text.TextSpan"/> indicating the position of the match.</summary>
    /// <param name="pattern">The regular expression pattern to match.</param>
    /// <returns>
    /// Returns null if the character was not found.
    /// </returns>
    [Pure]
    public TextSpan? Match(Regex pattern)
        => pattern.Match(Source.Text, TextSpan.Start, TextSpan.Length) is { Success: true } match
        ? new TextSpan(match.Index, match.Length)
        : NoMatch;

    /// <summary>Reports a <see cref="Text.TextSpan"/> indicating the position of the character.</summary>
    /// <param name="ch">The character to get the index of.</param>
    /// <returns>
    /// Returns null if the character was not found.
    /// </returns>
    [Pure]
    public TextSpan? IndexOf(char ch)
    {
        var index = Source.Text.IndexOf(ch, Start, Length);
        return index == -1
            ? NoMatch
            : new(index, 1);
    }

    /// <inheritdoc />
    [Pure]
    public override string ToString() => Text;

    /// <inheritdoc />
    [Pure]
    public override bool Equals(object? obj) => obj is SourceSpan other && Equals(other);

    /// <inheritdoc />
    [Pure]
    public bool Equals(SourceSpan other)
        => TextSpan == other.TextSpan
        && Source.Text == other.Text;

    /// <inheritdoc />
    [Pure]
    public override int GetHashCode() => TextSpan.GetHashCode();

    /// <summary>Returns true if left and right are equal.</summary>
    /// <param name="left">
    /// Left operator.
    /// </param>
    /// <param name="right">
    /// Right operator.
    /// </param>
    public static bool operator ==(SourceSpan left, SourceSpan right) => left.Equals(right);

    /// <summary>Returns true if left and right are different.</summary>
    /// <param name="left">
    /// Left operator.
    /// </param>
    /// <param name="right">
    /// Right operator.
    /// </param>
    public static bool operator !=(SourceSpan left, SourceSpan right) => !(left == right);

    /// <summary>Trims the first character of the source span.</summary>
    /// <param name="span">The source span to trim.</param>
    public static SourceSpan operator ++(SourceSpan span) => span.Skip(1);
}
