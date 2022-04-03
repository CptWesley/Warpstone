using System.Collections.Generic;
using System.Linq;

namespace Warpstone
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
        /// <param name="allowBacktracking">Indicates whether or not we can backtrack from this error.</param>
        /// <param name="expected">The expected characters.</param>
        /// <param name="found">The found character.</param>
        public UnexpectedTokenError(SourcePosition position, bool allowBacktracking, IEnumerable<string> expected, string found)
            : base(position, allowBacktracking)
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
        public override IParseError DisallowBacktracking()
            => new UnexpectedTokenError(Position, false, Expected, Found);

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
