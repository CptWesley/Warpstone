using System;

namespace Warpstone.Examples.Expressions
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Parse("2 + 6 * 3");
        }

        private static void Parse(string input)
            => Console.WriteLine(ExpressionParser.Parse(input));
    }
}
