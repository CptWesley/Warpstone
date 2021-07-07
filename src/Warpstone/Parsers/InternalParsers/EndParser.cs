using System;

namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// Parser that checks for the end of the input stream.
    /// </summary>
    /// <seealso cref="Parser{T}" />
    internal class EndParser : Parser<object>
    {
        /// <inheritdoc/>
        public override IParseResult<object> TryParse(string input, int position)
        {
            if (position == input.Length)
            {
                return new ParseResult<object>(this, default, input, position, position, Array.Empty<IParseResult<object>>());
            }

            return new ParseResult<object>(this, input, position, position, new UnexpectedTokenError(new SourcePosition(input, position, position), new string[] { string.Empty }, GetFound(input, position)), Array.Empty<IParseResult>());
        }
    }
}
