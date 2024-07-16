namespace Warpstone.Grammars;

internal sealed class RegularExpression<TKind>(Regex pattern, TKind kind, bool line)
    : Grammar<TKind>
    where TKind : struct, Enum
{
    internal RegularExpression(string pattern, TKind kind, bool line)
        : this(Regex(pattern), kind, line) { }

    private readonly Regex Pattern = pattern;
    private readonly TKind Kind = kind;
    private readonly bool Line = line;

    /// <inheritdoc />
    [Pure]
    public override Tokenizer<TKind> Match(Tokenizer<TKind> tokenizer)
        => Line
        ? tokenizer.Match(s => s.Line(Pattern), Kind)
        : tokenizer.Match(s => s.Regex(Pattern), Kind);

    /// <inheritdoc />
    [Pure]
    public override string ToString() => Kind.Equals(default(TKind)) switch
    {
        true => Line ? $"line({Formatted})" : $"regex({Formatted})",
        _ => Line ? $"line('{Formatted}', {Kind})" : $"regex('{Formatted}', {Kind})",
    };

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string Formatted => Pattern.ToString().TrimStart('^');

    private static Regex Regex(string regex) => regex[0] == '^'
        ? new(regex, Options, Timeout)
        : new('^' + regex, Options, Timeout);

    private static readonly RegexOptions Options = RegexOptions.CultureInvariant;

    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);
}
