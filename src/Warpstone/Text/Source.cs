using System;
using System.Diagnostics.Contracts;

namespace Warpstone.Text;

/// <summary>Represents the full source text.</summary>
public readonly struct Source
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Source"/> struct with the specified text.
    /// </summary>
    /// <param name="text">
    /// The text of the source.
    /// </param>
    private Source(string text)
    {
        Text = text ?? string.Empty;
        TextSpan = new TextSpan(0, Text.Length);
    }

    /// <summary>The text of the source.</summary>
    public readonly string Text;

    /// <summary>The text span of the source.</summary>
    public readonly TextSpan TextSpan;

    /// <summary>The length of the full source.</summary>
    public int Length => Text.Length;

    /// <summary>A character at the specified position of the source.</summary>
    /// <param name="index">The index of the character.</param>
    public char this[int index] => Text[index];

    /// <inheritdoc />
    public override string ToString() => Text;

    /// <summary>Gets the string representation of the source text within the specified span.</summary>
    /// <param name="span">The span of text to extract.</param>
    /// <returns>
    /// a substring of the source text.
    /// </returns>
    [Pure]
    public string ToString(TextSpan span) => Text.Substring(span.Start, span.Length);

    /// <summary>Implicitly casts to a <see cref="SourceSpan"/>.</summary>
    /// <param name="source">The source to cast.</param>
    public static implicit operator SourceSpan(Source source) => new(source, source.TextSpan);

    /// <summary>Creates a new source.</summary>
    /// <param name="text">The text of the source.</param>
    /// <returns>
    /// A new source.
    /// </returns>
    [Pure]
    public static Source From(string text) => new(text);
}
