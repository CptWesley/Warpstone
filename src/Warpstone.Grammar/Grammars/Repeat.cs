namespace Warpstone.Grammars;

internal sealed class Repeat<TKind>(Grammar<TKind> grammar, int min, int max)
    : Grammar<TKind>
    where TKind : struct, Enum
{
    private readonly Grammar<TKind> Grammar = grammar;

    private readonly int Minimum = min;

    private readonly int Maximum = max;

    /// <inheritdoc />
    [Pure]
    public override Tokenizer<TKind> Match(Tokenizer<TKind> tokenizer)
    {
        var i = 0;
        var prev = tokenizer;
        var next = tokenizer;
        var pos = -1;

        while (next.State == Matching.Match
            && i <= Maximum
            && next.SourceSpan.Start != pos)
        {
            prev = next;
            next = Grammar.Match(prev);
            i += next.State == Matching.NoMatch ? 0 : 1;
            pos = next.SourceSpan.Start;
        }

        var inRange = (i >= Minimum && i <= Maximum)
            // We have infinite matches with length 0.
            || (next.State == Matching.EoF && Grammar.Match(next).State == Matching.EoF);

        return next.State switch
        {
            _ when !inRange => tokenizer.NoMatch(),
            Matching.NoMatch => prev,
            _ => next,
        };
    }

    /// <inheritdoc />
    [Pure]
    public override string ToString() => Minimum switch
    {
        _ when Minimum == 0 && Maximum == 0 => $"~{Grammar}",
        _ when Minimum == 0 && Maximum == 1 => $"{Grammar}?",
        _ when Minimum == 0 && Maximum == int.MaxValue => $"{Grammar}*",
        _ when Minimum == 1 && Maximum == int.MaxValue => $"{Grammar}+",
        _ when Minimum == Maximum => $"{Grammar}[{Minimum}]",
        _ when Maximum == int.MaxValue => $"{Grammar}[{Minimum},]",
        _ => $"({Grammar})[{Minimum},{Maximum}]",
    };
}
