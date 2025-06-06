using System.Collections.Generic;
using System.Linq;

namespace Legacy.Warpstone1
{
    /// <summary>
    /// Represents an error that occured during parsing.
    /// </summary>
    public class UnexpectedTokenError : ParseError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedTokenError"/> class.
        /// </summary>
        /// <param name="position">The start position.</param>
        /// <param name="expected">The expected characters.</param>
        /// <param name="found">The found character.</param>
        public UnexpectedTokenError(SourcePosition position, IEnumerable<string> expected, string found)
            : base(position)
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
        protected override string GetSimpleMessage()
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
