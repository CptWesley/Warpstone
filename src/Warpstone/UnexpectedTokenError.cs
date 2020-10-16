using System.Collections.Generic;
using System.Linq;

namespace Warpstone
{
    /// <summary>
    /// Represents an error that occured during parsing.
    /// </summary>
    public class UnexpectedTokenError : IParseError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedTokenError"/> class.
        /// </summary>
        /// <param name="expected">The expected characters.</param>
        /// <param name="found">The found character.</param>
        public UnexpectedTokenError(IEnumerable<string> expected, string found)
        {
            Expected = expected;
            Found = found;
        }

        /// <summary>
        /// Gets the expected characters.
        /// </summary>
        public IEnumerable<string> Expected { get; }

        /// <summary>
        /// Gets the found character.
        /// </summary>
        public string Found { get; }

        /// <inheritdoc/>
        public string GetMessage()
        {
            string expectedString = $"Expected ";
            if (Expected.Count() > 1)
            {
                expectedString += "one of ";
            }

            expectedString += string.Join(", ", Expected.Select(x => string.IsNullOrEmpty(x) ? "EOF" : x).Distinct());
            expectedString += $" but found {Found}.";
            return expectedString;
        }
    }
}
