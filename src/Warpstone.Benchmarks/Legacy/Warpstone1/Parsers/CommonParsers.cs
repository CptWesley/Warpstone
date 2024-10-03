using static Legacy.Warpstone.Parsers.BasicParsers;

namespace Legacy.Warpstone.Parsers
{
    /// <summary>
    /// Static class providing a number of common parsers.
    /// </summary>
    public static class CommonParsers
    {
        /// <summary>
        /// A parser parsing a newline character.
        /// </summary>
        public static readonly IParser<string> Newline = Or(String("\r\n"), String("\n"));

        /// <summary>
        /// A parser parsing a whitespace.
        /// </summary>
        public static readonly IParser<string> Whitespace = Or(Newline, String("\t"), String(" "));

        /// <summary>
        /// A parser skipping all whitespaces that are optional.
        /// </summary>
        public static readonly IParser<string> OptionalWhitespaces
            = Many(Whitespace).Transform(x => string.Join(string.Empty, x));

        /// <summary>
        /// A parser skipping all whitespaces that are mandatory.
        /// </summary>
        public static readonly IParser<string> Whitespaces
            = OneOrMore(Whitespace).Transform(x => string.Join(string.Empty, x));

        /// <summary>
        /// A parser matching any lowercase letter.
        /// </summary>
        public static readonly IParser<char> LowercaseLetter
            = Regex("[a-z]").Transform(x => x[0]);

        /// <summary>
        /// A parser matching any uppercase letter.
        /// </summary>
        public static readonly IParser<char> UppercaseLetter
            = Regex("[A-Z]").Transform(x => x[0]);

        /// <summary>
        /// A parser matching any letter.
        /// </summary>
        public static readonly IParser<char> Letter
            = Regex("[a-zA-Z]").Transform(x => x[0]);

        /// <summary>
        /// A parser matching a single digit.
        /// </summary>
        public static readonly IParser<char> Digit
            = Regex("[0-9]").Transform(x => x[0]);

        /// <summary>
        /// A parser matching a single alphanumeric character.
        /// </summary>
        public static readonly IParser<char> Alphanumeric
            = Regex("[a-zA-Z0-9]").Transform(x => x[0]);

        /// <summary>
        /// Trims optional whitespaces from the left side of the parser.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The given parser.</param>
        /// <returns>A parser which trims optional whitespace from the left hand side before applying another parser.</returns>
        public static IParser<T> TrimLeft<T>(this IParser<T> parser)
            => OptionalWhitespaces.Then(parser);

        /// <summary>
        /// Trims optional whitespaces from the right side of the parser.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The given parser.</param>
        /// <returns>A parser which trims optional whitespace from the right hand side before applying another parser.</returns>
        public static IParser<T> TrimRight<T>(this IParser<T> parser)
            => parser.ThenSkip(OptionalWhitespaces);

        /// <summary>
        /// Trims optional whitespaces from the both sides of the parser.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The given parser.</param>
        /// <returns>A parser which trims optional whitespace from the left and right hand side before applying another parser.</returns>
        public static IParser<T> Trim<T>(this IParser<T> parser)
            => parser.TrimLeft().TrimRight();
    }
}
