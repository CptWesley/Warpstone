#pragma warning disable SA1300 // Element should begin with upper-case letter
// Grammar is commonly defined with lowercase words.

namespace Warpstone;

/// <summary>A way of defining formal grammar.</summary>
/// <typeparam name="TKind">
/// The kind of tokens that are tokenized.
/// </typeparam>
public partial class Grammar<TKind> where TKind : struct, Enum
{
    /// <summary>End of File.</summary>
    public static readonly Grammar<TKind> eof = new EndOfFile();

    /// <summary>Matches if the remaining source starts with the specified character.</summary>
    /// <param name="ch">
    /// The expected character.
    /// </param>
    /// <param name="kind">
    /// The (optional) token kind.
    /// </param>
    /// <returns>
    /// A new grammar.
    /// </returns>
    [Pure]
    public static Grammar<TKind> ch(char ch, TKind kind = default) => new Character(ch, kind);

    /// <summary>Matches if the remaining source starts with the specified string.</summary>
    /// <param name="str">
    /// The expected string.
    /// </param>
    /// <param name="kind">
    /// The (optional) token kind.
    /// </param>
    /// <returns>
    /// A new grammar.
    /// </returns>
    [Pure]
    public static Grammar<TKind> str(string str, TKind kind = default) => new Startwith(str, kind);

    /// <inheritdoc cref="line(System.Text.RegularExpressions.Regex, TKind)"/>
    [Pure]
    public static Grammar<TKind> line([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, TKind kind = default)
        => line(Regex(pattern), kind);

    /// <summary>Matches if the remaining source starts with the specified pattern.</summary>
    /// <param name="pattern">
    /// The expected pattern.
    /// </param>
    /// <param name="kind">
    /// The (optional) token kind.
    /// </param>
    /// <returns>
    /// A new grammar.
    /// </returns>
    /// <remarks>
    /// the pattern is only applied on the current line.
    /// </remarks>
    [Pure]
    public static Grammar<TKind> line(Regex pattern, TKind kind = default) => new RegularLineExpression(pattern, kind);

    /// <inheritdoc cref="regex(System.Text.RegularExpressions.Regex, TKind)"/>
    [Pure]
    public static Grammar<TKind> regex([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, TKind kind = default)
        => regex(Regex(pattern), kind);

    /// <summary>Matches if the remaining source starts with the specified pattern.</summary>
    /// <param name="pattern">
    /// The expected pattern.
    /// </param>
    /// <param name="kind">
    /// The (optional) token kind.
    /// </param>
    /// <returns>
    /// A new grammar.
    /// </returns>
    [Pure]
    public static Grammar<TKind> regex(Regex pattern, TKind kind = default) => new RegularLineExpression(pattern, kind);

    private static Regex Regex(string regex) => regex[0] == '^'
        ? new(regex, Options, Timeout)
        : new('^' + regex, Options, Timeout);

    private static readonly RegexOptions Options = RegexOptions.CultureInvariant;

    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);
}
