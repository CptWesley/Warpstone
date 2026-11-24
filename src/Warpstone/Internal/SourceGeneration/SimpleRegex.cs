using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

// S127: the scanning loops advance their index over runs on purpose. S2094: nodes without data are told apart by type.
// S3398: the character sets are shared by the parser, the safety check and the writer.
#pragma warning disable S127, S2094, S3398

namespace Warpstone.Internal.SourceGeneration
{
    /// <summary>
    /// Turns simple regular expressions into hand-written C# scanning loops for the generated source.
    /// Supported: literals, character classes and escapes with quantifiers, <c>\b</c>, non-capturing groups,
    /// and negative lookaheads (whose alternatives may use <c>|</c>). Everything else stays a <see cref="Regex"/>.
    /// </summary>
    /// <remarks>
    /// The loops are greedy and never backtrack, so a pattern is only converted when backtracking can't change the result:
    /// a repeated character set must not overlap with anything that can follow it.
    /// Which characters a set matches is found by asking the <see cref="Regex"/> engine itself about every character,
    /// so the Unicode semantics of <c>\w</c>, <c>\d</c>, <c>\s</c> and <c>\p{..}</c> are exactly those of .NET.
    /// </remarks>
    internal static class SimpleRegex
    {
        private const int CharCount = char.MaxValue + 1;
        private const int MaxAlternatives = 256;

        private const RegexOptions SupportedOptions = RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant
            | RegexOptions.Singleline | RegexOptions.Multiline;

        private static readonly ConcurrentDictionary<(string Atom, RegexOptions Options), bool[]> Sets = new();

        /// <summary>
        /// Tries to write a scanner for <paramref name="pattern"/>: a static method <c>int __R{n}(string input, int pos)</c>
        /// returning the end of the match at <c>pos</c>, or <c>-1</c>.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="options">The options of the regular expression.</param>
        /// <param name="emitter">The emitter to write the methods to.</param>
        /// <param name="method">The name of the scanner method.</param>
        /// <returns><see langword="true"/> if the pattern could be converted.</returns>
        public static bool TryEmit(string pattern, RegexOptions options, SourceEmitter emitter, out string method)
        {
            method = string.Empty;
            if ((options & ~SupportedOptions) != 0)
            {
                return false;
            }

            options &= ~RegexOptions.Compiled;
            List<List<Node>> alternatives;
            try
            {
                var parser = new PatternParser(pattern, options);
                alternatives = parser.ParseAlternation();
                if (!parser.AtEnd)
                {
                    return false;
                }
            }
            catch (Exception e) when (e is FormatException or ArgumentException)
            {
                return false;
            }

            // Alternation in the pattern itself picks the first alternative that matches, not the longest, so it is not supported.
            if (alternatives.Count != 1 || alternatives[0].Any(n => n is GroupNode) || !IsSafe(alternatives[0]))
            {
                return false;
            }

            try
            {
                return new ScannerWriter(emitter, options).TryWrite(alternatives[0], out method);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static bool[] SetOf(string atom, RegexOptions options)
            => Sets.GetOrAdd((atom, options), key =>
            {
                var regex = new Regex($@"^(?:{key.Atom})\z", key.Options, TimeSpan.FromSeconds(10));
                var members = new bool[CharCount];
                for (var c = 0; c < CharCount; c++)
                {
                    members[c] = regex.IsMatch(((char)c).ToString());
                }

                return members;
            });

        /// <summary>
        /// The characters that are word characters for <c>\b</c>: those with no boundary between them and a letter.
        /// </summary>
        private static bool[] WordChars(RegexOptions options)
            => Sets.GetOrAdd((@"\b", options), key =>
            {
                var regex = new Regex(@"^a\B", key.Options, TimeSpan.FromSeconds(10));
                var members = new bool[CharCount];
                for (var c = 0; c < CharCount; c++)
                {
                    members[c] = regex.IsMatch("a" + (char)c);
                }

                return members;
            });

#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
        private static bool IsSafe(List<Node> sequence)
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
        {
            for (var k = 0; k < sequence.Count; k++)
            {
                switch (sequence[k])
                {
                    case LookaheadNode lookahead:
                        // Backtracking into a repetition before it would move it, so it must follow something of fixed length.
                        if (k > 0 && sequence[k - 1] is SetNode { IsFixed: false })
                        {
                            return false;
                        }

                        if (!lookahead.Alternatives.All(IsSafe))
                        {
                            return false;
                        }

                        break;

                    case SetNode { IsFixed: false } set:
                        if (k + 1 < sequence.Count && sequence[k + 1] is BoundaryNode)
                        {
                            // Giving back characters only moves the boundary between two characters of the set,
                            // which is never a boundary when the set holds only word characters, or none.
                            var word = WordChars(set.Options);
                            var allWord = true;
                            var noWord = true;
                            for (var c = 0; c < CharCount; c++)
                            {
                                if (set.Members[c])
                                {
                                    allWord &= word[c];
                                    noWord &= !word[c];
                                }
                            }

                            if (set.Min < 1 || !(allWord || noWord))
                            {
                                return false;
                            }

                            break;
                        }

                        // Giving back characters only helps when what follows can start with one of them.
                        for (var j = k + 1; j < sequence.Count; j++)
                        {
                            if (sequence[j] is not SetNode next)
                            {
                                return false;
                            }

                            for (var c = 0; c < CharCount; c++)
                            {
                                if (set.Members[c] && next.Members[c])
                                {
                                    return false;
                                }
                            }

                            if (next.Min > 0)
                            {
                                break;
                            }
                        }

                        break;
                }
            }

            return true;
        }

        private abstract class Node
        {
        }

        /// <summary>
        /// A character set repeated between <see cref="Min"/> and <see cref="Max"/> times.
        /// </summary>
        private sealed class SetNode : Node
        {
            public SetNode(bool[] members, int min, int max, RegexOptions options)
            {
                Members = members;
                Min = min;
                Max = max;
                Options = options;
            }

            public bool[] Members { get; }

            public int Min { get; }

            public int Max { get; }

            public RegexOptions Options { get; }

            public bool IsFixed => Min == Max;

            public char? Single
            {
                get
                {
                    char? single = null;
                    for (var c = 0; c < CharCount; c++)
                    {
                        if (Members[c])
                        {
                            if (single is not null)
                            {
                                return null;
                            }

                            single = (char)c;
                        }
                    }

                    return single;
                }
            }
        }

        private sealed class BoundaryNode : Node
        {
        }

        /// <summary>
        /// A negative lookahead: fails when any of the alternatives matches.
        /// </summary>
        private sealed class LookaheadNode : Node
        {
            public LookaheadNode(List<List<Node>> alternatives)
            {
                Alternatives = alternatives;
            }

            public List<List<Node>> Alternatives { get; }
        }

        /// <summary>
        /// A group with several alternatives, only allowed inside a lookahead, where it is expanded.
        /// </summary>
        private sealed class GroupNode : Node
        {
            public GroupNode(List<List<Node>> alternatives)
            {
                Alternatives = alternatives;
            }

            public List<List<Node>> Alternatives { get; }
        }

        /// <summary>
        /// Parses the supported subset. Throws a <see cref="FormatException"/> for anything else.
        /// </summary>
        private sealed class PatternParser
        {
            private readonly string pattern;
            private readonly RegexOptions options;
            private int index;

            public PatternParser(string pattern, RegexOptions options)
            {
                this.pattern = pattern;
                this.options = options;
            }

            public bool AtEnd => index == pattern.Length;

            public List<List<Node>> ParseAlternation()
            {
                var alternatives = new List<List<Node>> { ParseSequence() };
                while (Peek() == '|')
                {
                    index++;
                    alternatives.Add(ParseSequence());
                }

                return alternatives;
            }

#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
            private List<Node> ParseSequence()
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
            {
                var sequence = new List<Node>();
                while (!AtEnd && Peek() is not ('|' or ')'))
                {
                    var c = pattern[index];
                    if (c == '(')
                    {
                        sequence.AddRange(ParseGroup());
                        continue;
                    }

                    string atom;
                    if (c == '\\')
                    {
                        if (Peek(1) == 'b')
                        {
                            index += 2;
                            sequence.Add(new BoundaryNode());
                            continue;
                        }

                        atom = ParseEscape();
                    }
                    else if (c == '[')
                    {
                        atom = ParseClass();
                    }
                    else if (c == '.')
                    {
                        index++;
                        atom = ".";
                    }
                    else if (c is '^' or '$' or '*' or '+' or '?' or '{' or '}' or ']')
                    {
                        throw new FormatException();
                    }
                    else
                    {
                        index++;
                        atom = Regex.Escape(c.ToString());
                    }

                    var (min, max) = ParseQuantifier();
                    sequence.Add(new SetNode(SetOf(atom, options), min, max, options));
                }

                return sequence;
            }

            private IEnumerable<Node> ParseGroup()
            {
                var lookahead = Peek(1) == '?' && Peek(2) == '!';
                var nonCapturing = Peek(1) == '?' && Peek(2) == ':';
                if (lookahead || nonCapturing)
                {
                    index += 3;
                }
                else if (Peek(1) != '?' && (options & RegexOptions.ExplicitCapture) != 0)
                {
                    index++;
                }
                else
                {
                    throw new FormatException();
                }

                var alternatives = ParseAlternation();
                if (Peek() != ')')
                {
                    throw new FormatException();
                }

                index++;
                if (Peek() is '*' or '+' or '?' or '{')
                {
                    throw new FormatException();
                }

                if (lookahead)
                {
                    return [new LookaheadNode(Expand(alternatives))];
                }

                return alternatives.Count == 1 ? alternatives[0] : [new GroupNode(alternatives)];
            }

            /// <summary>
            /// Expands groups with alternatives into separate alternatives. Inside a lookahead only whether one matches counts, so this is safe.
            /// </summary>
            private static List<List<Node>> Expand(List<List<Node>> alternatives)
            {
                var result = new List<List<Node>>();
                foreach (var alternative in alternatives)
                {
                    var expanded = new List<List<Node>> { new() };
                    foreach (var node in alternative)
                    {
                        if (node is GroupNode group)
                        {
                            var options = Expand(group.Alternatives);
                            expanded = expanded.SelectMany(prefix => options.Select(option => prefix.Concat(option).ToList())).ToList();
                        }
                        else
                        {
                            expanded.ForEach(prefix => prefix.Add(node));
                        }

                        if (expanded.Count > MaxAlternatives)
                        {
                            throw new FormatException();
                        }
                    }

                    result.AddRange(expanded);
                }

                return result;
            }

            private string ParseEscape()
            {
                var start = index;
                var c = Peek(1);
                switch (c)
                {
                    case 'd' or 'D' or 'w' or 'W' or 's' or 'S' or 't' or 'n' or 'r' or 'f' or 'v' or 'e' or 'a':
                        index += 2;
                        break;
                    case 'p' or 'P':
                        var close = pattern.IndexOf('}', index);
                        if (Peek(2) != '{' || close < 0)
                        {
                            throw new FormatException();
                        }

                        index = close + 1;
                        break;
                    case 'x':
                        index += 4;
                        break;
                    case 'u':
                        index += 6;
                        break;
                    case '\0':
                        throw new FormatException();
                    default:
                        // Escaped punctuation is a literal; escaped letters and digits are assertions, back references and the like.
                        if (char.IsLetterOrDigit(c))
                        {
                            throw new FormatException();
                        }

                        index += 2;
                        break;
                }

                if (index > pattern.Length)
                {
                    throw new FormatException();
                }

                return pattern.Substring(start, index - start);
            }

            private string ParseClass()
            {
                var start = index;
                var i = index + 1;
                if (i < pattern.Length && pattern[i] == '^')
                {
                    i++;
                }

                if (i < pattern.Length && pattern[i] == ']')
                {
                    i++;
                }

                while (i < pattern.Length && pattern[i] != ']')
                {
                    // Character class subtraction, like [a-z-[aeiou]], is not supported.
                    if (pattern[i] == '[')
                    {
                        throw new FormatException();
                    }

                    i += pattern[i] == '\\' ? 2 : 1;
                }

                if (i >= pattern.Length)
                {
                    throw new FormatException();
                }

                index = i + 1;
                return pattern.Substring(start, index - start);
            }

            private (int Min, int Max) ParseQuantifier()
            {
                (int, int) quantifier;
                switch (Peek())
                {
                    case '*':
                        index++;
                        quantifier = (0, int.MaxValue);
                        break;
                    case '+':
                        index++;
                        quantifier = (1, int.MaxValue);
                        break;
                    case '?':
                        index++;
                        quantifier = (0, 1);
                        break;
                    case '{':
                        quantifier = ParseCounts();
                        break;
                    default:
                        return (1, 1);
                }

                // Lazy quantifiers prefer shorter matches.
                if (Peek() == '?')
                {
                    throw new FormatException();
                }

                return quantifier;
            }

            private (int Min, int Max) ParseCounts()
            {
                var close = pattern.IndexOf('}', index);
                if (close < 0)
                {
                    throw new FormatException();
                }

                var parts = pattern.Substring(index + 1, close - index - 1).Split(',');
                index = close + 1;
                var min = int.Parse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture);
                var max = parts.Length switch
                {
                    1 => min,
                    2 when parts[1].Length == 0 => int.MaxValue,
                    2 => int.Parse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture),
                    _ => throw new FormatException(),
                };

                return max < min ? throw new FormatException() : (min, max);
            }

            private char Peek(int offset = 0)
                => index + offset < pattern.Length ? pattern[index + offset] : '\0';
        }

        /// <summary>
        /// Writes the scanner methods.
        /// </summary>
        private sealed class ScannerWriter
        {
            private readonly SourceEmitter emitter;
            private readonly RegexOptions options;
            private string? boundary;

            public ScannerWriter(SourceEmitter emitter, RegexOptions options)
            {
                this.emitter = emitter;
                this.options = options;
            }

            public bool TryWrite(List<Node> sequence, out string method)
            {
                var code = new StringBuilder();
                if (!TryWriteSequence(sequence, code, "            "))
                {
                    method = string.Empty;
                    return false;
                }

                var body = code.ToString();
                method = emitter.Helper("regex " + body, name => $$"""
        private static int {{name}}(string input, int pos)
        {
            var i = pos;
{{body}}            return i;
        }


""");
                return true;
            }

#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
            private bool TryWriteSequence(List<Node> sequence, StringBuilder code, string indent)
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
            {
                for (var k = 0; k < sequence.Count; k++)
                {
                    switch (sequence[k])
                    {
                        case BoundaryNode:
                            code.Append($"{indent}if (!{Boundary()}(input, i))\n{indent}{{\n{indent}    return -1;\n{indent}}}\n\n");
                            break;

                        case LookaheadNode lookahead:
                            if (!TryWriteLookahead(lookahead, code, indent))
                            {
                                return false;
                            }

                            break;

                        case SetNode set when set.Min == 1 && set.Max == 1 && set.Single is not null:
                            // A run of literal characters is compared at once.
                            var literal = new StringBuilder();
                            while (k < sequence.Count && sequence[k] is SetNode { Min: 1, Max: 1 } next && next.Single is { } c)
                            {
                                literal.Append(c);
                                k++;
                            }

                            k--;
                            code.Append(literal.Length == 1
                                ? $"{indent}if (i >= input.Length || input[i] != {emitter.Literal(literal[0])})\n"
                                : $"{indent}if (input.Length - i < {literal.Length} || string.CompareOrdinal(input, i, {emitter.Literal(literal.ToString())}, 0, {literal.Length}) != 0)\n");
                            code.Append($"{indent}{{\n{indent}    return -1;\n{indent}}}\n\n{indent}i += {literal.Length};\n\n");
                            break;

                        case SetNode set:
                            if (Predicate(set.Members, "input[i]") is not { } predicate)
                            {
                                return false;
                            }

                            if (set.Min == 1 && set.Max == 1)
                            {
                                code.Append($"{indent}if (i >= input.Length || !({predicate}))\n{indent}{{\n{indent}    return -1;\n{indent}}}\n\n{indent}i++;\n\n");
                                break;
                            }

                            var start = $"start{k}";
                            var limit = set.Max == int.MaxValue ? "input.Length" : $"(input.Length - i > {set.Max} ? i + {set.Max} : input.Length)";
                            code.Append($"{indent}var {start} = i;\n");
                            code.Append($"{indent}var limit{k} = {limit};\n");
                            code.Append($"{indent}while (i < limit{k} && ({predicate}))\n{indent}{{\n{indent}    i++;\n{indent}}}\n\n");
                            if (set.Min > 0)
                            {
                                code.Append($"{indent}if (i - {start} < {set.Min})\n{indent}{{\n{indent}    return -1;\n{indent}}}\n\n");
                            }

                            break;

                        default:
                            return false;
                    }
                }

                return true;
            }

            private bool TryWriteLookahead(LookaheadNode lookahead, StringBuilder code, string indent)
            {
                var conditions = new List<(char? First, string Condition)>();
                foreach (var alternative in lookahead.Alternatives)
                {
                    if (TryLiteralCondition(alternative, out var first, out var condition))
                    {
                        conditions.Add((first, condition));
                        continue;
                    }

                    var nested = new ScannerWriter(emitter, options);
                    if (!nested.TryWrite(alternative, out var method))
                    {
                        return false;
                    }

                    conditions.Add((null, $"{method}(input, i) >= 0"));
                }

                // Many literal alternatives, like keywords, are only compared when their first character is next.
                if (conditions.Count >= 4 && conditions.All(c => c.First is not null))
                {
                    code.Append($"{indent}if (i < input.Length)\n{indent}{{\n{indent}    switch (input[i])\n{indent}    {{\n");
                    foreach (var group in conditions.GroupBy(c => c.First!.Value))
                    {
                        code.Append($"{indent}        case {emitter.Literal(group.Key)}:\n");
                        code.Append($"{indent}            if ({string.Join(" || ", group.Select(c => c.Condition))})\n{indent}            {{\n{indent}                return -1;\n{indent}            }}\n\n");
                        code.Append($"{indent}            break;\n");
                    }

                    code.Append($"{indent}    }}\n{indent}}}\n\n");
                    return true;
                }

                foreach (var (_, condition) in conditions)
                {
                    code.Append($"{indent}if ({condition})\n{indent}{{\n{indent}    return -1;\n{indent}}}\n\n");
                }

                return true;
            }

            /// <summary>
            /// Writes an alternative of literal characters, optionally followed by <c>\b</c>, as one condition.
            /// </summary>
            private bool TryLiteralCondition(List<Node> alternative, out char? first, out string condition)
            {
                first = null;
                condition = string.Empty;
                var literal = new StringBuilder();
                var count = alternative.Count;
                var trailingBoundary = count > 0 && alternative[count - 1] is BoundaryNode;
                for (var k = 0; k < (trailingBoundary ? count - 1 : count); k++)
                {
                    if (alternative[k] is not SetNode { Min: 1, Max: 1 } set || set.Single is not { } c)
                    {
                        return false;
                    }

                    literal.Append(c);
                }

                if (literal.Length == 0)
                {
                    return false;
                }

                first = literal[0];
                condition = $"(input.Length - i >= {literal.Length} && string.CompareOrdinal(input, i, {emitter.Literal(literal.ToString())}, 0, {literal.Length}) == 0"
                    + (trailingBoundary ? $" && {Boundary()}(input, i + {literal.Length}))" : ")");
                return true;
            }

            /// <summary>
            /// Gets the helper telling whether there is a word boundary at an index.
            /// </summary>
            private string Boundary()
            {
                if (boundary is not null)
                {
                    return boundary;
                }

                var predicate = Predicate(WordChars(options), "c") ?? throw new FormatException();
                var isWord = emitter.Helper($"word {predicate}", name => $$"""
        private static bool {{name}}(char c)
            => {{predicate}};


""");
                boundary = emitter.Helper($"boundary {isWord}", name => $$"""
        private static bool {{name}}(string input, int index)
            => (index > 0 && {{isWord}}(input[index - 1])) != (index < input.Length && {{isWord}}(input[index]));


""");
                return boundary;
            }

            private string Range(int from, int to, string c)
            {
                if (from == to)
                {
                    return $"{c} == {emitter.Literal((char)from)}";
                }

                return to == char.MaxValue
                    ? $"{c} >= {emitter.Literal((char)from)}"
                    : $"({c} >= {emitter.Literal((char)from)} && {c} <= {emitter.Literal((char)to)})";
            }

            /// <summary>
            /// Writes the C# condition testing whether <paramref name="c"/> is one of <paramref name="members"/>,
            /// or returns <see langword="null"/> if the set is too irregular.
            /// </summary>
#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
            private string? Predicate(bool[] members, string c)
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
            {
                var ranges = new List<(int From, int To)>();
                for (var i = 0; i < CharCount; i++)
                {
                    if (!members[i])
                    {
                        continue;
                    }

                    var from = i;
                    while (i + 1 < CharCount && members[i + 1])
                    {
                        i++;
                    }

                    ranges.Add((from, i));
                }

                if (ranges.Count == 0)
                {
                    return "false";
                }

                if (ranges.Count <= 4)
                {
                    return string.Join(" || ", ranges.Select(r => Range(r.From, r.To, c)));
                }

                // ASCII from a bit mask; the rest by Unicode category, if every category is either fully in or fully out.
                ulong low = 0;
                ulong high = 0;
                for (var i = 0; i < 128; i++)
                {
                    if (members[i])
                    {
                        if (i < 64)
                        {
                            low |= 1UL << i;
                        }
                        else
                        {
                            high |= 1UL << (i - 64);
                        }
                    }
                }

                // Each category goes in or out by majority; the characters that disagree are listed.
                var counts = new int[32, 2];
                for (var i = 128; i < CharCount; i++)
                {
                    counts[(int)CharUnicodeInfo.GetUnicodeCategory((char)i), members[i] ? 1 : 0]++;
                }

                var mask = 0;
                for (var i = 0; i < 32; i++)
                {
                    if (counts[i, 1] > counts[i, 0])
                    {
                        mask |= 1 << i;
                    }
                }

                var exceptions = new List<char>();
                for (var i = 128; i < CharCount; i++)
                {
                    var byCategory = (mask & (1 << (int)CharUnicodeInfo.GetUnicodeCategory((char)i))) != 0;
                    if (byCategory != members[i])
                    {
                        exceptions.Add((char)i);
                        if (exceptions.Count > 16)
                        {
                            return null;
                        }
                    }
                }

                var ascii = $"((({c} < 64 ? 0x{low:X}UL >> {c} : 0x{high:X}UL >> ({c} - 64)) & 1) != 0)";
                var other = mask == 0 ? "false" : $"((1 << (int)global::System.Globalization.CharUnicodeInfo.GetUnicodeCategory({c})) & 0x{mask:X}) != 0";
                if (exceptions.Count > 0)
                {
                    other = $"(({other}) ^ ({string.Join(" || ", exceptions.Select(e => $"{c} == {emitter.Literal(e)}"))}))";
                }

                return other == "false" ? $"({c} < 128 && {ascii})" : $"({c} < 128 ? {ascii} : {other})";
            }
        }
    }
}
