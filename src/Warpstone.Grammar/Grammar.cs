#pragma warning disable SA1300 // Element should begin with upper-case letter
// Grammar is commonly defined with lowercase words.

namespace Warpstone;

/// <summary>A way of defining formal grammar.</summary>
/// <typeparam name="TKind">
/// The kind of tokens that are tokenized.
/// </typeparam>
public class Grammar<TKind> where TKind : struct, Enum
{
    /// <summary>A pattern do define token(s).</summary>
    /// <param name="tokenizer">
    /// The current tokenizer (state).
    /// </param>
    /// <returns>
    /// A new tokenizer with an updated state.
    /// </returns>
    public delegate Tokenizer<TKind> Tokens(Tokenizer<TKind> tokenizer);

    /// <summary>End of File.</summary>
    /// <param name="t">
    /// The current tokenizer (state).
    /// </param>
    /// <returns>
    /// A new tokenizer with an updated state.
    /// </returns>
    [Pure]
    public static Tokenizer<TKind> eof(Tokenizer<TKind> t)
        => t.State == Matching.EoF
        ? t
        : t.NoMatch();

    /// <summary>Successfully matches if the pattern does not.</summary>
    /// <param name="pattern">
    /// The pattern to match.
    /// </param>
    /// <returns>
    /// A new tokenizer with an updated state.
    /// </returns>
    [Pure]
    public static Tokens Not(Tokens pattern) =>
        t => pattern(t).State == Matching.Match ? t.NoMatch() : t;

    /// <summary>Successfully matches, if the pattern does or not.</summary>
    /// <param name="pattern">
    /// The pattern to match.
    /// </param>
    /// <returns>
    /// A new tokenizer with an updated state.
    /// </returns>
    [Pure]
    public static Tokens Option(Tokens pattern) =>
       t => Option(t, pattern);

    /// <summary>Successfully matches, if the pattern matches multiple times.</summary>
    /// <param name="pattern">
    /// The pattern to match.
    /// </param>
    /// <param name="min">
    /// The minimum number of repetitions (default; 0).
    /// </param>
    /// <param name="max">
    /// The maximum number of repetitions (default: unlimited).
    /// </param>
    /// <returns>
    /// A new tokenizer with an updated state.
    /// </returns>
    [Pure]
    public static Tokens Repeat(Tokens pattern, int min = 0, int max = int.MaxValue) =>
       t => Repeat(t, pattern, min, max);

    /// <summary>Matches if the remaining source starts with the specified character.</summary>
    /// <param name="ch">
    /// The expected character.
    /// </param>
    /// <param name="kind">
    /// The (optional) token kind.
    /// </param>
    /// <returns>
    /// A new tokenizer with an updated state.
    /// </returns>
    [Pure]
    public static Tokens ch(char ch, TKind kind = default) => t
        => t.Match(s => s.StartsWith(ch), kind);

    /// <summary>Matches if the remaining source starts with the specified string.</summary>
    /// <param name="str">
    /// The expected string.
    /// </param>
    /// <param name="kind">
    /// The (optional) token kind.
    /// </param>
    /// <returns>
    /// A new tokenizer with an updated state.
    /// </returns>
    [Pure]
    public static Tokens str(string str, TKind kind = default) => t
        => t.Match(s => s.StartsWith(str), kind);

    /// <inheritdoc cref="line(System.Text.RegularExpressions.Regex, TKind)"/>
    [Pure]
    public static Tokens line([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, TKind kind = default)
        => line(Regex(pattern), kind);

    /// <summary>Matches if the remaining source starts with the specified pattern.</summary>
    /// <param name="pattern">
    /// The expected pattern.
    /// </param>
    /// <param name="kind">
    /// The (optional) token kind.
    /// </param>
    /// <returns>
    /// A new tokenizer with an updated state.
    /// </returns>
    /// <remarks>
    /// the pattern is only applied on the current line.
    /// </remarks>
    [Pure]
    public static Tokens line(Regex pattern, TKind kind = default) => t
        => t.Match(s => s.Line(pattern), kind);

    /// <inheritdoc cref="regex(System.Text.RegularExpressions.Regex, TKind)"/>
    [Pure]
    public static Tokens regex([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, TKind kind = default)
        => regex(Regex(pattern), kind);

    /// <summary>Matches if the remaining source starts with the specified pattern.</summary>
    /// <param name="pattern">
    /// The expected pattern.
    /// </param>
    /// <param name="kind">
    /// The (optional) token kind.
    /// </param>
    /// <returns>
    /// A new tokenizer with an updated state.
    /// </returns>
    [Pure]
    public static Tokens regex(Regex pattern, TKind kind = default) => t
        => t.Match(s => s.Regex(pattern), kind);

    [Pure]
    private static Tokenizer<TKind> Repeat(Tokenizer<TKind> t, Tokens match, int min, int max)
    {
        var curr = t;
        var repeat = 0;

        while (repeat < max)
        {
            var next = match(curr);

            switch (next.State)
            {
                case Matching.Match:
                    repeat++;
                    curr = next;
                    break;

                case Matching.EoF:
                    repeat++;
                    return repeat >= min ? next : t.NoMatch();

                case Matching.NoMatch:
                default:
                    return repeat >= min ? next : t.NoMatch();
            }
        }

        return t.NoMatch();
    }

    [Pure]
    private static Tokenizer<TKind> Option(Tokenizer<TKind> t, Tokens match)
    {
        var next = match(t);
        if (next.State != Matching.NoMatch)
        {
            return next;
        }
        else
        {
            return t;
        }
    }

    private static Regex Regex(string regex) => regex[0] == '^'
        ? new(regex, Options, Timeout)
        : new('^' + regex, Options, Timeout);

    private static readonly RegexOptions Options = RegexOptions.CultureInvariant;

    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);
}
