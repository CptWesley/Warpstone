namespace Warpstone
{
    /// <summary>
    /// Parser interface for parsing textual input.
    /// </summary>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    public interface IParser<out TOutput>
    {
        /// <summary>
        /// Parses the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The result of running the parser.</returns>
        IParseResult<TOutput> TryParse(string input);

        /// <summary>
        /// Parses the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The parsed result.</returns>
        /// <exception cref="ParseException">Thrown when the parser fails.</exception>
        TOutput Parse(string input);
    }
}
