using System;
using System.Collections.Generic;
using System.Linq;
using Warpstone.Expressions;
using static Warpstone.Parsers.BasicParsers;

namespace Warpstone.Parsers
{
    /// <summary>
    /// Static class for dealing with expression parsers.
    /// </summary>
    public static class ExpressionParsers
    {
        /// <summary>
        /// Creates a left-to-right associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="op">The operator.</param>
        /// <param name="transformation">The transformation.</param>
        /// <returns>A left-to-right associative operation.</returns>
        public static Operation<TOperator, TExpression> LeftToRight<TOperator, TExpression>(Parser<TOperator> op, OperatorTransform<TOperator, TExpression> transformation)
            => SingleOperation(Associativity.Left, op, transformation);

        /// <summary>
        /// Creates a left-to-right associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="op">The operator.</param>
        /// <param name="transformation">The transformation.</param>
        /// <returns>A left-to-right associative operation.</returns>
        public static Operation<TOperator, TExpression> LeftToRight<TOperator, TExpression>(Parser<TOperator> op, OperatorTransform<TExpression> transformation)
            => LeftToRight(op, transformation.ExpandTransform<TOperator, TExpression>());

        /// <summary>
        /// Creates a left-to-right associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A left-to-right associative operation.</returns>
        public static Operation<TOperator, TExpression> LeftToRight<TOperator, TExpression>(IEnumerable<(Parser<TOperator>, OperatorTransform<TOperator, TExpression>)> transformations)
            => LeftToRight(transformations.ToDictionary(x => x.Item1, x => x.Item2));

        /// <summary>
        /// Creates a left-to-right associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A left-to-right associative operation.</returns>
        public static Operation<TOperator, TExpression> LeftToRight<TOperator, TExpression>(IEnumerable<(Parser<TOperator>, OperatorTransform<TExpression>)> transformations)
            => LeftToRight(transformations.Select(x => x.ExpandTransform()));

        /// <summary>
        /// Creates a left-to-right associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="first">The first transformation.</param>
        /// <param name="others">The other transformations.</param>
        /// <returns>A left-to-right associative operation.</returns>
        public static Operation<TOperator, TExpression> LeftToRight<TOperator, TExpression>((Parser<TOperator>, OperatorTransform<TOperator, TExpression>) first, params (Parser<TOperator>, OperatorTransform<TOperator, TExpression>)[] others)
            => LeftToRight(others.Prepend(first));

        /// <summary>
        /// Creates a left-to-right associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="first">The first transformation.</param>
        /// <param name="others">The other transformations.</param>
        /// <returns>A left-to-right associative operation.</returns>
        public static Operation<TOperator, TExpression> LeftToRight<TOperator, TExpression>((Parser<TOperator>, OperatorTransform<TExpression>) first, params (Parser<TOperator>, OperatorTransform<TExpression>)[] others)
            => LeftToRight(others.Prepend(first).Select(x => x.ExpandTransform()));

        /// <summary>
        /// Creates a left-to-right associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A left-to-right associative operation.</returns>
        public static Operation<TOperator, TExpression> LeftToRight<TOperator, TExpression>(Dictionary<Parser<TOperator>, OperatorTransform<TOperator, TExpression>> transformations)
            => new Operation<TOperator, TExpression>(Associativity.Left, transformations);

        /// <summary>
        /// Creates a left-to-right associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A left-to-right associative operation.</returns>
        public static Operation<TOperator, TExpression> LeftToRight<TOperator, TExpression>(Dictionary<Parser<TOperator>, OperatorTransform<TExpression>> transformations)
            => LeftToRight(transformations.ToDictionary(x => x.Key, x => x.Value.ExpandTransform<TOperator, TExpression>()));

        /// <summary>
        /// Creates a right-to-left associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="op">The operator.</param>
        /// <param name="transformation">The transformation.</param>
        /// <returns>A right-to-left associative operation.</returns>
        public static Operation<TOperator, TExpression> RightToLeft<TOperator, TExpression>(Parser<TOperator> op, OperatorTransform<TOperator, TExpression> transformation)
            => SingleOperation(Associativity.Right, op, transformation);

        /// <summary>
        /// Creates a right-to-left associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="op">The operator.</param>
        /// <param name="transformation">The transformation.</param>
        /// <returns>A right-to-left associative operation.</returns>
        public static Operation<TOperator, TExpression> RightToLeft<TOperator, TExpression>(Parser<TOperator> op, OperatorTransform<TExpression> transformation)
            => RightToLeft(op, transformation.ExpandTransform<TOperator, TExpression>());

        /// <summary>
        /// Creates a right-to-left associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A right-to-left associative operation.</returns>
        public static Operation<TOperator, TExpression> RightToLeft<TOperator, TExpression>(IEnumerable<(Parser<TOperator>, OperatorTransform<TOperator, TExpression>)> transformations)
            => RightToLeft(transformations.ToDictionary(x => x.Item1, x => x.Item2));

        /// <summary>
        /// Creates a right-to-left associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A right-to-left associative operation.</returns>
        public static Operation<TOperator, TExpression> RightToLeft<TOperator, TExpression>(IEnumerable<(Parser<TOperator>, OperatorTransform<TExpression>)> transformations)
            => RightToLeft(transformations.Select(x => x.ExpandTransform()));

        /// <summary>
        /// Creates a right-to-left associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="first">The first transformation.</param>
        /// <param name="others">The other transformations.</param>
        /// <returns>A right-to-left associative operation.</returns>
        public static Operation<TOperator, TExpression> RightToLeft<TOperator, TExpression>((Parser<TOperator>, OperatorTransform<TOperator, TExpression>) first, params (Parser<TOperator>, OperatorTransform<TOperator, TExpression>)[] others)
            => RightToLeft(others.Prepend(first));

        /// <summary>
        /// Creates a right-to-left associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="first">The first transformation.</param>
        /// <param name="others">The other transformations.</param>
        /// <returns>A right-to-left associative operation.</returns>
        public static Operation<TOperator, TExpression> RightToLeft<TOperator, TExpression>((Parser<TOperator>, OperatorTransform<TExpression>) first, params (Parser<TOperator>, OperatorTransform<TExpression>)[] others)
            => RightToLeft(others.Prepend(first).Select(x => x.ExpandTransform()));

        /// <summary>
        /// Creates a right-to-left associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A right-to-left associative operation.</returns>
        public static Operation<TOperator, TExpression> RightToLeft<TOperator, TExpression>(Dictionary<Parser<TOperator>, OperatorTransform<TOperator, TExpression>> transformations)
            => new Operation<TOperator, TExpression>(Associativity.Right, transformations);

        /// <summary>
        /// Creates a right-to-left associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A right-to-left associative operation.</returns>
        public static Operation<TOperator, TExpression> RightToLeft<TOperator, TExpression>(Dictionary<Parser<TOperator>, OperatorTransform<TExpression>> transformations)
            => RightToLeft(transformations.ToDictionary(x => x.Key, x => x.Value.ExpandTransform<TOperator, TExpression>()));

        /// <summary>
        /// Creates a parser which parses a binary expression recursively.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="terminalParser">The terminal parser.</param>
        /// <param name="operations">The operations.</param>
        /// <returns>A parser parsing a binary expression recursively.</returns>
        public static Parser<TExpression> BuildExpression<TOperator, TExpression>(Parser<TExpression> terminalParser, IEnumerable<Operation<TOperator, TExpression>> operations)
        {
            if (operations == null)
            {
                throw new ArgumentNullException(nameof(operations));
            }

            List<Parser<Parser<TOperator>>> operators = new List<Parser<Parser<TOperator>>>();
            foreach (var operation in operations)
            {
                foreach (var transformation in operation.Transformations)
                {
                    operators.Add(transformation.Key.Transform(x => transformation.Key));
                }
            }

            if (operators.Count <= 0)
            {
                throw new ArgumentException("Requires at least one operator.", nameof(operations));
            }

            Parser<Parser<TOperator>> operatorParser = operators[0];

            for (int i = 1; i < operators.Count; i++)
            {
                operatorParser = Or(operatorParser, operators[i]);
            }

            return terminalParser.ThenAdd(Many(operatorParser.ThenAdd(terminalParser)))
            .Transform((x, y) => UnfoldExpression(x, y, operations));
        }

        private static Operation<TOperator, TExpression> SingleOperation<TOperator, TExpression>(Associativity associativity, Parser<TOperator> op, OperatorTransform<TOperator, TExpression> transformation)
            => new Operation<TOperator, TExpression>(associativity, new Dictionary<Parser<TOperator>, OperatorTransform<TOperator, TExpression>>
            {
                { op, transformation },
            });

        private static TExpression UnfoldExpression<TOperator, TExpression>(TExpression head, IEnumerable<(Parser<TOperator>, TExpression)> tail, IEnumerable<Operation<TOperator, TExpression>> operations)
        {
            List<object> list = new List<object>
            {
                head,
            };
            foreach ((Parser<TOperator> op, TExpression e) in tail)
            {
                list.Add(op);
                list.Add(e);
            }

            foreach (Operation<TOperator, TExpression> operation in operations)
            {
                if (operation.Associativity == Associativity.Left)
                {
                    UnfoldExpressionLeft(list, operation);
                }
                else
                {
                    UnfoldExpressionRight(list, operation);
                }
            }

            return (TExpression)list[0];
        }

        private static void UnfoldExpressionLeft<TOperator, TExpression>(List<object> list, Operation<TOperator, TExpression> operation)
        {
            if (operation is Operation<TOperator, TExpression> binop)
            {
                int index = 1;
                while (index < list.Count)
                {
                    if (binop.Transformations.TryGetValue((Parser<TOperator>)list[index], out OperatorTransform<TOperator, TExpression> transformation))
                    {
                        ReplaceExpression(list, index, transformation((TOperator)list[index], (TExpression)list[index - 1], (TExpression)list[index + 1]));
                    }
                    else
                    {
                        index += 2;
                    }
                }
            }
        }

        private static void UnfoldExpressionRight<TOperator, TExpression>(List<object> list, Operation<TOperator, TExpression> operation)
        {
            if (operation is Operation<TOperator, TExpression> binop)
            {
                int index = list.Count - 2;
                while (index > 0)
                {
                    if (binop.Transformations.TryGetValue((Parser<TOperator>)list[index], out OperatorTransform<TOperator, TExpression> transformation))
                    {
                        ReplaceExpression(list, index, transformation((TOperator)list[index], (TExpression)list[index - 1], (TExpression)list[index + 1]));
                    }

                    index -= 2;
                }
            }
        }

        private static void ReplaceExpression<TOutput>(List<object> list, int index, TOutput value)
        {
            list.RemoveAt(index + 1);
            list.RemoveAt(index);
            list.Insert(index, value);
            list.RemoveAt(index - 1);
        }

        private static OperatorTransform<TOperator, TExpression> ExpandTransform<TOperator, TExpression>(this OperatorTransform<TExpression> transformation)
            => (d, l, r) => transformation(l, r);

        private static (Parser<TOperator>, OperatorTransform<TOperator, TExpression>) ExpandTransform<TOperator, TExpression>(this (Parser<TOperator> op, OperatorTransform<TExpression> transformation) pair)
            => (pair.op, (d, l, r) => pair.transformation(l, r));
    }
}
