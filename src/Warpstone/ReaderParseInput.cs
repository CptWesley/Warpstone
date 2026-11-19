using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Warpstone
{
    /// <summary>
    /// An <see cref="IParseInput"/> that reads its content from a <see cref="TextReader"/> as parsers need it.
    /// Everything read stays buffered, so parsers can backtrack into it.
    /// </summary>
    internal sealed class ReaderParseInput : IParseInput
    {
        private const int MinimumReadSize = 4096;

        private readonly TextReader reader;
        private readonly List<int> lineStarts = [0];
        private char[] buffer = new char[MinimumReadSize];
        private int length;
        private string? content;
        private int pendingEnd;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderParseInput"/> class.
        /// </summary>
        /// <param name="source">The source of the input.</param>
        /// <param name="reader">The reader to read the input from.</param>
        public ReaderParseInput(IParseInputSource source, TextReader reader)
        {
            Source = source;
            this.reader = reader;
        }

        // ponytail: keeps both the buffer and this string once built (2x memory); drop the buffer if big reader inputs need Content.

        /// <inheritdoc />
        public string Content
        {
            get
            {
                if (content is null)
                {
                    Read(int.MaxValue);
                    content = new string(buffer, 0, length);
                }

                return content;
            }
        }

        /// <inheritdoc />
        public IParseInputSource Source { get; }

        /// <summary>
        /// Whether a parse runs asynchronously.
        /// Matches that need more input then return <see cref="InputMatch.Pending"/>
        /// and leave reading it to <see cref="ReadPendingAsync"/>, instead of blocking.
        /// </summary>
        public bool IsAsynchronous { get; set; }

        /// <summary>
        /// Whether a match waits for <see cref="ReadPendingAsync"/>.
        /// </summary>
        public bool IsPending => pendingEnd > 0;

        private bool IsComplete { get; set; }

        /// <inheritdoc />
        public ParseInputPosition GetPosition(int index)
        {
            var low = 0;
            var high = lineStarts.Count - 1;

            while (low < high)
            {
                var mid = (low + high + 1) / 2;
                if (lineStarts[mid] <= index)
                {
                    low = mid;
                }
                else
                {
                    high = mid - 1;
                }
            }

            return new ParseInputPosition(this, index, low + 1, index - lineStarts[low] + 1);
        }

        /// <inheritdoc />
        public InputMatch Match(int index, char character)
        {
            var available = Request(index + 1);
            if (available != InputMatch.Success)
            {
                return available;
            }

            return buffer[index] == character ? InputMatch.Success : InputMatch.Failure;
        }

        /// <inheritdoc />
        public InputMatch Match(int index, string str, CultureInfo culture, CompareOptions options, out string? match)
        {
            match = null;
            var available = Request(index + str.Length);
            if (available != InputMatch.Success)
            {
                return available;
            }

            if (options == CompareOptions.Ordinal)
            {
                for (var i = 0; i < str.Length; i++)
                {
                    if (buffer[index + i] != str[i])
                    {
                        return InputMatch.Failure;
                    }
                }

                match = str;
                return InputMatch.Success;
            }

            var text = new string(buffer, index, str.Length);
            if (string.Compare(text, str, culture, options) != 0)
            {
                return InputMatch.Failure;
            }

            match = text;
            return InputMatch.Success;
        }

        /// <inheritdoc />
        public InputMatch Match(int index, Regex regex, out string? match)
        {
            // A regex can look arbitrarily far ahead, so it needs the whole input.
            match = null;
            if (!IsComplete)
            {
                var available = Request(int.MaxValue);
                if (available == InputMatch.Pending)
                {
                    return available;
                }
            }

#if NET7_0_OR_GREATER
            var matches = regex.EnumerateMatches(new ReadOnlySpan<char>(buffer, 0, length), index);
            if (matches.MoveNext())
            {
                match = new string(buffer, matches.Current.Index, matches.Current.Length);
                return InputMatch.Success;
            }

            return InputMatch.Failure;
#else
            var result = regex.Match(Content, index);
            if (!result.Success)
            {
                return InputMatch.Failure;
            }

            match = result.Value;
            return InputMatch.Success;
#endif
        }

        /// <inheritdoc />
        public InputMatch MatchWhitespaces(int index, out string? match)
        {
            match = null;
            var end = index;
            while (true)
            {
                while (end < length && char.IsWhiteSpace(buffer[end]))
                {
                    end++;
                }

                if (end < length)
                {
                    break;
                }

                var available = Request(end + 1);
                if (available == InputMatch.Pending)
                {
                    return available;
                }

                if (available == InputMatch.Failure)
                {
                    break;
                }
            }

            match = end > index ? new string(buffer, index, end - index) : string.Empty;
            return end > index ? InputMatch.Success : InputMatch.Failure;
        }

        /// <inheritdoc />
        public InputMatch MatchEnd(int index)
            => Request(index + 1) switch
            {
                InputMatch.Success => InputMatch.Failure,
                InputMatch.Failure => InputMatch.Success,
                _ => InputMatch.Pending,
            };

        /// <inheritdoc />
        public string Substring(int index, int length)
        {
            Read(index + length);
            if (index < 0 || length < 0 || index + length > this.length)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "The input ends before the substring does.");
            }

            return new string(buffer, index, length);
        }

        /// <inheritdoc />
        public bool TryGetChar(int index, out char character)
        {
            Read(index + 1);
            if (index >= 0 && index < length)
            {
                character = buffer[index];
                return true;
            }

            character = default;
            return false;
        }

        /// <summary>
        /// Reads the input that was requested while <see cref="IsAsynchronous"/> was set.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that completes when the input is read.</returns>
        public async Task ReadPendingAsync(CancellationToken cancellationToken)
        {
            // ponytail: TextReader.ReadAsync(char[]) has no cancellation token before .NET Core 3; cancellation is checked between reads.
            while (length < pendingEnd && !IsComplete)
            {
                cancellationToken.ThrowIfCancellationRequested();
                EnsureCapacity();
                Append(await reader.ReadAsync(buffer, length, buffer.Length - length).ConfigureAwait(false));
            }

            pendingEnd = 0;
        }

        // ponytail: Pending is only expected from iterative parsers; IsAsynchronous is only set by IterativeParseContext.RunToEndAsync.

        /// <summary>
        /// Makes the input up to <paramref name="end"/> available.
        /// Reads it synchronously, or leaves it pending when <see cref="IsAsynchronous"/> is set.
        /// </summary>
        /// <param name="end">The length of input needed, <see cref="int.MaxValue"/> for the whole input.</param>
        /// <returns>
        /// <see cref="InputMatch.Success"/> when the input is available,
        /// <see cref="InputMatch.Failure"/> when the input ends before <paramref name="end"/>,
        /// <see cref="InputMatch.Pending"/> when it is left to <see cref="ReadPendingAsync"/>.
        /// </returns>
        private InputMatch Request(int end)
        {
            if (end <= length)
            {
                return InputMatch.Success;
            }

            if (IsComplete)
            {
                return InputMatch.Failure;
            }

            if (IsAsynchronous)
            {
                pendingEnd = end;
                return InputMatch.Pending;
            }

            Read(end);
            return end <= length ? InputMatch.Success : InputMatch.Failure;
        }

        /// <summary>
        /// Reads synchronously until the input up to <paramref name="end"/> is buffered or the input ends.
        /// Each read takes only what the reader has ready, so it never waits for more than <paramref name="end"/> needs.
        /// </summary>
        private void Read(int end)
        {
            while (length < end && !IsComplete)
            {
                EnsureCapacity();
                Append(reader.Read(buffer, length, buffer.Length - length));
            }
        }

        private void EnsureCapacity()
        {
            if (buffer.Length - length < MinimumReadSize)
            {
                Array.Resize(ref buffer, Math.Max(buffer.Length * 2, length + MinimumReadSize));
            }
        }

        private void Append(int read)
        {
            if (read == 0)
            {
                IsComplete = true;
                return;
            }

            for (var i = length; i < length + read; i++)
            {
                if (buffer[i] == '\n')
                {
                    lineStarts.Add(i + 1);
                }
            }

            length += read;
        }
    }
}
