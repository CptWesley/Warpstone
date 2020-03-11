namespace Warpstone.InternalParsers
{
    /// <summary>
    /// Parser that checks for the end of the input stream.
    /// </summary>
    /// <seealso cref="Warpstone.Parser{T}" />
    public class EndParser : Parser<object>
    {
        /// <inheritdoc/>
        internal override ParseResult<object> Parse(string input, int position)
        {
            if (position == input.Length)
            {
                return new ParseResult<object>(default, position, position);
            }

            return new ParseResult<object>();
        }
    }
}
