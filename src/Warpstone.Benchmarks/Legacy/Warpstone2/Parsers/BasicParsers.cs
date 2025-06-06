#nullable enable

using Legacy.Warpstone2.Errors;
using Legacy.Warpstone2.Internal;
using Legacy.Warpstone2.Parsers.Internal;
using System.Collections.Immutable;

namespace Legacy.Warpstone2.Parsers;

/// <summary>
/// Static class providing simple parsers.
/// </summary>
[SuppressMessage("Maintainability", "CA1506", Justification = "Not exposed to end user.")]
public static partial class BasicParsers
{
    /// <summary>
    /// A parser matching the end of an input stream.
    /// </summary>
    public static readonly IParser<string> End = EndOfFileParser.Instance;

    /// <summary>
    /// A parser which returns the current position in the input.
    /// </summary>
    public static readonly IParser<ParseInputPosition> Position = PositionParser.Instance;

    /// <summary>
    /// Creates a parser that always succeeds.
    /// </summary>
    /// <typeparam name="T">The return type of the parser.</typeparam>
    /// <returns>A parser that always succeeds.</returns>
    public static IParser<T?> Pass<T>()
        => VoidParser<T>.Instance;

    /// <summary>
    /// Creates a parser that always fails.
    /// </summary>
    /// <typeparam name="T">The return type of the parser.</typeparam>
    /// <returns>A parser that always fails.</returns>
    public static IParser<T> Fail<T>()
        => FailParser<T>.Instance;

    /// <summary>
    /// Creates a parser which matches a regular expression.
    /// </summary>
    /// <param name="pattern">The pattern to match.</param>
    /// <param name="options">The regex options to use.</param>
    /// <returns>A parser matching a regular expression.</returns>
    public static IParser<string> Regex([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, RegexOptions options)
        => new RegexParser(pattern.MustNotBeNull(), options);

    /// <summary>
    /// Creates a parser which matches a regular expression.
    /// </summary>
    /// <param name="pattern">The pattern to match.</param>
    /// <returns>A parser matching a regular expression.</returns>
    public static IParser<string> Regex([StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
        => new RegexParser(pattern.MustNotBeNull());

    /// <summary>
    /// Creates a parser that parses a string.
    /// </summary>
    /// <param name="str">The string to parse.</param>
    /// <returns>A parser parsing a string.</returns>
    public static IParser<string> String(string str)
        => new StringParser(str.MustNotBeNull());

    /// <summary>
    /// Creates a parser that parses a string, using the specified string comparison method.
    /// </summary>
    /// <param name="str">The string to parse.</param>
    /// <param name="stringComparison">The string comparison method to use.</param>
    /// <returns>A parser parsing a string.</returns>
    public static IParser<string> String(string str, StringComparison stringComparison)
        => new StringParser(str.MustNotBeNull(), stringComparison);

    /// <summary>
    /// Creates a parser parsing the given character.
    /// </summary>
    /// <param name="c">The character to parse.</param>
    /// <returns>A parser parsing the given character.</returns>
    public static IParser<char> Char(char c)
        => new CharacterParser(c);

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="T1">The type of results collected.</typeparam>
    /// <typeparam name="T2">The type of delimiters.</typeparam>
    /// <param name="parser">The parser to apply multiple times.</param>
    /// <param name="count">The exact number of matches.</param>
    /// <param name="terminator">The terminator indicating the end of the sequence.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<T1>> Multiple<T1, T2>(IParser<T1> parser, int count, IParser<T2> terminator)
        => Multiple(
            parser.MustNotBeNull(),
            VoidParser<T2>.Instance,
            count.MustBeGreaterThan(0),
            terminator.MustNotBeNull());

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="T">The type of results collected.</typeparam>
    /// <param name="parser">The parser to apply multiple times.</param>
    /// <param name="count">The exact number of matches.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<T>> Multiple<T>(IParser<T> parser, int count)
        => Multiple(
            parser.MustNotBeNull(),
            count.MustBeGreaterThan(0),
            VoidParser<object>.Instance);

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="T1">The type of results collected.</typeparam>
    /// <typeparam name="T2">The type of delimiters.</typeparam>
    /// <typeparam name="T3">The type of the terminator.</typeparam>
    /// <param name="parser">The parser to apply multiple times.</param>
    /// <param name="delimiter">The delimiter seperating the different elements.</param>
    /// <param name="count">The exact number of matches.</param>
    /// <param name="terminator">The terminator indicating the end of the sequence.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<T1>> Multiple<T1, T2, T3>(IParser<T1> parser, IParser<T2> delimiter, int count, IParser<T3> terminator)
        => Multiple(
            parser.MustNotBeNull(),
            delimiter.MustNotBeNull(),
            count.MustBeGreaterThanOrEqualTo(0),
            count,
            terminator.MustNotBeNull());

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="T1">The type of results collected.</typeparam>
    /// <typeparam name="T2">The type of delimiters.</typeparam>
    /// <param name="parser">The parser to apply multiple times.</param>
    /// <param name="delimiter">The delimiter seperating the different elements.</param>
    /// <param name="count">The exact number of matches.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<T1>> Multiple<T1, T2>(IParser<T1> parser, IParser<T2> delimiter, int count)
        => Multiple(
            parser.MustNotBeNull(),
            delimiter.MustNotBeNull(),
            count.MustBeGreaterThanOrEqualTo(0),
            VoidParser<T2>.Instance);

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="T1">The type of results collected.</typeparam>
    /// <typeparam name="T2">The type of the terminator.</typeparam>
    /// <param name="parser">The parser to apply multiple times.</param>
    /// <param name="min">The minimum number of matches.</param>
    /// <param name="max">The maximum number of matches.</param>
    /// <param name="terminator">The terminator indicating the end of the sequence.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<T1>> Multiple<T1, T2>(IParser<T1> parser, int min, int max, IParser<T2> terminator)
        => Multiple(
            parser.MustNotBeNull(),
            VoidParser<T2>.Instance,
            min.MustBeGreaterThanOrEqualTo(0),
            max.MustBeGreaterThanOrEqualTo(min),
            terminator.MustNotBeNull());

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="T">The type of results collected.</typeparam>
    /// <param name="parser">The parser to apply multiple times.</param>
    /// <param name="min">The minimum number of matches.</param>
    /// <param name="max">The maximum number of matches.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<T>> Multiple<T>(IParser<T> parser, int min, int max)
        => Multiple(
            parser.MustNotBeNull(),
            min.MustBeGreaterThanOrEqualTo(0),
            max.MustBeGreaterThanOrEqualTo(min),
            VoidParser<object>.Instance);

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="T1">The type of results collected.</typeparam>
    /// <typeparam name="T2">The type of delimiters.</typeparam>
    /// <typeparam name="T3">The type of the terminator.</typeparam>
    /// <param name="parser">The parser to apply multiple times.</param>
    /// <param name="delimiter">The delimiter seperating the different elements.</param>
    /// <param name="min">The minimum number of matches.</param>
    /// <param name="max">The maximum number of matches.</param>
    /// <param name="terminator">The terminator indicating the end of the sequence.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<T1>> Multiple<T1, T2, T3>(IParser<T1> parser, IParser<T2> delimiter, int min, int max, IParser<T3> terminator)
        => Multiple(
            parser.MustNotBeNull(),
            delimiter.MustNotBeNull(),
            (ulong)min.MustBeGreaterThanOrEqualTo(0),
            (ulong)max.MustBeGreaterThanOrEqualTo(min),
            terminator.MustNotBeNull());

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="T1">The type of results collected.</typeparam>
    /// <typeparam name="T2">The type of delimiters.</typeparam>
    /// <param name="parser">The parser to apply multiple times.</param>
    /// <param name="delimiter">The delimiter seperating the different elements.</param>
    /// <param name="min">The minimum number of matches.</param>
    /// <param name="max">The maximum number of matches.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<T1>> Multiple<T1, T2>(IParser<T1> parser, IParser<T2> delimiter, int min, int max)
        => Multiple(
            parser.MustNotBeNull(),
            delimiter.MustNotBeNull(),
            min.MustBeGreaterThanOrEqualTo(0),
            max.MustBeGreaterThanOrEqualTo(max),
            VoidParser<T2>.Instance);

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="T1">The type of results collected.</typeparam>
    /// <typeparam name="T2">The type of the terminator.</typeparam>
    /// <param name="parser">The parser to apply multiple times.</param>
    /// <param name="count">The exact number of matches.</param>
    /// <param name="terminator">The terminator indicating the end of the sequence.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<T1>> Multiple<T1, T2>(IParser<T1> parser, ulong count, IParser<T2> terminator)
        => Multiple(
            parser.MustNotBeNull(),
            VoidParser<T2>.Instance,
            count.MustBeGreaterThanOrEqualTo(0),
            terminator.MustNotBeNull());

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="T">The type of results collected.</typeparam>
    /// <param name="parser">The parser to apply multiple times.</param>
    /// <param name="count">The exact number of matches.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<T>> Multiple<T>(IParser<T> parser, ulong count)
        => Multiple(
            parser.MustNotBeNull(),
            count.MustBeGreaterThanOrEqualTo(0),
            VoidParser<object>.Instance);

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="T1">The type of results collected.</typeparam>
    /// <typeparam name="T2">The type of delimiters.</typeparam>
    /// <typeparam name="T3">The type of the terminator.</typeparam>
    /// <param name="parser">The parser to apply multiple times.</param>
    /// <param name="delimiter">The delimiter seperating the different elements.</param>
    /// <param name="count">The exact number of matches.</param>
    /// <param name="terminator">The terminator indicating the end of the sequence.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<T1>> Multiple<T1, T2, T3>(IParser<T1> parser, IParser<T2> delimiter, ulong count, IParser<T3> terminator)
        => Multiple(
            parser.MustNotBeNull(),
            delimiter.MustNotBeNull(),
            count.MustBeGreaterThanOrEqualTo(0),
            count,
            terminator.MustNotBeNull());

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="T1">The type of results collected.</typeparam>
    /// <typeparam name="T2">The type of delimiters.</typeparam>
    /// <param name="parser">The parser to apply multiple times.</param>
    /// <param name="delimiter">The delimiter seperating the different elements.</param>
    /// <param name="count">The exact number of matches.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<T1>> Multiple<T1, T2>(IParser<T1> parser, IParser<T2> delimiter, ulong count)
        => Multiple(
            parser.MustNotBeNull(),
            delimiter.MustNotBeNull(),
            count.MustBeGreaterThanOrEqualTo(0),
            VoidParser<T2>.Instance);

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="T1">The type of results collected.</typeparam>
    /// <typeparam name="T2">The type of the terminator.</typeparam>
    /// <param name="parser">The parser to apply multiple times.</param>
    /// <param name="min">The minimum number of matches.</param>
    /// <param name="max">The maximum number of matches.</param>
    /// <param name="terminator">The terminator indicating the end of the sequence.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<T1>> Multiple<T1, T2>(IParser<T1> parser, ulong min, ulong max, IParser<T2> terminator)
        => Multiple(
            parser.MustNotBeNull(),
            VoidParser<T2>.Instance,
            min.MustBeGreaterThanOrEqualTo(0),
            max.MustBeGreaterThanOrEqualTo(min),
            terminator.MustNotBeNull());

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="T">The type of results collected.</typeparam>
    /// <param name="parser">The parser to apply multiple times.</param>
    /// <param name="min">The minimum number of matches.</param>
    /// <param name="max">The maximum number of matches.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<T>> Multiple<T>(IParser<T> parser, ulong min, ulong max)
        => Multiple(
            parser.MustNotBeNull(),
            min.MustBeGreaterThanOrEqualTo(0),
            max.MustBeGreaterThanOrEqualTo(max),
            VoidParser<object>.Instance);

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="T1">The type of results collected.</typeparam>
    /// <typeparam name="T2">The type of delimiters.</typeparam>
    /// <typeparam name="T3">The type of the terminator.</typeparam>
    /// <param name="parser">The parser to apply multiple times.</param>
    /// <param name="delimiter">The delimiter seperating the different elements.</param>
    /// <param name="min">The minimum number of matches.</param>
    /// <param name="max">The maximum number of matches.</param>
    /// <param name="terminator">The terminator indicating the end of the sequence.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<T1>> Multiple<T1, T2, T3>(IParser<T1> parser, IParser<T2> delimiter, ulong min, ulong max, IParser<T3> terminator)
        => new MultipleParser<T1, T2, T3>(
            parser.MustNotBeNull(),
            delimiter.MustNotBeNull(),
            terminator.MustNotBeNull(),
            min.MustBeGreaterThanOrEqualTo(0),
            max.MustBeGreaterThanOrEqualTo(min));

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="T1">The type of results collected.</typeparam>
    /// <typeparam name="T2">The type of delimiters.</typeparam>
    /// <param name="parser">The parser to apply multiple times.</param>
    /// <param name="delimiter">The delimiter seperating the different elements.</param>
    /// <param name="min">The minimum number of matches.</param>
    /// <param name="max">The maximum number of matches.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<T1>> Multiple<T1, T2>(IParser<T1> parser, IParser<T2> delimiter, ulong min, ulong max)
        => Multiple(
            parser.MustNotBeNull(),
            delimiter.MustNotBeNull(),
            min.MustBeGreaterThanOrEqualTo(0),
            max.MustBeGreaterThanOrEqualTo(min),
            VoidParser<T2>.Instance);

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="T">The type of results collected.</typeparam>
    /// <param name="parser">The parser to apply multiple times.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<T>> Many<T>(IParser<T> parser)
        => Many(
            parser.MustNotBeNull(),
            VoidParser<object>.Instance);

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="T1">The type of results collected.</typeparam>
    /// <typeparam name="T2">The type of delimiters.</typeparam>
    /// <typeparam name="T3">The type of the terminator.</typeparam>
    /// <param name="parser">The parser to apply multiple times.</param>
    /// <param name="delimiter">The delimiter seperating the different elements.</param>
    /// <param name="terminator">The terminator indicating the end of the sequence.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<T1>> Many<T1, T2, T3>(IParser<T1> parser, IParser<T2> delimiter, IParser<T3> terminator)
          => Multiple(
              parser.MustNotBeNull(),
              delimiter.MustNotBeNull(),
              0,
              ulong.MaxValue,
              terminator.MustNotBeNull());

    /// <summary>
    /// Creates a parser applying the given parser multiple times and collects all results.
    /// </summary>
    /// <typeparam name="T1">The type of results collected.</typeparam>
    /// <typeparam name="T2">The type of delimiters.</typeparam>
    /// <param name="parser">The parser to apply multiple times.</param>
    /// <param name="delimiter">The delimiter seperating the different elements.</param>
    /// <returns>A parser applying the given parser multiple times.</returns>
    public static IParser<IImmutableList<T1>> Many<T1, T2>(IParser<T1> parser, IParser<T2> delimiter)
          => Multiple(
              parser.MustNotBeNull(),
              delimiter.MustNotBeNull(),
              0,
              ulong.MaxValue,
              VoidParser<T2>.Instance);

    /// <summary>
    /// Creates a parser which applies the given parser at least once and collects all results.
    /// </summary>
    /// <typeparam name="T">The result type of the parser.</typeparam>
    /// <param name="parser">The given parser.</param>
    /// <returns>A parser applying the given parser at least once and collecting all results.</returns>
    public static IParser<IImmutableList<T>> OneOrMore<T>(IParser<T> parser)
        => OneOrMore(
            parser.MustNotBeNull(),
            VoidParser<object>.Instance);

    /// <summary>
    /// Creates a parser which applies the given parser at least once and collects all results.
    /// </summary>
    /// <typeparam name="T1">The type of results collected.</typeparam>
    /// <typeparam name="T2">The type of delimiters.</typeparam>
    /// <typeparam name="T3">The type of the terminator.</typeparam>
    /// <param name="parser">The parser to apply multiple times.</param>
    /// <param name="delimiter">The delimiter seperating the different elements.</param>
    /// <param name="terminator">The terminator indicating the end of the sequence.</param>
    /// <returns>A parser applying the given parser at least once and collecting all results.</returns>
    public static IParser<IImmutableList<T1>> OneOrMore<T1, T2, T3>(IParser<T1> parser, IParser<T2> delimiter, IParser<T3> terminator)
        => Multiple(
            parser.MustNotBeNull(),
            delimiter.MustNotBeNull(),
            1,
            ulong.MaxValue,
            terminator.MustNotBeNull());

    /// <summary>
    /// Creates a parser which applies the given parser at least once and collects all results.
    /// </summary>
    /// <typeparam name="T1">The type of results collected.</typeparam>
    /// <typeparam name="T2">The type of delimiters.</typeparam>
    /// <param name="parser">The parser to apply multiple times.</param>
    /// <param name="delimiter">The delimiter seperating the different elements.</param>
    /// <returns>A parser applying the given parser at least once and collecting all results.</returns>
    public static IParser<IImmutableList<T1>> OneOrMore<T1, T2>(IParser<T1> parser, IParser<T2> delimiter)
        => Multiple(
            parser.MustNotBeNull(),
            delimiter.MustNotBeNull(),
            1,
            ulong.MaxValue,
            VoidParser<T2>.Instance);

    /// <summary>
    /// Creates a parser that tries to apply the given parsers in order and returns the result of the first successful one.
    /// </summary>
    /// <typeparam name="T">The type of results of the given parsers.</typeparam>
    /// <param name="parsers">The parsers to try.</param>
    /// <returns>A parser trying multiple parsers in order and returning the result of the first successful one.</returns>
    public static IParser<T> Or<T>(params IParser<T>[] parsers)
        => Or((IEnumerable<IParser<T>>)parsers);

    /// <summary>
    /// Creates a parser that tries to apply the given parsers in order and returns the result of the first successful one.
    /// </summary>
    /// <typeparam name="T">The type of results of the given parsers.</typeparam>
    /// <param name="parsers">The parsers to try.</param>
    /// <returns>A parser trying multiple parsers in order and returning the result of the first successful one.</returns>
    public static IParser<T> Or<T>(IEnumerable<IParser<T>> parsers)
    {
        parsers.MustNotBeNull();
        if (!parsers.Any())
        {
            throw new ArgumentException("List of parsers may not be empty.", nameof(parsers));
        }

        IParser<T> result = parsers.First().MustNotBeNull();

        foreach (IParser<T> parser in parsers.Skip(1))
        {
            result = new ChoiceParser<T>(result, parser.MustNotBeNull());
        }

        return result;
    }

    /// <summary>
    /// Creates a parser that fails if the specified parser succeeds.
    /// </summary>
    /// <typeparam name="T">The result type of the parser that should fail.</typeparam>
    /// <param name="not">The parser which, if it succeeds, causes the returned parser to fail.</param>
    /// <returns>A parser trying the given parser, and failing if it succeeds.</returns>
    public static IParser<T?> Not<T>(IParser<T> not)
        => new NegativeLookaheadParser<T>(not.MustNotBeNull());

    /// <summary>
    /// Creates a parser that parses the given parser, except if the exclusion parser succeedds, in which case it fails.
    /// </summary>
    /// <typeparam name="T1">The result type of condition the parser.</typeparam>
    /// <typeparam name="T2">The result type of the nested parser.</typeparam>
    /// <param name="parser">The nested parser. This parser is executed if exclusion parser fails.</param>
    /// <param name="exclusion">The exclusion parser. If this parser succeeds, the expression fails. Otherwise, the value from the nested parser is produced.</param>
    /// <returns>A parser trying the given parser, running the nested parser if the condition fails, or failing if the condition succeeds.</returns>
    public static IParser<T2> Except<T1, T2>(this IParser<T2> parser, IParser<T1> exclusion)
        => Not(exclusion.MustNotBeNull()).Then(parser.MustNotBeNull());

    /// <summary>
    /// Creates a parser that first applies the given parser and then applies a transformation on its result.
    /// </summary>
    /// <typeparam name="TInput">The result type of the given input parser.</typeparam>
    /// <typeparam name="TOutput">The result type of the transformation.</typeparam>
    /// <param name="parser">The given input parser.</param>
    /// <param name="output">The result of the transformation.</param>
    /// <returns>A parser first applying the given parser and then applying a transformation on its result.</returns>
    public static IParser<TOutput> Transform<TInput, TOutput>(this IParser<TInput> parser, TOutput output)
        => parser.MustNotBeNull().Transform(_ => output);

    /// <summary>
    /// Creates a parser that first applies the given parser and then applies a transformation on its result.
    /// </summary>
    /// <typeparam name="TInput">The result type of the given input parser.</typeparam>
    /// <typeparam name="TOutput">The result type of the transformation.</typeparam>
    /// <param name="parser">The given input parser.</param>
    /// <param name="transformation">The transformation to apply on the parser result.</param>
    /// <returns>A parser first applying the given parser and then applying a transformation on its result.</returns>
    public static IParser<TOutput> Transform<TInput, TOutput>(this IParser<TInput> parser, Func<TInput, TOutput> transformation)
        => new TransformationParser<TInput, TOutput>(parser.MustNotBeNull(), transformation.MustNotBeNull());

    /// <summary>
    /// Creates a parser that applies two parsers and combines the results.
    /// </summary>
    /// <typeparam name="T1">The result type of the first parser.</typeparam>
    /// <typeparam name="T2">The result type of the second parser.</typeparam>
    /// <param name="first">The first parser.</param>
    /// <param name="second">The second parser.</param>
    /// <returns>A parser combining the results of both parsers.</returns>
    public static IParser<(T1 First, T2 Second)> ThenAdd<T1, T2>(this IParser<T1> first, IParser<T2> second)
        => new SequenceParser<T1, T2>(first.MustNotBeNull(), second.MustNotBeNull());

    /// <summary>
    /// Creates a parser that applies two parsers and combines the results.
    /// </summary>
    /// <typeparam name="T1">The result type of the first parser.</typeparam>
    /// <typeparam name="T2">The result type of the second parser.</typeparam>
    /// <param name="first">The first parser.</param>
    /// <param name="second">The second parser.</param>
    /// <returns>A parser combining the results of both parsers.</returns>
    public static IParser<(T1 First, T2 Second)> ThenAdd<T1, T2>(this IParser<T1> first, Func<T1, IParser<T2>> second)
        => new SelectParser<T1, T2>(first.MustNotBeNull(), second.MustNotBeNull());

    /// <summary>
    /// Creates a parser that applies two parsers and returns the result of the second one.
    /// </summary>
    /// <typeparam name="T1">The result type of the first parser.</typeparam>
    /// <typeparam name="T2">The result type of the second parser.</typeparam>
    /// <param name="first">The first parser.</param>
    /// <param name="second">The second parser.</param>
    /// <returns>A parser returning the result of the second parser.</returns>
    public static IParser<T2> Then<T1, T2>(this IParser<T1> first, IParser<T2> second)
        => first.MustNotBeNull()
        .ThenAdd(second.MustNotBeNull())
        .Transform((_, r) => r);

    /// <summary>
    /// Creates a parser that applies two parsers and returns the result of the second one.
    /// </summary>
    /// <typeparam name="T1">The result type of the first parser.</typeparam>
    /// <typeparam name="T2">The result type of the second parser.</typeparam>
    /// <param name="first">The first parser.</param>
    /// <param name="second">The function to select the second parser.</param>
    /// <returns>A parser returning the result of the second parser.</returns>
    public static IParser<T2> Then<T1, T2>(this IParser<T1> first, Func<T1, IParser<T2>> second)
        => first.MustNotBeNull()
        .ThenAdd(second.MustNotBeNull())
        .Transform((_, r) => r);

    /// <summary>
    /// Creates a parser that applies two parsers and returns the result of the first one.
    /// </summary>
    /// <typeparam name="T1">The result type of the first parser.</typeparam>
    /// <typeparam name="T2">The result type of the second parser.</typeparam>
    /// <param name="first">The first parser.</param>
    /// <param name="second">The second parser.</param>
    /// <returns>A parser returning the result of the first parser.</returns>
    public static IParser<T1> ThenSkip<T1, T2>(this IParser<T1> first, IParser<T2> second)
        => first.MustNotBeNull()
        .ThenAdd(second.MustNotBeNull())
        .Transform((l, _) => l);

    /// <summary>
    /// Creates a parser that applies two parsers and returns the result of the first one.
    /// </summary>
    /// <typeparam name="T1">The result type of the first parser.</typeparam>
    /// <typeparam name="T2">The result type of the second parser.</typeparam>
    /// <param name="first">The first parser.</param>
    /// <param name="second">The second parser.</param>
    /// <returns>A parser returning the result of the first parser.</returns>
    public static IParser<T1> ThenSkip<T1, T2>(this IParser<T1> first, Func<T1, IParser<T2>> second)
        => first.MustNotBeNull()
        .ThenAdd(second.MustNotBeNull())
        .Transform((l, _) => l);

    /// <summary>
    /// Creates a parser that applies the given parser but does not consume the input.
    /// </summary>
    /// <typeparam name="T">The result type of the given parser.</typeparam>
    /// <param name="parser">The given parser.</param>
    /// <returns>A parser applying the given parser that does not consume the input.</returns>
    public static IParser<T> Peek<T>(IParser<T> parser)
        => new PositiveLookaheadParser<T>(parser.MustNotBeNull());

    /// <summary>
    /// Creates a parser that applies a parser and then applies a different parser depending on the result.
    /// </summary>
    /// <typeparam name="TCondition">The result type of the attempted parser.</typeparam>
    /// <typeparam name="TBranches">The result type of the branch parsers.</typeparam>
    /// <param name="conditionParser">The condition parser.</param>
    /// <param name="thenParser">The then branch parser.</param>
    /// <param name="elseParser">The else branch parser.</param>
    /// <returns>A parser applying a parser based on a condition.</returns>
    public static IParser<TBranches> If<TCondition, TBranches>(IParser<TCondition> conditionParser, IParser<TBranches> thenParser, IParser<TBranches> elseParser)
        => Or(conditionParser.MustNotBeNull().Then(thenParser.MustNotBeNull()), elseParser.MustNotBeNull());

    /// <summary>
    /// Creates a parser that tries to parse something, but still proceeds if it fails.
    /// </summary>
    /// <typeparam name="T">The result type of the parser.</typeparam>
    /// <param name="parser">The parser.</param>
    /// <returns>A parser trying to apply a parser, but always proceeding.</returns>
    public static IParser<T?> Maybe<T>(IParser<T> parser)
        => Or<T?>(parser.MustNotBeNull(), VoidParser<T>.Instance);

    /// <summary>
    /// Creates a parser that tries to apply a given parser, but proceeds and returns a default value if it fails.
    /// </summary>
    /// <typeparam name="T">The result type of the parser.</typeparam>
    /// <param name="parser">The given parser.</param>
    /// <param name="defaultValue">The default value to return when the parser fails.</param>
    /// <returns>A parser applying a parser, but returning a default value if it fails.</returns>
    public static IParser<T> Maybe<T>(IParser<T> parser, T defaultValue)
        => Or(parser.MustNotBeNull(), Create(defaultValue));

    /// <summary>
    /// Creates a parser that always passes and creates an object.
    /// </summary>
    /// <typeparam name="T">The type of the parser result.</typeparam>
    /// <param name="value">The value to always return from the parser.</param>
    /// <returns>A parser always returning the object.</returns>
    public static IParser<T> Create<T>(T value)
        => VoidParser<T>.Instance.Transform(x => value);

    /// <summary>
    /// Creates a parser that applies the given parser and then expects the input stream to end.
    /// </summary>
    /// <typeparam name="T">The result type of the given parser.</typeparam>
    /// <param name="parser">The given parser.</param>
    /// <returns>A parser applying the given parser and then expects the input stream to end.</returns>
    public static IParser<T> ThenEnd<T>(this IParser<T> parser)
        => parser.MustNotBeNull().ThenSkip(End);

    /// <summary>
    /// Creates a parser that lazily applies a given parser allowing for recursion.
    /// </summary>
    /// <typeparam name="T">The result type of the given parser.</typeparam>
    /// <param name="parser">The given parser.</param>
    /// <returns>A parser that lazily applies a given parser.</returns>
    public static IParser<T> Lazy<T>(Func<IParser<T>> parser)
        => new LazyParser<T>(parser.MustNotBeNull());

    /// <summary>
    /// Creates a parser that replaces the nested expected values with a given expected name.
    /// </summary>
    /// <typeparam name="T">The result type of the given parser.</typeparam>
    /// <param name="parser">The given parser.</param>
    /// <param name="name">The name.</param>
    /// <returns>A parser that replaces the nested expected values with a given expected name.</returns>
    public static IParser<T> WithName<T>(this IParser<T> parser, string name)
        => parser.MustNotBeNull().WithNames(new[] { name });

    /// <summary>
    /// Creates a parser that replaces the nested expected values with given expected names.
    /// </summary>
    /// <typeparam name="T">The result type of the given parser.</typeparam>
    /// <param name="parser">The given parser.</param>
    /// <param name="names">The names.</param>
    /// <returns>A parser that replaces the nested expected values with given expected names.</returns>
    public static IParser<T> WithNames<T>(this IParser<T> parser, IEnumerable<string> names)
        => new ExpectedParser<T>(parser.MustNotBeNull(), names.MustNotBeNull());

    /// <summary>
    /// Creates a parser that replaces the nested expected values with given expected names.
    /// </summary>
    /// <typeparam name="T">The result type of the given parser.</typeparam>
    /// <param name="parser">The given parser.</param>
    /// <param name="firstName">The first name.</param>
    /// <param name="otherNames">The other names.</param>
    /// <returns>A parser that replaces the nested expected values with given expected names.</returns>
    public static IParser<T> WithNames<T>(this IParser<T> parser, string firstName, params string[] otherNames)
        => parser.MustNotBeNull().WithNames(new[] { firstName.MustNotBeNull() }.Concat(otherNames.MustNotBeNull()));

    /// <summary>
    /// Creates a parser that returns its inner <see cref="IParseResult{T}"/> directly.
    /// </summary>
    /// <typeparam name="T">The result type of the given parser.</typeparam>
    /// <param name="parser">The given parser.</param>
    /// <returns>A parser that returns its inner <see cref="IParseResult{T}"/> directly.</returns>
    public static IParser<IParseResult<T>> AsResult<T>(this IParser<T> parser)
        => new AsResultParser<T>(parser.MustNotBeNull());

    /// <summary>
    /// Creates a parser that attempts to parse something and if it fails attempt to recover.
    /// </summary>
    /// <typeparam name="TIn">The input type.</typeparam>
    /// <typeparam name="TOut">The output type.</typeparam>
    /// <param name="parser">The parser to try and use.</param>
    /// <param name="recoveryParser">The parser to use for recovery.</param>
    /// <param name="resultTransformation">The transformation to apply to the result.</param>
    /// <returns>A parsers that attempts to parse or recovers.</returns>
    public static IParser<TOut> Try<TIn, TOut>(IParser<TIn> parser, IParser<string> recoveryParser, Func<IParseResult<TIn>, TOut> resultTransformation)
        => Try(parser.MustNotBeNull(), recoveryParser.MustNotBeNull())
            .Transform(resultTransformation.MustNotBeNull());

    /// <summary>
    /// Creates a parser that attempts to parse something and if it fails attempt to recover.
    /// </summary>
    /// <typeparam name="TIn">The input type.</typeparam>
    /// <typeparam name="TOut">The output type.</typeparam>
    /// <param name="parser">The parser to try and use.</param>
    /// <param name="recoveryParser">The parser to use for recovery.</param>
    /// <param name="successTransformation">The transformation to apply to the succesful result.</param>
    /// <param name="failureTransformation">The transformation to apply to the failure result.</param>
    /// <returns>A parsers that attempts to parse or recovers.</returns>
    public static IParser<TOut> Try<TIn, TOut>(IParser<TIn> parser, IParser<string> recoveryParser, Func<TIn, TOut> successTransformation, Func<IImmutableList<IParseError>, TOut> failureTransformation)
        => Try(parser.MustNotBeNull(), recoveryParser.MustNotBeNull(), result =>
        {
            if (result.Success)
            {
                return successTransformation(result.Value!);
            }

            return failureTransformation(result.Errors!);
        });

    /// <summary>
    /// Creates a parser that attempts to parse something and if it fails attempt to recover.
    /// </summary>
    /// <typeparam name="T">The output type.</typeparam>
    /// <param name="parser">The parser to try and use.</param>
    /// <param name="recoveryParser">The parser to use for recovery.</param>
    /// <param name="failureTransformation">The transformation to apply to the failure result.</param>
    /// <returns>A parsers that attempts to parse or recovers.</returns>
    public static IParser<T> Try<T>(IParser<T> parser, IParser<string> recoveryParser, Func<IImmutableList<IParseError>, T> failureTransformation)
        => Try(parser.MustNotBeNull(), recoveryParser.MustNotBeNull(), value => value, error => failureTransformation(error));

    /// <summary>
    /// Creates a parser that attempts to parse something and if it fails attempt to recover.
    /// </summary>
    /// <typeparam name="T">The output type.</typeparam>
    /// <param name="parser">The parser to try and use.</param>
    /// <param name="recoveryParser">The parser to use for recovery.</param>
    /// <returns>A parser that attempts to parse or recovers.</returns>
    public static IParser<IParseResult<T>> Try<T>(IParser<T> parser, IParser<string> recoveryParser)
    {
        recoveryParser.MustNotBeNull();
        return parser.MustNotBeNull().AsResult().ThenSkip(r => r.Success switch
        {
            true => Pass<string>(),
            false => recoveryParser,
        });
    }
}
