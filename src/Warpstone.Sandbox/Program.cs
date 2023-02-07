using System.Diagnostics;
using System.Text;
using Warpstone.Parsers;

using static Warpstone.Parsers.BasicParsers;

namespace Warpstone.Sandbox;

public static class Program
{
    private static readonly IParser<string> Integer = Regex(@"0|-?[1-9][0-9]*");
    private static readonly IParser<string> Layout = Regex(@"(\s*)|(\/\*[\s\S]*?\*\/)|(\/\/.*)");

    private static readonly IParser<Exp> E5 = Char('(').ThenSkip(Layout).Then(Lazy(() => E0!)).ThenSkip(Layout).ThenSkip(Char(')'));

    private static readonly IParser<Exp> Number = Integer.Transform(x => new NumExp(int.Parse(x)));
    private static readonly IParser<Exp> E4 = Or(Number, E5);

    private static readonly IParser<Exp> IncrementPostfix = E4.ThenSkip(Layout).ThenSkip(String("++")).Transform(x => new IncrementPostfixExp(x));
    private static readonly IParser<Exp> DecrementPostfix = E4.ThenSkip(Layout).ThenSkip(String("--")).Transform(x => new DecrementPostfixExp(x));
    private static readonly IParser<Exp> E3 = Or(IncrementPostfix, DecrementPostfix, E4);

    private static readonly IParser<Exp> Minus = Char('-').ThenSkip(Layout).Then(E3).Transform(x => new MinusExp(x));
    private static readonly IParser<Exp> IncrementPrefix = String("++").ThenSkip(Layout).Then(E3).Transform(x => new IncrementPrefixExp(x));
    private static readonly IParser<Exp> DecrementPrefix = String("--").ThenSkip(Layout).Then(E3).Transform(x => new DecrementPrefixExp(x));
    private static readonly IParser<Exp> E2 = Or(Minus, IncrementPrefix, DecrementPrefix, E3);

    private static readonly IParser<Exp> Mul = Lazy(() => E1!).ThenSkip(Layout).ThenSkip(Char('*')).ThenSkip(Layout).ThenAdd(E2).Transform((l, r) => new MulExp(l, r));
    private static readonly IParser<Exp> Div = Lazy(() => E1!).ThenSkip(Layout).ThenSkip(Char('/')).ThenSkip(Layout).ThenAdd(E2).Transform((l, r) => new DivExp(l, r));
    private static readonly IParser<Exp> E1 = Or(Mul, Div, E2);

    private static readonly IParser<Exp> Add = Lazy(() => E0!).ThenSkip(Layout).ThenSkip(Char('+')).ThenSkip(Layout).ThenAdd(E1).Transform((l, r) => new AddExp(l, r));
    private static readonly IParser<Exp> Sub = Lazy(() => E0!).ThenSkip(Layout).ThenSkip(Char('-')).ThenSkip(Layout).ThenAdd(E1).Transform((l, r) => new SubExp(l, r));
    private static readonly IParser<Exp> E0 = Or(Add, Sub, E1);



    private static readonly IParser<string> Digit = Or(String('0'), String('1'), String('2'), String('3'), String('4'), String('5'), String('6'), String('7'), String('8'), String('9')).WithName("DIGIT");
    private static readonly IParser<string> Num = Or(Lazy(() => Num!).ThenAdd(Digit).Transform((x, y) => x + y), Digit).WithName("Num");
    private static readonly IParser<string> Expr = Or(Lazy(() => Expr!).ThenAdd(String('+')).ThenAdd(Num).Transform((x, y, z) => x + y + z), Num).WithName("Expr");

    public static void Main(string[] args)
    {
        Console.WriteLine("Hello World!");
        string input1 = "45 + 3 * 72 + (54 - 2) * -(60 / 3) + 45 + 3 * 72 + (54 - 2) * -(60 / 3) + 45 + 3 * 72 + (54 - 2) * -(60 / 3) + 45 + 3 * 72 + (54 - 2) * -(60 / 3)";
        string input = string.Join(" * ", Enumerable.Repeat(input1, 100));
        ParseUnit<Exp> unit = new ParseUnit<Exp>(input, E0);
        //ParseUnit<Exp> unit = new ParseUnit<Exp>("(22)", E0);
        Stopwatch sw = Stopwatch.StartNew();
        unit.Parse();
        sw.Stop();
        Console.WriteLine(unit.Result);
        Console.WriteLine(sw.ElapsedMilliseconds);

        IParser<string>? z = null;
        z = Lazy(() => z!);
        Console.WriteLine(z.Parse("2423"));
        
        //ParseExpr("12+34");
        //ParseExpr("12+34+56");
    }

    private static void ParseExpr(string expr)
    {
        ParseUnit<string> unit = new ParseUnit<string>(expr, Expr);
        unit.Parse();
        Console.WriteLine(unit.Result);
        Console.WriteLine();
    }

    private abstract record Exp(int Precedence)
    {
        protected string InnerExpToString(Exp innerExp)
        {
            if (innerExp.Precedence < Precedence)
            {
                return $"({innerExp})";
            }

            return innerExp.ToString();
        }
    }

    private sealed record NumExp(int Value) : Exp(4)
    {
        public sealed override string ToString()
            => Value.ToString();
    }

    private abstract record BinOp(int Precedence, string Operator, Exp Left, Exp Right) : Exp(Precedence)
    {
        public sealed override string ToString()
            => $"{InnerExpToString(Left)} {Operator} {InnerExpToString(Right)}";
    }

    private abstract record PrefixOp(int Precedence, string Operator, Exp Exp) : Exp(Precedence)
    {
        public sealed override string ToString()
            => $"{Operator}{InnerExpToString(Exp)}";
    }

    private abstract record PostfixOp(int Precedence, string Operator, Exp Exp) : Exp(Precedence)
    {
        public sealed override string ToString()
            => $"{InnerExpToString(Exp)}{Operator}";
    }

    private record AddExp(Exp Left, Exp Right) : BinOp(0, "+", Left, Right);

    private record SubExp(Exp Left, Exp Right) : BinOp(0, "-", Left, Right);

    private record MulExp(Exp Left, Exp Right) : BinOp(1, "*", Left, Right);

    private record DivExp(Exp Left, Exp Right) : BinOp(1, "/", Left, Right);

    private record MinusExp(Exp Exp) : PrefixOp(2, "-", Exp);

    private record IncrementPrefixExp(Exp Exp) : PrefixOp(2, "++", Exp);

    private record DecrementPrefixExp(Exp Exp) : PrefixOp(2, "--", Exp);

    private record IncrementPostfixExp(Exp Exp) : PrefixOp(3, "++", Exp);

    private record DecrementPostfixExp(Exp Exp) : PrefixOp(3, "--", Exp);
}