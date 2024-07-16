namespace Warpstone.Grammars;

internal sealed class Or<TKind>(Grammar<TKind> left, Grammar<TKind> right)
    : Grammar<TKind>
    where TKind : struct, Enum
{
    private readonly Grammar<TKind> Left = left;
    private readonly Grammar<TKind> Right = right;

    /// <inheritdoc />
    [Pure]
    public override Tokenizer<TKind> Match(Tokenizer<TKind> tokenizer) => tokenizer switch
    {
        _ when Left.Match(tokenizer) is { State: not Matching.NoMatch } next => next,
        _ when Right.Match(tokenizer) is { State: not Matching.NoMatch } next => next,
        _ => tokenizer.NoMatch(),
    };

    /// <inheritdoc />
    [Pure]
    public override string ToString() => $"( {Left} | {Right} )";
}
