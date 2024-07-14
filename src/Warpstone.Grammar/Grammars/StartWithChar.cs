namespace Warpstone.Grammars;

internal sealed class StartWithChar<TKind>(char ch, TKind kind)
    : Grammar<TKind>
    where TKind : struct, Enum
{
    private readonly char Char = ch;
    private readonly TKind Kind = kind;

    /// <inheritdoc />
    [Pure]
    public override Tokenizer<TKind> Match(Tokenizer<TKind> tokenizer)
        => tokenizer.Match(s => s.StartsWith(Char), Kind);

    /// <inheritdoc />
    [Pure]
    public override string ToString() => Kind.Equals(default(TKind))
        ? $@"ch({Char})"
        : $@"ch({Char}, {Kind})";
}
