using System.Globalization;

namespace Warpstone.Tests.SourceGeneration;

/// <summary>
/// Grammars used to check that generated parsers parse like Warpstone.
/// Their transformations only use parameters and public static members, so their source can be generated.
/// </summary>
public static class Grammars
{
    /// <summary>The calculator from the README: left recursion, Lazy, Or, Regex, WithName and Then/ThenSkip.</summary>
    public static readonly IParser<int> Calculator = CreateCalculator();

    /// <summary>S &lt;- A 'b' / 'b' ; A &lt;- A 'a' / S 'a': a left-recursive cycle without a leader.</summary>
    public static readonly IParser<string> IndirectCycle = CreateIndirectCycle();

    /// <summary>Expr &lt;- Expr '+' Num / Num ; Num &lt;- Num [0-9] / [0-9]: nested left recursion.</summary>
    public static readonly IParser<string> NestedLeftRecursion = CreateNestedLeftRecursion();

    /// <summary>A list of key-value pairs: Many with a delimiter, whitespace, tuples and spread transformations.</summary>
    public static readonly IParser<IReadOnlyList<KeyValuePair<string, int>>> KeyValues
        = Many(
            Regex("[a-z]+").ThenSkip(OptionalWhitespaces).ThenSkip(Char('=')).ThenSkip(OptionalWhitespaces)
                .ThenAdd(Regex("[0-9]+").Transform(int.Parse))
                .Transform((key, value) => new KeyValuePair<string, int>(key, value)),
            Char(',').ThenSkip(OptionalWhitespaces))
        .ThenEnd();

    /// <summary>Lookaheads, names, Maybe, Not and End.</summary>
    public static readonly IParser<string> Keywords
        = Or(
            String("let").ThenSkip(Not(Regex("[a-z]"))).Transform(_ => "keyword"),
            Regex("[a-z]+").WithName("identifier"),
            Peek(Char('(')).Then(String("()")))
        .ThenAdd(Maybe(Char('!'), '?'))
        .Transform((word, mark) => word + mark)
        .ThenEnd();

    /// <summary>Three-element tuples, a custom Aggregate, case-insensitive and culture-aware strings.</summary>
    public static readonly IParser<string> Mixed
        = String("select", ignoreCase: true)
            .ThenAdd(Whitespaces)
            .ThenAdd(Aggregate(Char('x'), Char('.'), 1, 3, () => 0, (count, _) => count + 1))
            .ThenAdd(String("straße", StringComparison.InvariantCultureIgnoreCase))
            .Transform((select, space, count, street) => $"{select.ToUpperInvariant()}{space.Length}{count}{street}")
            .ThenEnd();

    /// <summary>A transformation that throws on large numbers, and a sequence transformed twice.</summary>
    public static readonly IParser<int> Throwing
        = Regex("[0-9]+").ThenAdd(Char('!')).Transform((digits, bang) => digits + bang).Transform(text => int.Parse(text.TrimEnd('!'), CultureInfo.InvariantCulture)).ThenEnd();

    /// <summary>Lambdas of every shape: block bodies, typed parameters, discards, static lambdas and method groups.</summary>
    public static readonly IParser<string> Lambdas
        = Regex("[a-z]+").Transform(static w => { var upper = w.ToUpperInvariant(); return upper + upper.Length; })
            .ThenAdd(Char('-').Then(Regex("[0-9]+")).Transform(int.Parse))
            .ThenAdd(Char('-').Then(Regex("[a-z]")))
#pragma warning disable S3257 // The parameter types are explicit on purpose: they test typed lambda parameters.
            .Transform((string word, int number, string letter) => word + (number * 2) + letter)
#pragma warning restore S3257
            .ThenAdd(Maybe(Char('!').ThenAdd(Char('!')).Transform((_, _) => "!!"), string.Empty))
            .Transform(static (text, bangs) => text + bangs)
            .ThenEnd();

    /// <summary>Explicit memoization of a parser that is tried twice at the same position.</summary>
    public static readonly IParser<string> Memoized = CreateMemoized();

    /// <summary>Words separated by spaces: a scanned regex whose matches are interned.</summary>
    public static readonly IParser<IReadOnlyList<string>> Words = Many(Regex("[a-z0-9]+"), Char(' ')).ThenEnd();

    /// <summary>Repetitions of one element type nested in each other, and a repetition that fails after collecting elements.</summary>
    public static readonly IParser<string> NestedLists = CreateNestedLists();

    // The calculator of the README, written the same way.
    private static IParser<int> CreateCalculator()
    {
        IParser<int> sum = null!;
        IParser<int> term = null!;
        var number = Regex("[0-9]+").Transform(int.Parse).WithName("number");
        var atom = Or(number, Char('(').Then(Lazy(() => sum)).ThenSkip(Char(')')));
        term = Or(
            Lazy(() => term).ThenSkip(Operator("*")).ThenAdd(atom).Transform((left, right) => left * right),
            Lazy(() => term).ThenSkip(Operator("/")).ThenAdd(atom).Transform((left, right) => left / right),
            atom);
        sum = Or(
            Lazy(() => sum).ThenSkip(Operator("+")).ThenAdd(term).Transform((left, right) => left + right),
            Lazy(() => sum).ThenSkip(Operator("-")).ThenAdd(term).Transform((left, right) => left - right),
            term);
        return sum.ThenEnd();
    }

    private static IParser<string> Operator(string symbol)
        => OptionalWhitespaces.Then(String(symbol)).ThenSkip(OptionalWhitespaces);

    private static IParser<string> CreateIndirectCycle()
    {
        IParser<string> s = null!;
        IParser<string> a = null!;
        s = Or(Lazy(() => a).ThenAdd(String("b")).Transform((x, y) => x + y), String("b"));
        a = Or(Lazy(() => a).ThenAdd(String("a")).Transform((x, y) => x + y), Lazy(() => s).ThenAdd(String("a")).Transform((x, y) => x + y));
        return s.ThenEnd();
    }

    private static IParser<string> CreateNestedLeftRecursion()
    {
        IParser<string> expr = null!;
        IParser<string> num = null!;
        var digit = Regex("[0-9]");
        num = Or(Lazy(() => num).ThenAdd(digit).Transform((x, y) => x + y), digit);
        expr = Or(Lazy(() => expr).ThenAdd(String("+")).ThenAdd(num).Transform((x, y, z) => x + y + z), num);
        return expr.ThenEnd();
    }

    private static IParser<string> CreateNestedLists()
    {
        IParser<string> item = null!;
        var letter = Regex("[a-z]");
        item = Or(
            Char('[').Then(Many(Lazy(() => item))).ThenSkip(Char(']')).Transform(xs => "[" + string.Join(",", xs) + "]"),
            Multiple(letter, 3, 3).Transform(xs => string.Concat(xs)),
            letter);
        return Many(item).Transform(xs => string.Join(";", xs)).ThenEnd();
    }

    private static IParser<string> CreateMemoized()
    {
        var word = Memo(Regex("[a-z]+").Transform(w => w.ToUpperInvariant()));
        return Or(word.ThenSkip(Char(';')), word.ThenSkip(Char('.'))).ThenEnd();
    }
}
