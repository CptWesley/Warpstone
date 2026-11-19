using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Warpstone.Internal.Input
{
    /// <summary>
    /// Represents information for inputs used by parsers.
    /// Basic concrete implementation of the <see cref="IParseInput"/> interface.
    /// </summary>
    internal sealed class StringParseInput : IParseInput
    {
        private static readonly string[] Spaces = CreateRuns(string.Empty);
        private static readonly string[] LineFeedSpaces = CreateRuns("\n");
        private static readonly string[] CrLfSpaces = CreateRuns("\r\n");

        private int[]? lineStarts;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringParseInput"/> class.
        /// </summary>
        /// <param name="source">The source of the input.</param>
        /// <param name="content">The input string.</param>
        /// <param name="lineStarts">The start index of each line, or <see langword="null"/> to find them when a position is first asked for.</param>
        public StringParseInput(IParseInputSource source, string content, int[]? lineStarts)
        {
            Source = source;
            Content = content;
            this.lineStarts = lineStarts;
        }

        /// <inheritdoc />
        public string Content { get; }

        /// <inheritdoc />
        public IParseInputSource Source { get; }

        /// <inheritdoc />
        public ParseInputPosition GetPosition(int index)
        {
            // Only errors and results ask for positions, so a successful parse never builds the line index.
            var lineLengths = lineStarts ??= FindLineStarts(Content);
            var start = 0;
            var end = lineLengths.Length;
            var range = end;

            while (range > 1)
            {
                var halfRange = range / 2;
                var pivot = start + halfRange;

                var startIndex = lineLengths[pivot];

                if (index < startIndex)
                {
                    end -= halfRange;
                    range = end - start;
                }
                else if (index == startIndex)
                {
                    return new ParseInputPosition(this, index, pivot + 1, 1);
                }
                else if (index > startIndex)
                {
                    if (pivot == lineLengths.Length - 1)
                    {
                        return new ParseInputPosition(this, index, pivot + 1, index - startIndex + 1);
                    }

                    var nextStartIndex = lineLengths[pivot + 1];
                    if (index < nextStartIndex)
                    {
                        return new ParseInputPosition(this, index, pivot + 1, index - startIndex + 1);
                    }

                    start += halfRange;
                    range = end - start;
                }
            }

            return new ParseInputPosition(this, index, start + 1, index - lineLengths[start] + 1);
        }

        /// <summary>
        /// Finds the start of every line, plus one past the end of the content, as splitting on <c>'\n'</c> would.
        /// </summary>
        private static int[] FindLineStarts(string content)
        {
            var starts = new List<int> { 0 };
            for (var i = content.IndexOf('\n'); i >= 0; i = content.IndexOf('\n', i + 1))
            {
                starts.Add(i + 1);
            }

            starts.Add(content.Length + 1);
            return starts.ToArray();
        }

        /// <inheritdoc />
        public InputMatch Match(int index, char character)
            => index < Content.Length && Content[index] == character
            ? InputMatch.Success
            : InputMatch.Failure;

        /// <inheritdoc />
        public InputMatch Match(int index, string str, CultureInfo culture, CompareOptions options, out string? match)
        {
            if (index + str.Length > Content.Length
                || (options == CompareOptions.Ordinal
                    ? string.CompareOrdinal(Content, index, str, 0, str.Length) != 0
                    : string.Compare(Content, index, str, 0, str.Length, culture, options) != 0))
            {
                match = null;
                return InputMatch.Failure;
            }

            match = options == CompareOptions.Ordinal
                ? str
                : Content.Substring(index, str.Length);
            return InputMatch.Success;
        }

        /// <inheritdoc />
        public InputMatch Match(int index, Regex regex, out string? match)
        {
#if NET7_0_OR_GREATER
            var matches = regex.EnumerateMatches(Content, index);
            if (matches.MoveNext())
            {
                match = Content.Substring(matches.Current.Index, matches.Current.Length);
                return InputMatch.Success;
            }

            match = null;
            return InputMatch.Failure;
#else
            var result = regex.Match(Content, index);
            if (!result.Success)
            {
                match = null;
                return InputMatch.Failure;
            }

            match = result.Value;
            return InputMatch.Success;
#endif
        }

        /// <summary>
        /// Matches a regex like <see cref="Match(int, Regex, out string?)"/>, but returns <paramref name="first"/> instead of a new
        /// string when the match has the same text, and stores the match in it when it is still <see langword="null"/>.
        /// A regex that matches one fixed text, such as a keyword, then allocates once instead of on every match.
        /// </summary>
        /// <param name="index">The index to match at.</param>
        /// <param name="regex">The regex to match.</param>
        /// <param name="prefix">Literal text every match starts with, checked before running the regex.</param>
        /// <param name="first">The first text matched, shared by all parses. Only ever written while it is <see langword="null"/>.</param>
        /// <param name="match">The matched text.</param>
        /// <returns>Whether the regex matched.</returns>
        internal InputMatch Match(int index, Regex regex, string prefix, ref string? first, out string? match)
        {
            if (prefix.Length > 0 && string.CompareOrdinal(Content, index, prefix, 0, prefix.Length) != 0)
            {
                match = null;
                return InputMatch.Failure;
            }

#if NET7_0_OR_GREATER
            var matches = regex.EnumerateMatches(Content, index);
            if (matches.MoveNext())
            {
                var start = matches.Current.Index;
                var length = matches.Current.Length;
                if (first is { } known && known.Length == length && string.CompareOrdinal(Content, start, known, 0, length) == 0)
                {
                    match = known;
                }
                else
                {
                    match = Content.Substring(start, length);
                    first ??= match;
                }

                return InputMatch.Success;
            }

            match = null;
            return InputMatch.Failure;
#else
            return Match(index, regex, out match);
#endif
        }

        /// <inheritdoc />
        public InputMatch MatchWhitespaces(int index, out string? match)
        {
            var end = index;
            while (end < Content.Length && char.IsWhiteSpace(Content[end]))
            {
                end++;
            }

            match = FindCommonRun(Content, index, end - index) ?? Content.Substring(index, end - index);
            return end > index ? InputMatch.Success : InputMatch.Failure;
        }

        /// <summary>
        /// Returns a shared string for the usual whitespace between tokens: spaces, optionally after one line break.
        /// Whitespace is mostly skipped, so allocating a new string for every run would mostly produce garbage.
        /// </summary>
        // ponytail: spaces only, up to 64; tabs and other runs still allocate. Add tables if those show up in profiles.
        private static string? FindCommonRun(string content, int start, int length)
        {
            var end = start + length;
            var runs = Spaces;
            if (length >= 2 && content[start] == '\r' && content[start + 1] == '\n')
            {
                runs = CrLfSpaces;
                start += 2;
            }
            else if (length >= 1 && content[start] == '\n')
            {
                runs = LineFeedSpaces;
                start++;
            }

            if (end - start >= runs.Length)
            {
                return null;
            }

            for (var i = start; i < end; i++)
            {
                if (content[i] != ' ')
                {
                    return null;
                }
            }

            return runs[end - start];
        }

        private static string[] CreateRuns(string prefix)
        {
            var runs = new string[65];
            for (var i = 0; i < runs.Length; i++)
            {
                runs[i] = prefix + new string(' ', i);
            }

            return runs;
        }

        /// <inheritdoc />
        public InputMatch MatchEnd(int index)
            => index >= Content.Length ? InputMatch.Success : InputMatch.Failure;

        /// <inheritdoc />
        public string Substring(int index, int length)
            => Content.Substring(index, length);

        /// <inheritdoc />
        public bool TryGetChar(int index, out char character)
        {
            if (index >= 0 && index < Content.Length)
            {
                character = Content[index];
                return true;
            }

            character = default;
            return false;
        }
    }
}
