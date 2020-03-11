namespace Warpstone.InternalParsers
{
    /// <summary>
    /// Parser that doesn't take any arguments and always succeeds.
    /// </summary>
    /// <typeparam name="T">The result type of the parser.</typeparam>
    /// <seealso cref="Warpstone.Parser{T}" />
    internal class VoidParser<T> : Parser<T>
    {
        /// <inheritdoc/>
        internal override ParseResult<T> TryParse(string input, int position)
            => new ParseResult<T>(default, position, position);
    }
}
