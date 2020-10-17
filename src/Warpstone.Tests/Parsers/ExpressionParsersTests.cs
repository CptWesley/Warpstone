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
        private static readonly IParser<Expression> Num
            = Regex("[0-9]+").Transform(x => new NumExpression(int.Parse(x, CultureInfo.InvariantCulture))).Trim();

        [SuppressMessage("Readability Rules", "SA1118", Justification = "Nicer to look at.")]
        [SuppressMessage("Readability Rules", "SA1009", Justification = "Nicer to look at.")]
        [SuppressMessage("Readability Rules", "SA1111", Justification = "Nicer to look at.")]
        private static readonly IParser<Expression> Exp
            = BuildExpression(Num, new[]
            {
                Post<string, Expression>(
                    (Operator("[]"), (e) => new ArrayExpression(e))
                ),
                Pre<string, Expression>(
                    (Operator("++"), (e) => new PreIncrExpression(e)),
                    (Operator("--"), (e) => new PreDecrExpression(e))
                ),
                Post<string, Expression>(
                    (Operator("++"), (e) => new PostIncrExpression(e)),
                    (Operator("--"), (e) => new PostDecrExpression(e))
                ),
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
        public static void PostIncrSimple()
            => AssertThat(Parse("5++"))
            .IsEquivalentTo(new PostIncrExpression(new NumExpression(5)));

        /// <summary>
        /// Checks that simple increments work correctly.
        /// </summary>
        [Fact]
        public static void PostIncrDoubleSimple()
            => AssertThat(Parse("5++++"))
            .IsEquivalentTo(new PostIncrExpression(new PostIncrExpression(new NumExpression(5))));

        /// <summary>
        /// Checks that simple unary operators work correctly.
        /// </summary>
        [Fact]
        public static void PostIncrDecrSimple()
            => AssertThat(Parse("5++--"))
            .IsEquivalentTo(new PostDecrExpression(new PostIncrExpression(new NumExpression(5))));

        /// <summary>
        /// Checks that simple decrements work correctly.
        /// </summary>
        [Fact]
        public static void PreDecrSimple()
            => AssertThat(Parse("--5"))
            .IsEquivalentTo(new PreDecrExpression(new NumExpression(5)));

        /// <summary>
        /// Checks that simple decrements work correctly.
        /// </summary>
        [Fact]
        public static void PreDecrDoubleSimple()
            => AssertThat(Parse("----5"))
            .IsEquivalentTo(new PreDecrExpression(new PreDecrExpression(new NumExpression(5))));

        /// <summary>
        /// Checks that simple unary operators work correctly.
        /// </summary>
        [Fact]
        public static void PreIncrDecrSimple()
            => AssertThat(Parse("--++5"))
            .IsEquivalentTo(new PreDecrExpression(new PreIncrExpression(new NumExpression(5))));

        /// <summary>
        /// Checks that simple unary operators work correctly.
        /// </summary>
        [Fact]
        public static void PreIncrPostDecrSimple()
            => AssertThat(Parse("++5--"))
            .IsEquivalentTo(new PostDecrExpression(new PreIncrExpression(new NumExpression(5))));

        /// <summary>
        /// Checks that simple unary operators work correctly.
        /// </summary>
        [Fact]
        public static void MixedUnary()
            => AssertThat(Parse("++--5--++"))
            .IsEquivalentTo(new PostIncrExpression(new PostDecrExpression(new PreIncrExpression(new PreDecrExpression(new NumExpression(5))))));

        /// <summary>
        /// Checks that simple unary operators work correctly.
        /// </summary>
        [Fact]
        public static void UnaryInBinary()
            => AssertThat(Parse("5 * ++--5--++ + 6--"))
            .IsEquivalentTo(new AddExpression(new MulExpression(Parse("5"), Parse("++--5--++")), Parse("6--")));

        /// <summary>
        /// Checks that simple unary operators work correctly.
        /// </summary>
        [Fact]
        public static void PostPriorityCorrect()
            => AssertThat(Parse("5[]++"))
            .IsEquivalentTo(new PostIncrExpression(new ArrayExpression(new NumExpression(5))));

        /// <summary>
        /// Checks that simple unary operators work correctly.
        /// </summary>
        [Fact]
        public static void PostPriorityIncorrect()
        {
            IParseResult<Expression> result = Exp.ThenEnd().TryParse("5++[]");
            AssertThat(result.Success).IsFalse();
            AssertThat(result.Error).IsExactlyInstanceOf<UnexpectedTokenError>();
            AssertThat(((UnexpectedTokenError)result.Error).Expected).ContainsExactly("expression");
        }

        private static IParser<string> Operator(string c)
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

        private class UnaryExpression : Expression
        {
            public UnaryExpression(Expression expression)
                => Expression = expression;

            public Expression Expression { get; }
        }

        private class PreIncrExpression : UnaryExpression
        {
            public PreIncrExpression(Expression expression)
                : base(expression)
            {
            }
        }

        private class PreDecrExpression : UnaryExpression
        {
            public PreDecrExpression(Expression expression)
                : base(expression)
            {
            }
        }

        private class PostIncrExpression : UnaryExpression
        {
            public PostIncrExpression(Expression expression)
                : base(expression)
            {
            }
        }

        private class PostDecrExpression : UnaryExpression
        {
            public PostDecrExpression(Expression expression)
                : base(expression)
            {
            }
        }

        private class ArrayExpression : UnaryExpression
        {
            public ArrayExpression(Expression expression)
                : base(expression)
            {
            }
        }
    }
}
