using System;
using static Warpstone.Parsers;

namespace Warpstone.Sandbox
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Parser<string> parser = Many(Or(String("xyz"), String("abc"))).Transform(x => string.Join(string.Empty, x));
            ParseResult<string> result = parser.Parse("xyzabcabc");
            Console.WriteLine($"Success: {result.Success}, Value: {result.Value}");

            parser = Many(String("aaa"), String(", ")).Transform(x => string.Join(string.Empty, x));
            result = parser.Parse("");
            Console.WriteLine($"Success: {result.Success}, Value: {result.Value}");
        }
    }
}
