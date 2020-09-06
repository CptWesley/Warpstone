using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Xunit;
using static AssertNet.Assertions;
using static Warpstone.Parsers.BasicParsers;
using static Warpstone.Parsers.CommonParsers;
using static Warpstone.Parsers.ExpressionParsers;

namespace Warpstone.Tests.Parsers
{
    /// <summary>
    /// Test class for the <see cref="Warpstone.Parsers.ExpressionParsers"/> class.
    /// </summary>
    public static class ExpressionParsersTests
    {
        private static readonly Parser<Expression> Num
            = Regex("[0-9]+").Transform(x => new NumExpression(int.Parse(x, CultureInfo.InvariantCulture)) as Expression).Trim();

        [SuppressMessage("Readability Rules", "SA1118", Justification = "Nicer to look at.")]
        [SuppressMessage("Readability Rules", "SA1009", Justification = "Nicer to look at.")]
        [SuppressMessage("Readability Rules", "SA1111", Justification = "Nicer to look at.")]
        private static readonly Parser<Expression> Exp
            = BuildExpression(Num, new[]
            {
                Post<string, Expression>(Operator("++"), (op, e) => new IncrExpression(e)),
                RightToLeft<string, Expression>(
                    (Operator("^"), (l, r) => new PowExpression(l, r))
                ),
                LeftToRight<string, Expression>(Operator("*"), (l, r) => new MulExpression(l, r)),
                LeftToRight<string, Expression>(
                    (Operator("+"), (l, r) => new AddExpression(l, r)),
                    (Operator("-"), (l, r) => new SubExpression(l, r))
                ),
            });

        /// <summary>
        /// Checks that num parsing works correctly.
        /// </summary>
        [Fact]
        public static void NumParsing()
            => AssertThat(Parse("42")).IsEquivalentTo(new NumExpression(42));

        /// <summary>
        /// Checks that add parsing works correctly.
        /// </summary>
        [Fact]
        public static void AddParsing()
            => AssertThat(Parse("40 + 2")).IsEquivalentTo(new AddExpression(new NumExpression(40), new NumExpression(2)));

        /// <summary>
        /// Checks that sub parsing works correctly.
        /// </summary>
        [Fact]
        public static void SubParsing()
            => AssertThat(Parse("40 - 2")).IsEquivalentTo(new SubExpression(new NumExpression(40), new NumExpression(2)));

        /// <summary>
        /// Checks that mul parsing works correctly.
        /// </summary>
        [Fact]
        public static void MulParsing()
            => AssertThat(Parse("40 * 2")).IsEquivalentTo(new MulExpression(new NumExpression(40), new NumExpression(2)));

        /// <summary>
        /// Checks that pow parsing works correctly.
        /// </summary>
        [Fact]
        public static void PowParsing()
            => AssertThat(Parse("40 ^ 2")).IsEquivalentTo(new PowExpression(new NumExpression(40), new NumExpression(2)));

        /// <summary>
        /// Checks that add parsing works correctly.
        /// </summary>
        [Fact]
        public static void AddParsingLeftAssoc()
            => AssertThat(Parse("39 + 1 + 2"))
            .IsEquivalentTo(new AddExpression(new AddExpression(new NumExpression(39), new NumExpression(1)), new NumExpression(2)));

        /// <summary>
        /// Checks that sub parsing works correctly.
        /// </summary>
        [Fact]
        public static void SubParsingLeftAssoc()
            => AssertThat(Parse("39 - 1 - 2"))
            .IsEquivalentTo(new SubExpression(new SubExpression(new NumExpression(39), new NumExpression(1)), new NumExpression(2)));

        /// <summary>
        /// Checks that mul parsing works correctly.
        /// </summary>
        [Fact]
        public static void MulParsingLeftAssoc()
            => AssertThat(Parse("39 * 1 * 2"))
            .IsEquivalentTo(new MulExpression(new MulExpression(new NumExpression(39), new NumExpression(1)), new NumExpression(2)));

        /// <summary>
        /// Checks that pow parsing works correctly.
        /// </summary>
        [Fact]
        public static void PowParsingRightAssoc()
            => AssertThat(Parse("39 ^ 1 ^ 2"))
            .IsEquivalentTo(new PowExpression(new NumExpression(39), new PowExpression(new NumExpression(1), new NumExpression(2))));

        /// <summary>
        /// Checks that presedence works correctly.
        /// </summary>
        [Fact]
        public static void AddMulPresedence()
            => AssertThat(Parse("3 + 2 * 6 + 1"))
            .IsEquivalentTo(new AddExpression(new AddExpression(new NumExpression(3), new MulExpression(new NumExpression(2), new NumExpression(6))), new NumExpression(1)));

        /// <summary>
        /// Checks that simple increments work correctly.
        /// </summary>
        [Fact]
        public static void IncrSimple()
            => AssertThat(Parse("5++"))
            .IsEquivalentTo(new IncrExpression(new NumExpression(5)));

        private static Parser<string> Operator(string c)
            => String(c).Trim();

        private static Expression Parse(string input)
            => Exp.ThenEnd().Parse(input);

        private abstract class Expression
        {
        }

        private class NumExpression : Expression
        {
            public NumExpression(int value)
                => Value = value;

            public int Value { get; }
        }

        private abstract class BinaryExpression : Expression
        {
            public BinaryExpression(Expression left, Expression right)
            {
                Left = left;
                Right = right;
            }

            public Expression Left { get; }

            public Expression Right { get; }
        }

        private class AddExpression : BinaryExpression
        {
            public AddExpression(Expression left, Expression right)
                : base(left, right)
            {
            }
        }

        private class SubExpression : BinaryExpression
        {
            public SubExpression(Expression left, Expression right)
                : base(left, right)
            {
            }
        }

        private class MulExpression : BinaryExpression
        {
            public MulExpression(Expression left, Expression right)
                : base(left, right)
            {
            }
        }

        private class PowExpression : BinaryExpression
        {
            public PowExpression(Expression left, Expression right)
                : base(left, right)
            {
            }
        }

        private class IncrExpression : Expression
        {
            public IncrExpression(Expression expression)
                => Expression = expression;

            public Expression Expression { get; }
        }
    }
}
