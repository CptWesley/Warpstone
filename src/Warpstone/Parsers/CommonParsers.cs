using static Warpstone.Parsers.BasicParsers;

namespace Warpstone.Parsers
{
    /// <summary>
    /// Static class providing a number of common parsers.
    /// </summary>
    public static class CommonParsers
    {
        /// <summary>
        /// A parser parsing a newline character.
        /// </summary>
        public static readonly Parser<string> Newline = Or(String("\r\n"), String("\n"));

        /// <summary>
        /// A parser parsing a whitespace.
        /// </summary>
        public static readonly Parser<string> Whitespace = Or(Newline, String("\t"), String(" "));

        /// <summary>
        /// A parser skipping all whitespaces that are optional.
        /// </summary>
        public static readonly Parser<string> OptionalWhitespaces
            = Many(Whitespace).Transform(x => string.Join(string.Empty, x));

        /// <summary>
        /// A parser skipping all whitespaces that are mandatory.
        /// </summary>
        public static readonly Parser<string> Whitespaces
            = OneOrMore(Whitespace).Transform(x => string.Join(string.Empty, x));

        /// <summary>
        /// A parser matching any lowercase letter.
        /// </summary>
        public static readonly Parser<char> LowercaseLetter
            = Regex("[a-z]").Transform(x => x[0]);

        /// <summary>
        /// A parser matching any uppercase letter.
        /// </summary>
        public static readonly Parser<char> UppercaseLetter
            = Regex("[A-Z]").Transform(x => x[0]);

        /// <summary>
        /// A parser matching any letter.
        /// </summary>
        public static readonly Parser<char> Letter
            = Regex("[a-zA-Z]").Transform(x => x[0]);

        /// <summary>
        /// A parser matching a single digit.
        /// </summary>
        public static readonly Parser<char> Digit
            = Regex("[0-9]").Transform(x => x[0]);

        /// <summary>
        /// A parser matching a single alphanumeric character.
        /// </summary>
        public static readonly Parser<char> Alphanumeric
            = Regex("[a-zA-Z0-9]").Transform(x => x[0]);

        /// <summary>
        /// Trims optional whitespaces from the left side of the parser.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The given parser.</param>
        /// <returns>A parser which trims optional whitespace from the left hand side before applying another parser.</returns>
        public static Parser<T> TrimLeft<T>(this Parser<T> parser)
            => OptionalWhitespaces.Then(parser);

        /// <summary>
        /// Trims optional whitespaces from the right side of the parser.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The given parser.</param>
        /// <returns>A parser which trims optional whitespace from the right hand side before applying another parser.</returns>
        public static Parser<T> TrimRight<T>(this Parser<T> parser)
            => parser.ThenSkip(OptionalWhitespaces);

        /// <summary>
        /// Trims optional whitespaces from the both sides of the parser.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <param name="parser">The given parser.</param>
        /// <returns>A parser which trims optional whitespace from the left and right hand side before applying another parser.</returns>
        public static Parser<T> Trim<T>(this Parser<T> parser)
            => parser.TrimLeft().TrimRight();
    }
}
