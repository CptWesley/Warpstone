namespace Warpstone;

public partial class Grammar<TKind> where TKind : struct, Enum
{
    /// <summary>Initializes a new instance of the <see cref="Grammar{TKind}"/> class.</summary>
    /// <remarks>
    /// This class should not be instantiated.
    /// </remarks>
    protected Grammar() { }

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
    public Grammar<TKind> Option => new Optional(this);

    /// <summary>This grammar must not match.</summary>
    [Pure]
    public Grammar<TKind> Not => new Negate(this);

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
    public Grammar<TKind> Repeat(int min, int max = int.MaxValue) => new Multiple(this, min, max);

    public static Grammar<TKind> operator |(Grammar<TKind> l, Grammar<TKind> r) => new Or(l, r);

    public static Grammar<TKind> operator &(Grammar<TKind> l, Grammar<TKind> r) => new And(l, r);

    private sealed class And(Grammar<TKind> left, Grammar<TKind> right) : Grammar<TKind>
    {
        private readonly Grammar<TKind> Left = left;
        private readonly Grammar<TKind> Right = right;

        [Pure]
        public override Tokenizer<TKind> Match(Tokenizer<TKind> tokenizer)
            => Left.Match(tokenizer) is { State: not Matching.NoMatch } first
            && Right.Match(first) is { State: not Matching.NoMatch } second
            ? second
            : tokenizer.NoMatch();

        [Pure]
        public override string ToString() => $"({Left} & {Right})";
    }

    private sealed class Or(Grammar<TKind> left, Grammar<TKind> right) : Grammar<TKind>
    {
        private readonly Grammar<TKind> Left = left;
        private readonly Grammar<TKind> Right = right;

        [Pure]
        public override Tokenizer<TKind> Match(Tokenizer<TKind> tokenizer) => tokenizer switch
        {
            _ when Left.Match(tokenizer) is { State: not Matching.NoMatch } next => next,
            _ when Right.Match(tokenizer) is { State: not Matching.NoMatch } next => next,
            _ => tokenizer.NoMatch(),
        };

        [Pure]
        public override string ToString() => $"({Left} | {Right})";
    }

    private sealed class Optional(Grammar<TKind> grammar) : Grammar<TKind>
    {
        private readonly Grammar<TKind> Grammar = grammar;

        [Pure]
        public override Tokenizer<TKind> Match(Tokenizer<TKind> tokenizer)
            => Grammar.Match(tokenizer) is { State: not Matching.NoMatch } next
            ? next
            : tokenizer;

        [Pure]
        public override string ToString() => $"({Grammar})?";
    }

    private sealed class Negate(Grammar<TKind> grammar) : Grammar<TKind>
    {
        private readonly Grammar<TKind> Grammar = grammar;

        [Pure]
        public override Tokenizer<TKind> Match(Tokenizer<TKind> tokenizer)
            => Grammar.Match(tokenizer) is { State: Matching.NoMatch } 
            ? tokenizer
            : tokenizer.NoMatch();

        [Pure]
        public override string ToString() => $"({Grammar}).Not";
    }

    private sealed class Multiple(Grammar<TKind> grammar, int min, int max) : Grammar<TKind>
    {
        private readonly Grammar<TKind> Grammar = grammar;
        
        private readonly int Minimum = min;
        
        private readonly int Maximum = max;

        [Pure]
        public override Tokenizer<TKind> Match(Tokenizer<TKind> tokenizer)
        {

            var i = 0;
            var next = tokenizer;
            while (i++ < Maximum && next.State == Matching.Match)
            {
                next = Grammar.Match(next);
            }

            return i < Minimum || next.State == Matching.NoMatch
                ? tokenizer.NoMatch()
                : next;
        }

        [Pure]
        public override string ToString() => Minimum switch
        {
            _ when Minimum == 0 && Maximum == int.MaxValue => $"({Grammar})*",
            _ when Minimum == 1 && Maximum == int.MaxValue => $"({Grammar})+",
            _ when Minimum == Maximum => $"({Grammar}){{{Minimum}}}",
            _ when Maximum == int.MaxValue => $"({Grammar}){{{Minimum},}}",
            _ => $"({Grammar}){{{Minimum},{Maximum}}}",
        };
    }

    private sealed class Character(char ch, TKind kind) : Grammar<TKind>
    {
        private readonly char Char = ch;
        private readonly TKind Kind = kind;

        public override Tokenizer<TKind> Match(Tokenizer<TKind> tokenizer)
            => tokenizer.Match(s => s.StartsWith(Char), Kind);

        [Pure]
        public override string ToString() => $"ch('{Char}', {Kind})";
    }

    private sealed class Startwith(string str, TKind kind) : Grammar<TKind>
    {
        private readonly string String = str;
        private readonly TKind Kind = kind;

        [Pure]
        public override Tokenizer<TKind> Match(Tokenizer<TKind> tokenizer)
            => tokenizer.Match(s => s.StartsWith(String), Kind);

        [Pure]
        public override string ToString() => $@"string(""{String}"", {Kind})";
    }

    private sealed class RegularExpression(Regex pattern, TKind kind) : Grammar<TKind>
    {
        private readonly Regex Pattern = pattern;
        private readonly TKind Kind = kind;

        [Pure]
        public override Tokenizer<TKind> Match(Tokenizer<TKind> tokenizer)
            => tokenizer.Match(s => s.Regex(Pattern), Kind);

        [Pure]
        public override string ToString() => $@"regex(""{Pattern}"", {Kind})";
    }

    private sealed class RegularLineExpression(Regex pattern, TKind kind) : Grammar<TKind>
    {
        private readonly Regex Pattern = pattern;
        private readonly TKind Kind = kind;

        [Pure]
        public override Tokenizer<TKind> Match(Tokenizer<TKind> tokenizer)
            => tokenizer.Match(s => s.Line(Pattern), Kind);

        [Pure]
        public override string ToString() => $@"line(""{Pattern}"", {Kind})";
    }

    private sealed class EndOfFile() : Grammar<TKind>
    {
        [Pure]
        public override Tokenizer<TKind> Match(Tokenizer<TKind> tokenizer)
            => tokenizer.State == Matching.EoF
            ? tokenizer
            : tokenizer.NoMatch();

        [Pure]
        public override string ToString() => "eof";
    }
}
