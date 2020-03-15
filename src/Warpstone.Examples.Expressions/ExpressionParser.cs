using System;
using System.Collections.Generic;
using System.Linq;
using static Warpstone.Parsers;

namespace Warpstone.Examples.Expressions
{
    public static class ExpressionParser
    {
        private static readonly Parser<Expression> Num
            = Regex("[0-9]+").Transform(x => new NumExpression(int.Parse(x)) as Expression).Trim();

        private static readonly Parser<Expression> Parenthesized
            = Char('(')
            .Then(Lazy(() => Exp).Trim())
            .ThenSkip(Char(')'))
            .Trim();

        private static readonly Parser<Expression> Atom
            = Or(Parenthesized, Num).Trim();

        private static readonly Parser<char> Operator
            = Or(Char('+'), Char('*')).Trim();

        private static readonly Parser<Expression> Exp
            = Atom.ThenAdd(Many(Operator.ThenAdd(Atom)))
            .Transform(UnfoldExpression);

        private static Parser<T> Trim<T>(this Parser<T> parser)
            => OptionalWhitespaces.Then(parser).ThenSkip(OptionalWhitespaces);

        public static Expression Parse(string input)
            => Exp.ThenEnd().Parse(input);

        private static Expression UnfoldExpression(Expression head, IEnumerable<(char, Expression)> tail)
        {
            List<object> list = new List<object>();
            list.Add(head);
            foreach ((char op, Expression e) in tail)
            {
                list.Add(op);
                list.Add(e);
            }

            UnfoldExpression(list, '*', (l, r) => new MulExpression(l, r));
            UnfoldExpression(list, '+', (l, r) => new AddExpression(l, r));

            return list.First() as Expression;
        }

        private static void UnfoldExpression(List<object> list, char op, Func<Expression, Expression, Expression> transform)
        {
            int index = 1;
            while (index < list.Count)
            {
                if (list[index].Equals(op))
                {
                    Expression exp = transform(list[index - 1] as Expression, list[index + 1] as Expression);
                    list.RemoveAt(index + 1);
                    list.RemoveAt(index);
                    list.Insert(index, exp);
                    list.RemoveAt(index - 1);
                }
                else
                {
                    index += 2;
                }
            }
        }
    }
}
