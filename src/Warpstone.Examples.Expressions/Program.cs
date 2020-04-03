using System;

namespace Warpstone.Examples.Expressions
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Parse("2+3*5*6+7*(3+2)-6/9");
        }

        private static void Parse(string input)
            => Console.WriteLine(ExpressionParser.Parse(input));
    }
}
