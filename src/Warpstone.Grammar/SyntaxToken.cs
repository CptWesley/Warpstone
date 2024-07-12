namespace Warpstone;

/// <summary>Represents a syntax token.</summary>
/// <typeparam name="TKind">
/// The kind of syntax tokens.
/// </typeparam>
[DebuggerDisplay("{Span} {Text}, {Kind}")]
public readonly struct SyntaxToken<TKind>(SourceSpan sourceSpan, TKind kind)
{
    /// <summary>The (selected) source span.</summary>
    public readonly SourceSpan SourceSpan = sourceSpan;

    /// <summary>The token kind.</summary>
    public readonly TKind Kind = kind;

    /// <summary>The span of the token.</summary>
    public TextSpan Span => SourceSpan.Span;

    /// <summary>The text of the token.</summary>
    public string Text => SourceSpan.Text;

    /// <inheritdoc />
    [Pure]
    public override string ToString() => Text;
}
