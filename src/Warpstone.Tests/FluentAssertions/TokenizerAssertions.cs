using Microsoft.CodeAnalysis.Text;
using Warpstone;

namespace FluentAssertions;

internal sealed class TokenizerAssertions<TKind>(Tokenizer<TKind> subject) where TKind : struct, Enum
{
    public Tokenizer<TKind> Subject { get; } = subject;

    public void HaveTokenized(params Token<TKind>[] expected)
    {
        Subject.State.Should().Be(Matching.EoF, because: "Did not fully tokenized.");
        var tokens = Subject.Tokens.ToArray();
        tokens.Should().BeEquivalentTo(expected);
    }

    public void NotHaveTokenized(params Token<TKind>[] expected)
    {
        Subject.State.Should().Be(Matching.NoMatch, because: "Did not fully tokenized.");
        var tokens = Subject.Tokens.ToArray();
        tokens.Should().BeEquivalentTo(expected);
    }
}

internal readonly record struct Token<TKind>(TextSpan Span, string Text, TKind Kind) where TKind : struct, Enum;

internal static class Token
{
    public static Token<TKind> New<TKind>(int start, string text, TKind kind) where TKind : struct, Enum
        => new(new(start, text.Length), text, kind);
}
