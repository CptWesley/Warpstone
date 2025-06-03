using System;
using System.Collections.Generic;

namespace Legacy.Warpstone1
{
    /// <summary>
    /// Parser class for parsing textual input.
    /// </summary>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    public abstract class Parser<TOutput> : IParser<TOutput>
    {
        /// <summary>
        /// An empty set of results.
        /// </summary>
        protected static readonly IEnumerable<IParseResult> EmptyResults = Array.Empty<IParseResult>();

        /// <inheritdoc/>
        public IParseResult<TOutput> TryParse(string input)
            => TryParse(input, false);

        /// <inheritdoc/>
        public TOutput Parse(string input)
            => Parse(input, false);

        /// <inheritdoc/>
        public IParseResult<TOutput> TryParse(string input, bool collectTrace)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return TryParse(input, 0, collectTrace);
        }

        /// <inheritdoc/>
        public TOutput Parse(string input, bool collectTrace)
        {
            IParseResult<TOutput> result = TryParse(input, collectTrace);
            if (result.Success)
            {
                return result.Value!;
            }

            throw new ParseException(result.Error!.GetMessage());
        }

        /// <inheritdoc/>
        public abstract IParseResult<TOutput> TryParse(string input, int position, bool collectTrace);

        /// <inheritdoc/>
        public abstract string ToString(int depth);

        /// <inheritdoc/>
        public override string ToString()
            => ToString(4);

        /// <inheritdoc/>
        IParseResult IParser.TryParse(string input)
            => TryParse(input);

        /// <inheritdoc/>
        object? IParser.Parse(string input)
            => Parse(input);

        /// <inheritdoc/>
        IParseResult IParser.TryParse(string input, bool collectTrace)
            => TryParse(input, collectTrace);

        /// <inheritdoc/>
        object? IParser.Parse(string input, bool collectTrace)
            => Parse(input, collectTrace);

        /// <inheritdoc/>
        IParseResult IParser.TryParse(string input, int position, bool collectTrace)
            => TryParse(input, position, collectTrace);

        /// <summary>
        /// Gets the found characters.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="position">The position.</param>
        /// <returns>The found characters.</returns>
        protected string GetFound(string input, int position)
            => position < input?.Length ? $"'{input[position]}'" : "EOF";
    }
}
