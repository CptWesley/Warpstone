using System;
using System.Globalization;
using System.Numerics;
using static Warpstone.Parsers.BasicParsers;

namespace Warpstone.Parsers
{
    /// <summary>
    /// Contains logic for creating parsers dealing with numbers.
    /// </summary>
    public static class NumberParsers
    {
        /// <summary>
        /// Creates a parser parsing an integer with the given number of bits.
        /// </summary>
        /// <param name="bits">The number of bits.</param>
        /// <param name="signed">Determines whether or not the number is signed or unsigned.</param>
        /// <returns>A parser parsing the given signed or unsigned integer.</returns>
        public static Parser<string> Integer(int bits, bool signed)
        {
            if (signed)
            {
                bits--;
            }

            BigInteger max = BigInteger.Pow(2, bits) - 1;
            string pattern = BuildPattern(max.ToString(CultureInfo.InvariantCulture), true);

            if (signed)
            {
                pattern = $"-{max + 1}|-?{pattern}";
            }

            return Regex($@"({pattern})(?!\d)");
        }

        /// <summary>
        /// Creates a parser which parses any given integer number between zero and the given maximum value.
        /// </summary>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>A parser which parses an integer.</returns>
        /// <exception cref="ArgumentException">Must be greater than 0. - maxValue.</exception>
        public static Parser<string> Integer(long maxValue)
            => Integer(new BigInteger(maxValue));

        /// <summary>
        /// Creates a parser which parses any given integer number between zero and the given maximum value.
        /// </summary>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>A parser which parses an integer.</returns>
        /// <exception cref="ArgumentException">Must be greater than 0. - maxValue.</exception>
        public static Parser<string> Integer(int maxValue)
            => Integer(new BigInteger(maxValue));

        /// <summary>
        /// Creates a parser which parses any given integer number between zero and the given maximum value.
        /// </summary>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>A parser which parses an integer.</returns>
        /// <exception cref="ArgumentException">Must be greater than 0. - maxValue.</exception>
        public static Parser<string> Integer(BigInteger maxValue)
        {
            if (maxValue < 0)
            {
                throw new ArgumentException("Must be greater than 0.", nameof(maxValue));
            }

            return Regex($@"({BuildPattern(maxValue.ToString(CultureInfo.InvariantCulture), true)})(?!\d)");
        }

        /// <summary>
        /// Creates a parser which parses any given floating point value within the given maximum value either positive or negative.
        /// </summary>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>A parser which parses a floating point value.</returns>
        public static Parser<string> Float(float maxValue)
            => Float(new BigInteger(maxValue));

        /// <summary>
        /// Creates a parser which parses any given floating point value within the given maximum value either positive or negative.
        /// </summary>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>A parser which parses a floating point value.</returns>
        public static Parser<string> Float(double maxValue)
            => Float(new BigInteger(maxValue));

        /// <summary>
        /// Creates a parser which parses any given floating point value within the given maximum value either positive or negative.
        /// </summary>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>A parser which parses a floating point value.</returns>
        public static Parser<string> Float(BigInteger maxValue)
        {
            if (maxValue < 0)
            {
                maxValue = -maxValue;
            }

            string pattern = BuildPattern(maxValue.ToString(CultureInfo.InvariantCulture), true);

            return Regex($@"(((-?{pattern})(\.[0-9]+)?)|(\.[0-9]+))(?!\d)");
        }

        private static string BuildPattern(string number, bool isTop)
        {
            int first = int.Parse(number[0].ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

            if (number.Length == 1)
            {
                return Range(0, first);
            }

            int lesser = first - 1;
            string next = number.Substring(1);
            int length = next.Length;

            if (isTop)
            {
                return $@"({first}{BuildPattern(next, false)}{RangeCase(1, lesser, length)}|[1-9]\d{{0,{length - 1}}}|0)";
            }

            return $@"({first}{BuildPattern(next, false)}{RangeCase(0, lesser, length)})";
        }

        private static string RangeCase(int min, int max, int length)
        {
            if (max >= min)
            {
                return $"|{Range(min, max)}{Digits(length)}";
            }

            return string.Empty;
        }

        private static string Digits(int length)
            => length == 1 ? @"\d" : $@"\d{{0,{length}}}";

        private static string Range(int min, int max)
            => max == min ? $"{min}" : $"[{min}-{max}]";
    }
}
