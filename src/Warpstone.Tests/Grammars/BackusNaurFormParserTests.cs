using Warpstone.Grammars;
using Xunit;
using static AssertNet.Assertions;

namespace Warpstone.Tests.Grammars
{
    /// <summary>
    /// Test class for the <see cref="BackusNaurFormParser"/> class.
    /// </summary>
    public static class BackusNaurFormParserTests
    {
        /// <summary>
        /// Checks whether a very simple smoke test works.
        /// </summary>
        [Fact]
        public static void SimpleSmokeTest()
        {
            BackusNaurFormParser grammarParser = new BackusNaurFormParser();

            Parser<AstNode> parser = grammarParser.CreateParser(@"
<bool> ::= <true> | <false>
<true> ::= ""true""
<false> ::= ""false""
");

            string input = @"true";
            AstNode node = parser.Parse(input);
            AssertThat(node).IsExactlyInstanceOf<ObjectNode>();
            ObjectNode obj = node as ObjectNode;
            AssertThat(obj.Type).IsEqualTo("bool");
            AssertThat(obj.Children).HasSize(1);
            AstNode child = obj.Children[0];
            AssertThat(child).IsExactlyInstanceOf<ObjectNode>();
            ObjectNode childObj = child as ObjectNode;
            AssertThat(childObj.Type).IsEqualTo("true");
            AssertThat(childObj.Children).HasSize(1);
            AstNode innerChild = childObj.Children[0];
            AssertThat(innerChild).IsExactlyInstanceOf<StringNode>();
            StringNode innerChildStr = innerChild as StringNode;
            AssertThat(innerChildStr.Value).IsEqualTo("true");
        }
    }
}
