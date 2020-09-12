using Warpstone.Parsers;
using Xunit;
using static AssertNet.Assertions;
using static Warpstone.Parsers.NumberParsers;

namespace Warpstone.Tests.Parsers
{
    /// <summary>
    /// Test class for the <see cref="Warpstone.Parsers.NumberParsers"/> class.
    /// </summary>
    public static class NumberParsersTests
    {
        /// <summary>
        /// Checks that we can parse bytes correctly.
        /// </summary>
        [Fact]
        public static void ByteTest()
        {
            Parser<string> parser = Integer(8, false).ThenEnd();
            AssertThat(parser.Parse("0")).IsEqualTo("0");
            AssertThat(parser.Parse("128")).IsEqualTo("128");
            AssertThat(parser.Parse("255")).IsEqualTo("255");
            AssertThat(() => parser.Parse("256")).ThrowsExactlyException<ParseException>();
            AssertThat(() => parser.Parse("-1")).ThrowsExactlyException<ParseException>();
        }

        /// <summary>
        /// Checks that we can parse bytes correctly.
        /// </summary>
        /// <param name="x">The input of the test.</param>
        [Theory]
        [InlineData("0")]
        [InlineData("128")]
        [InlineData("255")]
        [InlineData("2147483647")]
        [InlineData("-2147483648")]
        [InlineData("914748364")]
        [InlineData("10")]
        [InlineData("20")]
        [InlineData("202")]
        public static void IntSuccessTest(string x)
        {
            Parser<string> parser = Integer(32, true).ThenEnd();
            AssertThat(parser.Parse(x)).IsEqualTo(x);
        }

        /// <summary>
        /// Checks that we can parse bytes correctly.
        /// </summary>
        /// <param name="x">The input of the test.</param>
        [Theory]
        [InlineData("04")]
        [InlineData("2147483648")]
        [InlineData("-2147483649")]
        public static void IntFailTest(string x)
        {
            Parser<string> parser = Integer(32, true).ThenEnd();
            AssertThat(() => parser.Parse(x)).ThrowsExactlyException<ParseException>();
        }

        /// <summary>
        /// Checks that we can parse bytes correctly.
        /// </summary>
        [Fact]
        public static void Max1023Test()
        {
            Parser<string> parser = Integer(10, false).ThenEnd();
            AssertThat(parser.Parse("0")).IsEqualTo("0");
            AssertThat(parser.Parse("128")).IsEqualTo("128");
            AssertThat(parser.Parse("255")).IsEqualTo("255");
            AssertThat(parser.Parse("10")).IsEqualTo("10");
            AssertThat(parser.Parse("20")).IsEqualTo("20");
            AssertThat(parser.Parse("202")).IsEqualTo("202");
            AssertThat(parser.Parse("1022")).IsEqualTo("1022");
            AssertThat(parser.Parse("1023")).IsEqualTo("1023");
            AssertThat(parser.Parse("999")).IsEqualTo("999");
            AssertThat(() => parser.Parse("04")).ThrowsExactlyException<ParseException>();
            AssertThat(() => parser.Parse("-1")).ThrowsExactlyException<ParseException>();
            AssertThat(() => parser.Parse("1024")).ThrowsExactlyException<ParseException>();
        }
    }
}
