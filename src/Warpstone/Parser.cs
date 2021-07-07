using System;

namespace Warpstone
{
    /// <summary>
    /// Parser class for parsing textual input.
    /// </summary>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    public abstract class Parser<TOutput> : IParser<TOutput>
    {
        /// <inheritdoc/>
        public IParseResult<TOutput> TryParse(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return TryParse(input, 0);
        }

        /// <inheritdoc/>
        public TOutput Parse(string input)
        {
            IParseResult<TOutput> result = TryParse(input);
            if (result.Success)
            {
                return result.Value!;
            }

            throw new ParseException(result.Error!.GetMessage());
        }

        /// <summary>
        /// Parses the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="position">The position.</param>
        /// <returns>The result of running the parser.</returns>
        public abstract IParseResult<TOutput> TryParse(string input, int position);

        /// <inheritdoc/>
        IParseResult IParser.TryParse(string input)
            => TryParse(input);

        /// <inheritdoc/>
        object? IParser.Parse(string input)
            => Parse(input);

        /// <inheritdoc/>
        IParseResult IParser.TryParse(string input, int position)
            => TryParse(input, position);

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
