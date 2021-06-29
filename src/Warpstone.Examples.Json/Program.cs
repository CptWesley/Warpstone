using System;
using System.Collections.Generic;
using Warpstone.SyntaxHighlighting;
using static Warpstone.Parsers.BasicParsers;

namespace Warpstone.Examples.Json
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            /*
            Parse(@"
{
    ""elements"": [null, null],
    ""name"": ""NullContainer"",
    ""body"": ""This is the first paragraph\nThis is the second paragraph"",
    ""inner"": {
        ""numVal"": 42
        ""boolVal"": true
    }
}");
            */
            IParser<string> aorb = Or(String("a"), String("b"));
            IParser<string> c = String("c");
            IParser<string> chain = null;
            chain = aorb.Then(Or(Lazy(() => chain), c));

            //Console.WriteLine(chain.ToRegex());
            //Console.WriteLine(JsonParser.GetGrammar());
            var x0 = PrintCaptures(new RegexLeaf("tr", Highlight.Comment));
            var x1 = PrintCaptures(new RegexAnd(x0, new RegexLeaf("ue", Highlight.Constant)));
            var x2 = PrintCaptures(new RegexOr(x1, x0));
            var x3 = PrintCaptures(new RegexOr(x2, new RegexAnd(new RegexAnd(new RegexLeaf("fal", Highlight.None), new RegexLeaf("s", Highlight.None)), new RegexLeaf("e", Highlight.Keyword))));
            var x4 = PrintCaptures(new RegexOr(new RegexAnd(new RegexAnd(x3, new RegexLeaf(",", Highlight.None)), x3), x3));

            var x5 = PrintCaptures(new RegexLeaf("a", Highlight.Keyword));
            var x6 = new RegexOr();
            x6.Left = new RegexAnd(x5, x6.ToRef());
            x6.Right = x5;
            PrintCaptures(x6);

            chain.PrintRegex();
            var chain23 = Multiple(chain, 2, 3);
            chain23.PrintRegex();
            JsonParser.Json.PrintRegex();
        }

        public static void Parse(string input)
            => Console.WriteLine(JsonParser.Parse(input));


        public static RegexNode PrintCaptures(RegexNode parser)
        {
            Console.WriteLine(parser);
            foreach (var x in parser.GetCaptures())
            {
                Console.WriteLine($"{x.Key}: {x.Value}");
            }
            Console.WriteLine();

            return parser;
        }

        public static void PrintRegex<T>(this IParser<T> parser)
            => Console.WriteLine(parser.ToRegex().Replace("\\", "\\\\").Replace("\"", "\\\"") + "\n");
    }
}
