using Warpstone;

namespace FluentAssertions;

internal static class WarpstoneAssertions
{
    [Pure]
    public static TokenizerAssertions<TKind> Should<TKind>(this Tokenizer<TKind> tokenizer)
       where TKind : struct, Enum
       => new(tokenizer);
}
