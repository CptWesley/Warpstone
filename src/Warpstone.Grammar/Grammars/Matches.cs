namespace Warpstone.Grammars;

internal sealed class Matches<TKind>(Predicate<char> predicate, TKind kind)
    : Grammar<TKind>
    where TKind : struct, Enum
{
    private readonly Predicate<char> Predicate = predicate;
    private readonly TKind Kind = kind;

    /// <inheritdoc />
    [Pure]
    public override Tokenizer<TKind> Match(Tokenizer<TKind> tokenizer)
        => tokenizer.Match(s => s.Matches(Predicate), Kind);

    /// <inheritdoc />
    [Pure]
    public override string ToString() => Kind.Equals(default(TKind))
        ? $@"match({Predicate})"
        : $@"match({Predicate}, {Kind})";
}
