namespace Warpstone.Grammars;

internal sealed class And<TKind>(Grammar<TKind> left, Grammar<TKind> right)
    : Grammar<TKind>
    where TKind : struct, Enum
{
    private readonly Grammar<TKind> Left = left;
    private readonly Grammar<TKind> Right = right;

    /// <inheritdoc />
    [Pure]
    public override Tokenizer<TKind> Match(Tokenizer<TKind> tokenizer)
        => Left.Match(tokenizer) is { State: not Matching.NoMatch } first
        && Right.Match(first) is { State: not Matching.NoMatch } second
        ? second
        : tokenizer.NoMatch();

    /// <inheritdoc />
    [Pure]
    public override string ToString() => $"({Left} & {Right})";
}
