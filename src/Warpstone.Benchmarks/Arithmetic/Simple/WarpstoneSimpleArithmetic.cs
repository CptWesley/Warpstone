using Warpstone.Parsers;
using static Warpstone.Parsers.BasicParsers;

namespace Warpstone.Benchmarks.Arithmetic.Simple;

public class WarpstoneSimpleArithmetic
{
    private static readonly IParser<string> Integer = Regex(@"0|-?[1-9][0-9]*");
    private static readonly IParser<string> Layout = Regex(@"(\s*)|(\/\*[\s\S]*?\*\/)|(\/\/.*)");

    private static readonly IParser<Exp> E4 = Char('(').ThenSkip(Layout).Then(Lazy(() => E0!)).ThenSkip(Layout).ThenSkip(Char(')'));

    private static readonly IParser<Exp> Number = Integer.Transform(x => new NumExp(int.Parse(x)));
    private static readonly IParser<Exp> E3 = Or(Number, E4);

    private static readonly IParser<Exp> Minus = Char('-').ThenSkip(Layout).Then(E3).Transform(x => new MinusExp(x));
    private static readonly IParser<Exp> E2 = Or(Minus, E3);

    private static readonly IParser<Exp> Mul = Lazy(() => E1!).ThenSkip(Layout).ThenSkip(Char('*')).ThenSkip(Layout).ThenAdd(E2).Transform((l, r) => new MulExp(l, r));
    private static readonly IParser<Exp> Div = Lazy(() => E1!).ThenSkip(Layout).ThenSkip(Char('/')).ThenSkip(Layout).ThenAdd(E2).Transform((l, r) => new DivExp(l, r));
    private static readonly IParser<Exp> E1 = Or(Mul, Div, E2);

    private static readonly IParser<Exp> Add = Lazy(() => E0!).ThenSkip(Layout).ThenSkip(Char('+')).ThenSkip(Layout).ThenAdd(E1).Transform((l, r) => new AddExp(l, r));
    private static readonly IParser<Exp> Sub = Lazy(() => E0!).ThenSkip(Layout).ThenSkip(Char('-')).ThenSkip(Layout).ThenAdd(E1).Transform((l, r) => new SubExp(l, r));
    private static readonly IParser<Exp> E0 = Or(Add, Sub, E1);

    public static Exp? Parse(string input)
    {
        IParseResult<Exp> result = E0.TryParse(input);

        if (!result.Success)
        {
            return null;
        }

        return result.Value;
    }
}
