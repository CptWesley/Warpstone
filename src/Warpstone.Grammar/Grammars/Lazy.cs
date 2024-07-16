namespace Warpstone.Grammars;

internal sealed class Lazy<TKind>(Func<Grammar<TKind>> factory) : Grammar<TKind> where TKind : struct, Enum
{
    private readonly Func<Grammar<TKind>> Factory = factory;

    private Grammar<TKind>? Grammar;

    /// <inheritdoc />
    [Pure]
    public override Tokenizer<TKind> Match(Tokenizer<TKind> tokenizer)
    {
        Grammar ??= Factory();
        return Grammar.Match(tokenizer);
    }

    /// <inheritdoc />
    [Pure]
    public override string ToString() => Grammar?.ToString() ?? "?";
}
