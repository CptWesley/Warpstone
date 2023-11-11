using Xunit;
using static AssertNet.Assertions;

namespace Warpstone.Tests.Parsers
{
    /// <summary>
    /// Test class for the <see cref="Warpstone.Parsers.CommonParsers"/> class.
    /// </summary>
    public static class CommonParsersTests
    {
        /// <summary>
        /// Checks that parsing alphanumericals works correctly.
        /// </summary>
        [Fact]
        public static void AlphanumericParserCorrectAlphabetical()
            => AssertThat(Alphanumeric.Parse("xyz")).IsEqualTo('x');

        /// <summary>
        /// Checks that parsing alphanumericals works correctly.
        /// </summary>
        [Fact]
        public static void AlphanumericParserCorrectNumerical()
            => AssertThat(Alphanumeric.Parse("987")).IsEqualTo('9');

        /// <summary>
        /// Checks that parsing alphanumericals works correctly.
        /// </summary>
        [Fact]
        public static void AlphanumericParserIncorrect()
            => AssertThat(() => Alphanumeric.Parse("-yz")).ThrowsExactlyException<ParseException>();

        /// <summary>
        /// Checks that parsing letters works correctly.
        /// </summary>
        [Fact]
        public static void LetterParserCorrectAlphabetical()
            => AssertThat(Letter.Parse("xyz")).IsEqualTo('x');

        /// <summary>
        /// Checks that parsing alphanumericals works correctly.
        /// </summary>
        [Fact]
        public static void LetterParserIncorrect()
            => AssertThat(() => Letter.Parse("-yz")).ThrowsExactlyException<ParseException>();

        /// <summary>
        /// Checks that parsing letters works correctly.
        /// </summary>
        [Fact]
        public static void UppercaseLetterParserCorrectAlphabetical()
            => AssertThat(UppercaseLetter.Parse("Xyz")).IsEqualTo('X');

        /// <summary>
        /// Checks that parsing alphanumericals works correctly.
        /// </summary>
        [Fact]
        public static void UppercaseLetterParserIncorrect()
            => AssertThat(() => UppercaseLetter.Parse("xyz")).ThrowsExactlyException<ParseException>();

        /// <summary>
        /// Checks that parsing letters works correctly.
        /// </summary>
        [Fact]
        public static void LowercaseLetterParserCorrectAlphabetical()
            => AssertThat(LowercaseLetter.Parse("xyz")).IsEqualTo('x');

        /// <summary>
        /// Checks that parsing alphanumericals works correctly.
        /// </summary>
        [Fact]
        public static void LowercaseLetterParserIncorrect()
            => AssertThat(() => LowercaseLetter.Parse("XYZ")).ThrowsExactlyException<ParseException>();

        /// <summary>
        /// Checks that parsing digits works correctly.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="result">The expected result.</param>
        [Theory]
        [InlineData("0x", '0')]
        [InlineData("1x", '1')]
        [InlineData("2x", '2')]
        [InlineData("3x", '3')]
        [InlineData("4x", '4')]
        [InlineData("5x", '5')]
        [InlineData("6x", '6')]
        [InlineData("7x", '7')]
        [InlineData("8x", '8')]
        [InlineData("9x", '9')]
        public static void DigitParserCorrect(string input, int result)
            => AssertThat(Digit.Parse(input)).IsEqualTo(result);

        /// <summary>
        /// Checks that parsing digits works correctly.
        /// </summary>
        [Fact]
        public static void DigitParserIncorrect()
            => AssertThat(() => Digit.Parse("xyz")).ThrowsExactlyException<ParseException>();

        /// <summary>
        /// Checks that parsing newlines works correctly.
        /// </summary>
        /// <param name="input">The input string.</param>
        [Theory]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public static void NewlineCorrect(string input)
            => AssertThat(Newline.Parse(input)).IsEqualTo(input);

        /// <summary>
        /// Checks that parsing newlines works correctly.
        /// </summary>
        [Fact]
        public static void NewlineParserIncorrect()
            => AssertThat(() => Newline.Parse("xyz")).ThrowsExactlyException<ParseException>();

        /// <summary>
        /// Checks that parsing whitespaces works correctly.
        /// </summary>
        /// <param name="input">The input string.</param>
        [Theory]
        [InlineData("\n")]
        [InlineData("\r\n")]
        [InlineData(" ")]
        [InlineData("\t")]
        public static void WhitespaceCorrect(string input)
            => AssertThat(Whitespace.Parse(input)).IsEqualTo(input);

        /// <summary>
        /// Checks that parsing whitespaces works correctly.
        /// </summary>
        [Fact]
        public static void WhitespaceParserIncorrect()
            => AssertThat(() => Whitespace.Parse("xyz")).ThrowsExactlyException<ParseException>();

        /// <summary>
        /// Checks that parsing whitespaces works correctly.
        /// </summary>
        /// <param name="input">The input string.</param>
        [Theory]
        [InlineData("\n")]
        [InlineData("\r\n")]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\r\n\n\t ")]
        public static void WhitespacesCorrect(string input)
            => AssertThat(Whitespaces.Parse(input)).IsEqualTo(input);

        /// <summary>
        /// Checks that parsing whitespaces works correctly.
        /// </summary>
        [Fact]
        public static void WhitespacesParserIncorrect()
            => AssertThat(() => Whitespaces.Parse(string.Empty)).ThrowsExactlyException<ParseException>();

        /// <summary>
        /// Checks that parsing whitespaces works correctly.
        /// </summary>
        /// <param name="input">The input string.</param>
        [Theory]
        [InlineData("")]
        [InlineData("\n")]
        [InlineData("\r\n")]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\r\n\n\t ")]
        public static void OptionalWhitespacesCorrect(string input)
            => AssertThat(OptionalWhitespaces.Parse(input)).IsEqualTo(input);

        /// <summary>
        /// Checks that parser trimming works correctly.
        /// </summary>
        [Fact]
        public static void TrimParserCorrect()
            => AssertThat(Alphanumeric.Trim().Parse("\tx  yz")).IsEqualTo('x');
    }
}
