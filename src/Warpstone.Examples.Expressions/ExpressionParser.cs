using static Warpstone.Parsers;

namespace Warpstone.Examples.Expressions
{
    public static class ExpressionParser
    {
        private static readonly Parser<Expression> Num
            = Regex("[0-9]+").Transform(x => new NumExpression(int.Parse(x)) as Expression);

        private static readonly Parser<Expression> Add
            = Or(
                Lazy(() => Num).ThenSkip(Char('+')).ThenAdd(Lazy(() => Exp))
                    .Transform(x => new AddExpression(x.Item1, x.Item2) as Expression),
                Num);

        private static readonly Parser<Expression> Mul
            = Or(
                Lazy(() => Add).ThenSkip(Char('*')).ThenAdd(Lazy(() => Exp))
                    .Transform(x => new MulExpression(x.Item1, x.Item2) as Expression),
                Add);

        private static readonly Parser<Expression> Exp
            = OptionalWhitespaces.Then(Mul).ThenSkip(OptionalWhitespaces);

        public static Expression Parse(string input)
            => Exp.ThenEnd().Parse(input);
    }
}
