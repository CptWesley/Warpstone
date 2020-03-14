using static Warpstone.Parsers;

namespace Warpstone.Examples.Expressions
{
    public static class ExpressionParser
    {
        private static readonly Parser<Expression> Num
            = Regex("[0-9]+").Transform(x => new NumExpression(int.Parse(x)) as Expression).Trim();

        private static readonly Parser<Expression> Mul
            = Or(Num.ThenSkip(Char('*')).ThenAdd(Lazy(() => Mul)).Transform((l, r) => new MulExpression(l, r) as Expression), Num).Trim();

        private static readonly Parser<Expression> Add
            = Or(Mul.ThenSkip(Char('+')).ThenAdd(Lazy(() => Add)).Transform((l, r) => new AddExpression(l, r) as Expression), Mul).Trim();

        private static readonly Parser<Expression> Exp
            = Add.Trim();

        private static Parser<T> Trim<T>(this Parser<T> parser)
            => OptionalWhitespaces.Then(parser).ThenSkip(OptionalWhitespaces);

        public static Expression Parse(string input)
            => Exp.ThenEnd().Parse(input);
    }
}
