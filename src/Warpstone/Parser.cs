using System;
using System.Linq;

namespace Warpstone
{
    /// <summary>
    /// Parser class for parsing textual input.
    /// </summary>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    public abstract class Parser<TOutput>
    {
        /// <summary>
        /// Parses the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The result of running the parser.</returns>
        public ParseResult<TOutput> TryParse(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return TryParse(input, 0);
        }

        /// <summary>
        /// Parses the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The parsed result.</returns>
        /// <exception cref="ParseException">Thrown when the parser fails.</exception>
        public TOutput Parse(string input)
        {
            ParseResult<TOutput> result = TryParse(input);
            if (result.Success)
            {
                return result.Value;
            }

            string expectedString = $"Expected ";
            if (result.Expected.Any())
            {
                expectedString += "one of ";
            }

            expectedString += string.Join(", ", result.Expected.Select(x => string.IsNullOrEmpty(x) ? "EOF" : $"'{x}'"));
            expectedString += $" but found '{input[result.Position]}'.";

            throw new ParseException(expectedString);
        }

        /// <summary>
        /// Parses the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="position">The position.</param>
        /// <returns>The result of running the parser.</returns>
        internal abstract ParseResult<TOutput> TryParse(string input, int position);
    }
}
