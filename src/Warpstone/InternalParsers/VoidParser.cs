namespace Warpstone.InternalParsers
{
    /// <summary>
    /// Parser that doesn't take any arguments and always succeeds.
    /// </summary>
    /// <typeparam name="T">The result type of the parser.</typeparam>
    /// <seealso cref="Warpstone.Parser{T}" />
    public class VoidParser<T> : Parser<T>
    {
        /// <inheritdoc/>
        internal override ParseResult<T> Parse(string input, int position)
            => new ParseResult<T>(default, position);
    }
}
