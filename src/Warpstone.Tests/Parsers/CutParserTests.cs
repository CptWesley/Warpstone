using Xunit;
using static AssertNet.Assertions;
using static Warpstone.Parsers.BasicParsers;
using static Warpstone.Parsers.CommonParsers;

namespace Warpstone.Tests.Parsers
{
    /// <summary>
    /// Test class for the cut parser.
    /// </summary>
    public static class CutParserTests
    {
        private static readonly IParser<object> Boolean = CompiledRegex("true|false");
        private static readonly IParser<object> Identifier = CompiledRegex(@"\w+");
        private static readonly IParser<object> Array = Char('[').Then(Many(Lazy(() => Expr!), Char(','), Char(']')));
        private static readonly IParser<object> Expr = Or(Boolean, Identifier, Array);
        private static readonly IParser<object> Exprs = Many(Expr, OptionalWhitespaces, End);

        /// <summary>
        /// Checks that simple cut works.
        /// </summary>
        [Fact]
        public static void SimpleCutPositive()
        {
            IParser<string> parser = DontBacktrack(String("x"));
            IParseResult<string> result = parser.TryParse("x");
            AssertThat(result.Success).IsTrue();
            AssertThat(result.Value).IsEqualTo("x");
        }

        /// <summary>
        /// Checks that simple cut works.
        /// </summary>
        [Fact]
        public static void SimpleCutNegative()
        {
            IParser<string> parser = DontBacktrack(String("x"));
            IParseResult<string> result = parser.TryParse("y");
            AssertThat(result.Success).IsFalse();
            AssertThat(result.Error).IsNotNull();
            AssertThat(result.Error!.AllowBacktracking).IsFalse();
        }

        /// <summary>
        /// Checks that cut works.
        /// </summary>
        [Fact]
        public static void SimpleCutOr()
        {
            //IParseResult<object> r1 = ExprPerLineEnd.TryParse("[true,false,x]");
            //AssertThat(r1.Success).IsTrue();
            //IParseResult<object> r2 = ExprPerLineEnd.TryParse("[true,false,x,truex,y]");
            //AssertThat(r2.Success).IsFalse();
            //AssertThat(r2.Error).IsNotNull();

            IParseResult<object> r3 = Exprs.TryParse("[[true,false],[false],true,[false,truex,false],true]");
            //AssertThat(r3.Success).IsTrue();

            //AssertThat(r2.Error!.GetMessage()).EndsWith("but found 'x'. At 1:19.");
            throw new System.Exception();
        }
    }
}
