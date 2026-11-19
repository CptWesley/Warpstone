using System.Globalization;
using System.Text.RegularExpressions;

namespace Warpstone
{
    /// <summary>
    /// Interface for inputs used by parsers.
    /// Parsers read the input through the match methods, so an input can read its content lazily.
    /// </summary>
    public interface IParseInput
    {
        /// <summary>
        /// The whole string content of the input.
        /// Reads the rest of the input first when it is read lazily, which blocks until it has been read.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// The source of the input.
        /// </summary>
        public IParseInputSource Source { get; }

        /// <summary>
        /// Gets line and column position of the given <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The found line number (starting at line number 1) and column for the given index.</returns>
        public ParseInputPosition GetPosition(int index);

        /// <summary>
        /// Matches the given <paramref name="character"/> at the given <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index to match at.</param>
        /// <param name="character">The character to match.</param>
        /// <returns>The outcome of the match.</returns>
        public InputMatch Match(int index, char character);

        /// <summary>
        /// Matches the given <paramref name="str"/> at the given <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index to match at.</param>
        /// <param name="str">The string to match.</param>
        /// <param name="culture">The culture used for comparing.</param>
        /// <param name="options">The options used for comparing.</param>
        /// <param name="match">The matched text when the match succeeds, otherwise <c>null</c>.</param>
        /// <returns>The outcome of the match.</returns>
        public InputMatch Match(int index, string str, CultureInfo culture, CompareOptions options, out string? match);

        /// <summary>
        /// Matches the given <paramref name="regex"/> at the given <paramref name="index"/>.
        /// The regex has to be anchored at the index with <c>\G</c>.
        /// </summary>
        /// <param name="index">The index to match at.</param>
        /// <param name="regex">The regex to match.</param>
        /// <param name="match">The matched text when the match succeeds, otherwise <c>null</c>.</param>
        /// <returns>The outcome of the match.</returns>
        public InputMatch Match(int index, Regex regex, out string? match);

        /// <summary>
        /// Matches as many whitespace characters as possible at the given <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index to match at.</param>
        /// <param name="match">The matched whitespace, empty when there is none, <c>null</c> when pending.</param>
        /// <returns><see cref="InputMatch.Success"/> when at least one whitespace character matched.</returns>
        public InputMatch MatchWhitespaces(int index, out string? match);

        /// <summary>
        /// Matches the end of the input at the given <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index to match at.</param>
        /// <returns>The outcome of the match.</returns>
        public InputMatch MatchEnd(int index);

        /// <summary>
        /// Gets the substring at the given <paramref name="index"/> with the given <paramref name="length"/>.
        /// Reads the input up to the end of the substring, or until the input ends, which blocks until it has been read.
        /// </summary>
        /// <param name="index">The start index.</param>
        /// <param name="length">The length.</param>
        /// <returns>The substring.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The input ends before the substring does.</exception>
        public string Substring(int index, int length);

        /// <summary>
        /// Gets the character at the given <paramref name="index"/>.
        /// Reads the input up to and including the index, or until the input ends, which blocks until it has been read.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="character">The found character.</param>
        /// <returns><c>true</c> when the input has a character at the index.</returns>
        public bool TryGetChar(int index, out char character);
    }
}
