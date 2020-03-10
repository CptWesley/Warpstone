using System;
using static Warpstone.Parsers;

namespace Warpstone.Sandbox
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Parser<string> parser = Many(Or(Char('x'), Char('y'))).Transform(x => string.Join(string.Empty, x));

            ParseResult<string> result = parser.Parse("xxxyyxxy");

            Console.WriteLine($"Success: {result.Success}, Value: {result.Value}");
        }
    }
}
