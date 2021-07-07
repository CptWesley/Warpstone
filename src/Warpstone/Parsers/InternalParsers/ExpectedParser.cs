using System.Collections.Generic;

namespace Warpstone.Parsers.InternalParsers
{
    /// <summary>
    /// A parser which replaced the expected string with a given string.
    /// </summary>
    /// <typeparam name="T">The result type of the parser.</typeparam>
    /// <seealso cref="Parser{T}" />
    internal class ExpectedParser<T> : Parser<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectedParser{T}"/> class.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="names">The names.</param>
        internal ExpectedParser(IParser<T> parser, IEnumerable<string> names)
        {
            Parser = parser;
            Names = names;
        }

        /// <summary>
        /// Gets the parser.
        /// </summary>
        internal IParser<T> Parser { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        internal IEnumerable<string> Names { get; }

        /// <inheritdoc/>
        public override IParseResult<T> TryParse(string input, int position)
        {
            IParseResult<T> result = Parser.TryParse(input, position);
            if (result.Success)
            {
                return new ParseResult<T>(this, result.Value, input, result.Position.Start, result.Position.End, new[] { result });
            }

            return new ParseResult<T>(this, input, result.Position.Start, result.Position.End, new UnexpectedTokenError(result.Error!.Position, Names, GetFound(input, position)), new[] { result });
        }
    }
}
