namespace Warpstone
{
    /// <summary>
    /// Parser interface for parsing textual input.
    /// </summary>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    public interface IParser<out TOutput> : IParser
    {
        /// <summary>
        /// Parses the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The result of running the parser.</returns>
        new IParseResult<TOutput> TryParse(string input);

        /// <summary>
        /// Parses the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The parsed result.</returns>
        /// <exception cref="ParseException">Thrown when the parser fails.</exception>
        new TOutput Parse(string input);

        /// <summary>
        /// Parses the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="position">The position.</param>
        /// <returns>The result of running the parser.</returns>
        new IParseResult<TOutput> TryParse(string input, int position);
    }

    /// <summary>
    /// Parser interface for parsing textual input.
    /// </summary>
    public interface IParser : IBoundedToString
    {
        /// <summary>
        /// Parses the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The result of running the parser.</returns>
        IParseResult TryParse(string input);

        /// <summary>
        /// Parses the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The parsed result.</returns>
        /// <exception cref="ParseException">Thrown when the parser fails.</exception>
        object? Parse(string input);

        /// <summary>
        /// Parses the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="position">The position.</param>
        /// <returns>The result of running the parser.</returns>
        IParseResult TryParse(string input, int position);
    }
}
