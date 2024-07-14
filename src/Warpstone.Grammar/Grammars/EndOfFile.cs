namespace Warpstone.Grammars;

internal sealed class EndOfFile<TKind> : Grammar<TKind> where TKind : struct, Enum
{
    /// <inheritdoc />
    [Pure]
    public override Tokenizer<TKind> Match(Tokenizer<TKind> tokenizer)
        => tokenizer.State == Matching.EoF
        ? tokenizer
        : tokenizer.NoMatch();

    /// <inheritdoc />
    [Pure]
    public override string ToString() => "eof";
}
