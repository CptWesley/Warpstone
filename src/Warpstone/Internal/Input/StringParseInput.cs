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
        private readonly int[] lineLengths;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringParseInput"/> class.
        /// </summary>
        /// <param name="source">The source of the input.</param>
        /// <param name="content">The input string.</param>
        /// <param name="lineLengths">The start index of each line.</param>
        public StringParseInput(IParseInputSource source, string content, int[] lineLengths)
        {
            Source = source;
            Content = content;
            this.lineLengths = lineLengths;
        }

        /// <inheritdoc />
        public string Content { get; }

        /// <inheritdoc />
        public IParseInputSource Source { get; }

        /// <inheritdoc />
        public ParseInputPosition GetPosition(int index)
        {
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

        /// <inheritdoc />
        public InputMatch Match(int index, char character)
            => index < Content.Length && Content[index] == character
            ? InputMatch.Success
            : InputMatch.Failure;

        /// <inheritdoc />
        public InputMatch Match(int index, string str, CultureInfo culture, CompareOptions options, out string? match)
        {
            if (index + str.Length > Content.Length
                || string.Compare(Content, index, str, 0, str.Length, culture, options) != 0)
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

        /// <inheritdoc />
        public InputMatch MatchWhitespaces(int index, out string? match)
        {
            var end = index;
            while (end < Content.Length && char.IsWhiteSpace(Content[end]))
            {
                end++;
            }

            match = Content.Substring(index, end - index);
            return end > index ? InputMatch.Success : InputMatch.Failure;
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
