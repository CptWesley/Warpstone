using System;
using static Warpstone.Parsers;

namespace Warpstone.Sandbox
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Parser<string> parser = Many(Or(String("xyz"), String("abc"))).Transform(x => string.Join(string.Empty, x));
            ParseResult<string> result = parser.TryParse("xyzabcabc");
            Console.WriteLine($"Success: {result.Success}, Value: {result.Value}");

            Many(String("aaa"), String(", "))
                .Transform(x => string.Join(string.Empty, x))
                .ThenEnd()
                .PrintParse("aaax");
        }

        private static void PrintParse(this Parser<string> parser, string input)
        {
            ParseResult<string> result = parser.TryParse(input);
            if (result.Success)
            {
                Console.WriteLine($"Success: {result.Value}");
            }
            else
            {
                Console.WriteLine($"Failure: Expected {string.Join(", ", result.Expected)}, but found {input[result.Position]}.");
            }
        }
    }
}
