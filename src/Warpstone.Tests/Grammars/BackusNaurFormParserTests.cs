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
        private static readonly BackusNaurFormParser GrammarParser = new BackusNaurFormParser();

        /// <summary>
        /// Checks whether a very simple smoke test works.
        /// </summary>
        [Fact]
        public static void SimpleSmokeTest()
        {
            IParser<AstNode> parser = GrammarParser.CreateParser(@"
<bool> ::= <true> | <false>
<true> ::= ""true""
<false> ::= ""false""
");

            string input = @"true";
            AstNode node = parser.Parse(input);
            AssertThat(node).IsExactlyInstanceOf<ObjectNode>();
            ObjectNode? obj = node as ObjectNode;
            AssertThat(obj).IsNotNull();
            AssertThat(obj!.Type).IsEqualTo("bool");
            AssertThat(obj.Children).HasSize(1);
            AstNode child = obj.Children[0];
            AssertThat(child).IsExactlyInstanceOf<ObjectNode>();
            ObjectNode? childObj = child as ObjectNode;
            AssertThat(childObj!.Type).IsEqualTo("true");
            AssertThat(childObj).IsNotNull();
            AssertThat(childObj.Children).HasSize(1);
            AstNode innerChild = childObj.Children[0];
            AssertThat(innerChild).IsExactlyInstanceOf<StringNode>();
            StringNode? innerChildStr = innerChild as StringNode;
            AssertThat(innerChildStr).IsNotNull();
            AssertThat(innerChildStr!.Value).IsEqualTo("true");
            AssertThat(node.ToString()).IsEqualTo(@"bool(true(""true""))");
        }

        /// <summary>
        /// Checks that creating a parser for postal codes in the Netherlands works correctly.
        /// </summary>
        [Fact]
        public static void PostalCodeNetherlands()
        {
            IParser<AstNode> parser = GrammarParser.CreateParser(@"
<postal> ::= <digit> <digit> <digit> <digit> <letter> <letter>
<letter> ::= ""A"" | ""B"" | ""C"" | ""D"" | ""E"" | ""F"" | ""G"" | ""H"" | ""I"" | ""J"" | ""K"" | ""L"" | ""M"" | ""N"" | ""O"" | ""P"" | ""Q"" | ""R"" | ""S"" | ""T"" | ""U"" | ""V"" | ""W"" | ""X"" | ""Y"" | ""Z""
<digit> ::= ""0"" | ""1"" | ""2"" | ""3"" | ""4"" | ""5"" | ""6"" | ""7"" | ""8"" | ""9""
");
            AssertThat(parser.Parse("1234AB").ToString()).IsEqualTo(@"postal(digit(""1""), digit(""2""), digit(""3""), digit(""4""), letter(""A""), letter(""B""))");
        }
    }
}
