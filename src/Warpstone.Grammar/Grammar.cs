#pragma warning disable SA1300 // Element should begin with upper-case letter
// Grammar is commonly defined with lowercase words.

using Warpstone.Grammars;

namespace Warpstone;

/// <summary>A way of defining formal grammar.</summary>
/// <typeparam name="TKind">
/// The kind of tokens that are tokenized.
/// </typeparam>

public class Grammar<TKind> where TKind : struct, Enum
{
    /// <summary>Initializes a new instance of the <see cref="Grammar{TKind}"/> class.</summary>
    /// <remarks>
    /// This class should not be instantiated.
    /// </remarks>
    protected Grammar() { }

    [Pure]
    public Tokenizer<TKind> Tokenize(SourceText sourceText)
        => Match(Tokenizer<TKind>.New(new(sourceText)));

    /// <summary>Matches the grammar on the current state of the tokenizer.</summary>
    /// <param name="tokenizer">
    /// The (current state of the) tokenizer.
    /// </param>
    /// <returns>
    /// An updated tokenizer.
    /// </returns>
    [Pure]
    public virtual Tokenizer<TKind> Match(Tokenizer<TKind> tokenizer) => throw new NotImplementedException();

    /// <summary>This grammar may or may not match.</summary>
    [Pure]
    public Grammar<TKind> Option => new Repeat<TKind>(this, 0, 1);

    /// <summary>This grammar must not match.</summary>
    [Pure]
    public Grammar<TKind> Not => new Repeat<TKind>(this, 0, 0);

    /// <summary>This grammar may match multiple times.</summary>
    [Pure]
    public Grammar<TKind> Star => Repeat(0);

    /// <summary>This grammar may match multiple times, but at least once.</summary>
    [Pure]
    public Grammar<TKind> Plus => Repeat(1);

    /// <summary>This grammar may match multiple times.</summary>
    /// <param name="min">
    /// The minimum number of matches.
    /// </param>
    /// <param name="max">
    /// The maximum number of matches.
    /// </param>
    /// <returns>
    /// A new grammar.
    /// </returns>
    [Pure]
    public Grammar<TKind> Repeat(int min, int max = int.MaxValue) => new Repeat<TKind>(this, min, max);

    public static Grammar<TKind> operator ~(Grammar<TKind> grammar) => grammar.Not;

    public static Grammar<TKind> operator |(Grammar<TKind> l, Grammar<TKind> r) => new Or<TKind>(l, r);

    public static Grammar<TKind> operator &(Grammar<TKind> l, Grammar<TKind> r) => new And<TKind>(l, r);
    /// <summary>End of File.</summary>
    public static readonly Grammar<TKind> eof = new EndOfFile<TKind>();

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
    public static Grammar<TKind> ch(char ch, TKind kind = default) => new StartWithChar<TKind>(ch, kind);

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
    public static Grammar<TKind> str(string str, TKind kind = default) => new StartWithString<TKind>(str, kind);

    /// <inheritdoc cref="line(System.Text.RegularExpressions.Regex, TKind)"/>
    [Pure]
    public static Grammar<TKind> line([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, TKind kind = default)
        => new RegularExpression<TKind>(pattern, kind, true);

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
    public static Grammar<TKind> line(Regex pattern, TKind kind = default) => new RegularExpression<TKind>(pattern, kind, true);

    /// <inheritdoc cref="regex(Regex, TKind)"/>
    [Pure]
    public static Grammar<TKind> regex([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, TKind kind = default)
        => new RegularExpression<TKind>(pattern, kind, false);

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
    public static Grammar<TKind> regex(Regex pattern, TKind kind = default) => new RegularExpression<TKind>(pattern, kind, false);
}
