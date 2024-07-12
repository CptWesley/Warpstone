namespace Warpstone;

/// <summary>A syntax tokenizer.</summary>
/// <typeparam name="TKind">
/// The kind of tokens that are tokenized.
/// </typeparam>
[DebuggerDisplay("Tokens: {Tokens.Count}, {SourceSpan.Span}, Text = {SourceSpan.Text}")]
public readonly struct Tokenizer<TKind> where TKind : struct, Enum
{
    /// <summary>Initializes a new instance of the <see cref="Tokenizer{TKind}"/> struct.</summary>
    private Tokenizer(SourceSpan sourceSpan, IImmutableList<SyntaxToken<TKind>> tokens, Matching state)
    {
        SourceSpan = sourceSpan;
        Tokens = tokens;
        State = state;
    }

    /// <summary>The tokinized tokens so far.</summary>
    public readonly IImmutableList<SyntaxToken<TKind>> Tokens;

    /// <summary>The (remaining) source span to tokenize.</summary>
    public readonly SourceSpan SourceSpan;

    /// <summary>The matching state.</summary>
    public readonly Matching State;

    /// <summary>The result of <see cref="Matching.NoMatch"/>.</summary>
    /// <returns>
    /// A new tokenizer with an updated state.
    /// </returns>
    [Pure]
    public Tokenizer<TKind> NoMatch() => new(SourceSpan, Tokens, Matching.NoMatch);

    /// <summary>Tries to apply a <see cref="SourceSpan.Match"/>.</summary>
    /// <param name="match">
    /// The match to apply.
    /// </param>
    /// <param name="kind">
    /// The token kind to add, if the match was successful.
    /// </param>
    /// <returns>
    /// A new tokenizer with an updated state.
    /// </returns>
    [Pure]
    public Tokenizer<TKind> Match(SourceSpan.Match match, TKind kind) => match(SourceSpan) switch
    {
        null => NoMatch(),
        var span when span.Value.Length == 0 => this,
        var span => New(span.Value, kind),
    };

    /// <summary>Applies an AND on the current state and the specified match.</summary>
    /// <param name="tokenizer">
    /// The current state.
    /// </param>
    /// <param name="match">
    /// The match to apply.
    /// </param>
    [Pure]
    public static Tokenizer<TKind> operator +(Tokenizer<TKind> tokenizer, Grammar<TKind>.Tokens match)
        => tokenizer.State == Matching.NoMatch
        ? tokenizer
        : match(tokenizer);

    /// <summary>Applies an OR on the current state and the specified match.</summary>
    /// <param name="tokenizer">
    /// The current state.
    /// </param>
    /// <param name="match">
    /// The match to apply.
    /// </param>
    public static Tokenizer<TKind> operator |(Tokenizer<TKind> tokenizer, Grammar<TKind>.Tokens match)
        => tokenizer.State != Matching.NoMatch
        ? tokenizer
        : match(tokenizer);

    /// <summary>Creates a new state.</summary>
    /// <param name="span">
    /// The matching span.
    /// </param>
    /// <param name="kind">
    /// The kind of the token to add.
    /// </param>
    /// <returns>
    /// A new tokenizer with an updated state.
    /// </returns>
    [Pure]
    private Tokenizer<TKind> New(TextSpan span, TKind kind)
    {
        var token = new SyntaxToken<TKind>(SourceSpan.Trim(span), kind);
        var trimmed = SourceSpan.TrimLeft(span.Length);
        return new(trimmed, Tokens.Add(token), trimmed.IsEmpty ? Matching.EoF : Matching.Match);
    }

    /// <summary>Creates a new state.</summary>
    /// <param name="sourceText">
    /// The source text to tokenize.
    /// </param>
    /// <returns>
    /// A new tokenizer with an updated state.
    /// </returns>
    [Pure]
    public static Tokenizer<TKind> New(SourceText sourceText)
        => New(new SourceSpan(sourceText));

    /// <summary>Creates a new state.</summary>
    /// <param name="sourceSpan">
    /// The source span to tokenize.
    /// </param>
    /// <returns>
    /// A new tokenizer with an updated state.
    /// </returns>
    [Pure]
    public static Tokenizer<TKind> New(SourceSpan sourceSpan)
        => new(sourceSpan, [], sourceSpan.Length == 0 ? Matching.EoF : Matching.Match);

    /// <summary>Tokenizes a source text.</summary>
    /// <param name="sourceText">
    /// The source text to tokenize.
    /// </param>
    /// <param name="grammar">
    /// The grammar to apply.
    /// </param>
    /// <returns>
    /// A tokinized source text.
    /// </returns>
    [Pure]
    public static Tokenizer<TKind> Tokenize(SourceText sourceText, Grammar<TKind>.Tokens grammar)
        => grammar(Tokenizer<TKind>.New(sourceText));
}
