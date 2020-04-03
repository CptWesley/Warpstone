using System;

namespace Warpstone.Examples.Expressions
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Parse("2+3*5*6+7*(3+2)-6/9+2-3-4+5");
            Parse("2^3^2");
            Parse("6+5^6^(2+3-5)");
            Parse("40+2");
        }

        private static void Parse(string input)
        {
            Expression expr = ExpressionParser.Parse(input);
            Console.WriteLine($"{expr} = {expr.Evaluate()}");
        }
    }
}
