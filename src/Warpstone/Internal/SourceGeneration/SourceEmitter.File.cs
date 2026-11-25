using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Warpstone.Internal.SourceGeneration
{
    /// <summary>
    /// The parts of the generated file that don't depend on the grammar.
    /// </summary>
    internal sealed partial class SourceEmitter
    {
        private string MemoClasses()
        {
            if (memoSlots.Count == 0)
            {
                return string.Empty;
            }

            var classes = new StringBuilder();
            classes.Append("""
        /// <summary>
        /// The memo entries of one position, one field per memo table.
        /// </summary>
        private sealed class __Row
        {

""");
            for (var slot = 0; slot < memoSlots.Count; slot++)
            {
                classes.Append($"            public __Entry<{memoSlots[slot]}> M{slot};\n\n");
            }

            classes.Append("""
        }

        /// <summary>
        /// The rows of the memo tables, in chunks of 256 positions, so no array grows onto the large object heap.
        /// Growing and memoizing parsers look up one position many times in a row, so the last row found is kept.
        /// </summary>
        private sealed class __Rows
        {
            private __Row[][] chunks = new __Row[16][];
            private int lastPosition = -1;
            private __Row lastRow;

            public __Row Find(int position)
            {
                if (position == lastPosition)
                {
                    return lastRow;
                }

                var index = position >> 8;
                var row = index < chunks.Length && chunks[index] != null ? chunks[index][position & 255] : null;
                lastPosition = position;
                lastRow = row;
                return row;
            }

            public __Row FindOrAdd(int position)
            {
                var row = Find(position);
                if (row != null)
                {
                    return row;
                }

                var index = position >> 8;
                if (index >= chunks.Length)
                {
                    global::System.Array.Resize(ref chunks, global::System.Math.Max(index + 1, chunks.Length * 2));
                }

                var chunk = chunks[index];
                if (chunk == null)
                {
                    chunk = new __Row[256];
                    chunks[index] = chunk;
                }

                row = new __Row();
                chunk[position & 255] = row;
                lastRow = row;
                return row;
            }
        }


""");
            for (var slot = 0; slot < memoSlots.Count; slot++)
            {
                classes.Append($$"""
        private sealed class __Table{{slot}}
        {
            private readonly __Rows rows;

            public __Table{{slot}}(__Rows rows)
            {
                this.rows = rows;
            }

            public __Entry<{{memoSlots[slot]}}> this[int position]
            {
                get
                {
                    TryGetValue(position, out var entry);
                    return entry;
                }

                set => rows.FindOrAdd(position).M{{slot}} = value;
            }

            public bool TryGetValue(int position, out __Entry<{{memoSlots[slot]}}> entry)
            {
                var row = rows.Find(position);
                if (row != null && row.M{{slot}}.State != 0)
                {
                    entry = row.M{{slot}};
                    return true;
                }

                entry = default;
                return false;
            }
        }


""");
            }

            return classes.ToString();
        }

        private string TryParseBody(string rootMethod)
        {
            if (trackAlways)
            {
                return $$"""
            var parser = new __Parser(input, true);
            if (parser.{{rootMethod}}(0, out _, out value))
            {
                error = null;
                return true;
            }

            error = parser.CreateError();
            return false;
""";
            }

            return $$"""
            // Tracking failures costs about as much as parsing, and only a failed parse needs them.
            // Parsing is deterministic, so parsing again while tracking fails the same way and finds the same failures.
            if (new __Parser(input, false).{{rootMethod}}(0, out _, out value))
            {
                error = null;
                return true;
            }

            var tracking = new __Parser(input, true);
            tracking.{{rootMethod}}(0, out _, out _);
            error = tracking.CreateError();
            return false;
""";
        }

        private string EmitFile(Type resultType, string rootMethod, string @namespace, string className, IEnumerable<string> usings)
        {
            var type = TypeName(resultType);
            var file = new StringBuilder();
            file.Append("// <auto-generated/>\n");
            file.Append("// Generated by Warpstone. The parser below doesn't depend on Warpstone.\n");
            file.Append("#pragma warning disable\n");
            file.Append("#nullable enable annotations\n");
            file.Append("#nullable disable warnings\n\n");

            // Transformations often use these, like implicit usings do. The generated code is fully qualified, so they can't clash with it.
            foreach (var import in new[] { "System", "System.Collections.Generic", "System.Linq" }.Concat(usings).Distinct(StringComparer.Ordinal))
            {
                file.Append($"using {import};\n");
            }

            file.Append($$"""

namespace {{@namespace}}
{
    static partial class {{className}}
    {
        /// <summary>
        /// Tries to parse <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="value">The parsed value, if parsing succeeded.</param>
        /// <returns><see langword="true"/> if parsing succeeded.</returns>
        public static bool TryParse(string input, out {{type}} value)
        {
            if (input is null)
            {
                throw new global::System.ArgumentNullException(nameof(input));
            }

            return new __Parser(input, false).{{rootMethod}}(0, out _, out value);
        }

        /// <summary>
        /// Tries to parse <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="value">The parsed value, if parsing succeeded.</param>
        /// <param name="error">The error, if parsing failed.</param>
        /// <returns><see langword="true"/> if parsing succeeded.</returns>
        public static bool TryParse(string input, out {{type}} value, out ParseError? error)
        {
            if (input is null)
            {
                throw new global::System.ArgumentNullException(nameof(input));
            }

{{TryParseBody(rootMethod)}}
        }

        /// <summary>
        /// Parses <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The parsed value.</returns>
        /// <exception cref="global::System.FormatException">Thrown when parsing failed.</exception>
        public static {{type}} Parse(string input)
            => TryParse(input, out var value, out var error) ? value : throw new global::System.FormatException(error!.ToString());

        /// <summary>
        /// Describes why parsing failed.
        /// </summary>
        public sealed class ParseError
        {
            internal ParseError(int position, global::System.Collections.Generic.IReadOnlyList<string> expected, global::System.Collections.Generic.IReadOnlyList<global::System.Exception> exceptions)
            {
                Position = position;
                Expected = expected;
                Exceptions = exceptions;
            }

            /// <summary>
            /// Gets the furthest position at which parsing failed.
            /// </summary>
            public int Position { get; }

            /// <summary>
            /// Gets what was expected at <see cref="Position"/>.
            /// </summary>
            public global::System.Collections.Generic.IReadOnlyList<string> Expected { get; }

            /// <summary>
            /// Gets the exceptions thrown by transformations at <see cref="Position"/>.
            /// </summary>
            public global::System.Collections.Generic.IReadOnlyList<global::System.Exception> Exceptions { get; }

            /// <inheritdoc />
            public override string ToString()
                => Exceptions.Count > 0
                ? $"Parsing failed at position {Position}: {Exceptions[0].Message}"
                : $"Expected {string.Join(", ", Expected)} at position {Position}.";
        }

        private readonly struct __Entry<T>
        {
            public __Entry(byte state, int end, T value)
            {
                State = state;
                End = end;
                Value = value;
            }

            /// <summary>1 while in progress, 2 after a failure, 3 after a success.</summary>
            public readonly byte State;

            public readonly int End;

            public readonly T Value;
        }

        private sealed class __CycleState
        {
            public readonly global::System.Collections.Generic.HashSet<int> InProgress = new global::System.Collections.Generic.HashSet<int>();

            public readonly global::System.Collections.Generic.HashSet<int> GrowRequested = new global::System.Collections.Generic.HashSet<int>();

            public readonly global::System.Collections.Generic.Stack<global::System.Collections.Generic.HashSet<int>> Suspended = new global::System.Collections.Generic.Stack<global::System.Collections.Generic.HashSet<int>>();

            public global::System.Collections.Generic.HashSet<int> Limits;
        }


{{statics}}{{MemoClasses()}}        private sealed class __Parser
        {
            private readonly string input;
            private readonly bool track;
            private object[] failures = new object[16];
            private int failureCount;
            private int failPos = -1;
            private int suppressed;
            private string?[]? interned;
{{fields}}
            public __Parser(string input, bool track)
            {
                this.input = input;
                this.track = track;
{{constructor}}            }

            public ParseError CreateError()
            {
                var expected = new global::System.Collections.Generic.List<string>();
                var exceptions = new global::System.Collections.Generic.List<global::System.Exception>();
                for (var i = 0; i < failureCount; i++)
                {
                    if (failures[i] is string text)
                    {
                        if (!expected.Contains(text))
                        {
                            expected.Add(text);
                        }
                    }
                    else
                    {
                        exceptions.Add((global::System.Exception)failures[i]);
                    }
                }

                return new ParseError(failureCount == 0 ? 0 : failPos, expected, exceptions);
            }

            private bool MoveTo(int position)
            {
                if (suppressed > 0 || position < failPos)
                {
                    return false;
                }

                if (position > failPos)
                {
                    failPos = position;
                    global::System.Array.Clear(failures, 0, failureCount);
                    failureCount = 0;
                }

                return true;
            }

            // Matched text that repeats, like identifiers, shares one string. Longer matches rarely repeat, so they skip the lookup.
            // The table is direct-mapped by hash: a collision overwrites the slot. It has a slot per 16 characters of input, at most 512,
            // so it costs little on small inputs.
            private string Intern(int start, int length)
            {
                if (length > 32)
                {
                    return input.Substring(start, length);
                }

                var hash = length;
                for (var i = start; i < start + length; i++)
                {
                    hash = unchecked((hash * 31) + input[i]);
                }

                var table = interned ??= CreateInternTable();
                var slot = hash & (table.Length - 1);
                var known = table[slot];
                if (known != null && known.Length == length && string.CompareOrdinal(input, start, known, 0, length) == 0)
                {
                    return known;
                }

                return table[slot] = input.Substring(start, length);
            }

            private string?[] CreateInternTable()
            {
                var size = 16;
                while (size < 512 && size * 16 < input.Length)
                {
                    size *= 2;
                }

                return new string?[size];
            }

            [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            private void Expect(int position, string expected)
            {
                if (track)
                {
                    Track(position, expected);
                }
            }

            private void Track(int position, string expected)
            {
                // Most failures happen behind the furthest one, so this check comes first.
                if (position < failPos || !MoveTo(position))
                {
                    return;
                }

                // The expected texts are literals, so the same text is the same instance.
                for (var i = 0; i < failureCount; i++)
                {
                    if (ReferenceEquals(failures[i], expected))
                    {
                        return;
                    }
                }

                Add(expected);
            }

            private void Error(int position, global::System.Exception exception)
            {
                if (track && MoveTo(position))
                {
                    Add(exception);
                }
            }

            private void Add(object failure)
            {
                if (failureCount == failures.Length)
                {
                    global::System.Array.Resize(ref failures, failureCount * 2);
                }

                failures[failureCount++] = failure;
            }

            private void EndLabel(int startPos, int startCount, int position, string name)
            {
                // Failures that got further than the start are more precise than the name, so they are kept.
                if (!track || failPos > position)
                {
                    return;
                }

                if (failPos == position && suppressed == 0)
                {
                    var keep = startPos == position ? startCount : 0;
                    global::System.Array.Clear(failures, keep, failureCount - keep);
                    failureCount = keep;
                }

                Expect(position, name);
            }

            private static bool Improved<T>(in __Entry<T> previous, bool success, int end)
                => previous.State != 3 ? success : success && end > previous.End;

            private static global::System.Collections.Generic.IReadOnlyList<T> AddToList<T>(global::System.Collections.Generic.IReadOnlyList<T> list, T element)
            {
                if (list is global::System.Collections.Generic.List<T> mutable)
                {
                    mutable.Add(element);
                    return mutable;
                }

                return new global::System.Collections.Generic.List<T> { element };
            }

""");

            file.Append(string.Join("\n", methods.Values));
            file.Append("        }\n");
            file.Append("    }\n");
            file.Append("}\n");
            return file.ToString();
        }
    }
}
