namespace Warpstone.Grammars;

internal sealed class StartWithString<TKind>(string str, TKind kind)
    : Grammar<TKind>
    where TKind : struct, Enum
{
    private readonly string String = str;
    private readonly TKind Kind = kind;

    /// <inheritdoc />
    [Pure]
    public override Tokenizer<TKind> Match(Tokenizer<TKind> tokenizer)
        => tokenizer.Match(s => s.StartsWith(String), Kind);

    /// <inheritdoc />
    [Pure]
    public override string ToString() => Kind.Equals(default(TKind))
        ? $@"string({String})"
        : $@"string({String}, {Kind})";
}
