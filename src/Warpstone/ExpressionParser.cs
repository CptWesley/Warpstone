using System;
using System.Collections.Generic;
using System.Linq;
using Warpstone.Expressions;
using static Warpstone.Parsers;

namespace Warpstone
{
    /// <summary>
    /// Static class for dealing with expression parsers.
    /// </summary>
    public static class ExpressionParser
    {
        /// <summary>
        /// Creates a left-to-right associative operation.
        /// </summary>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <param name="op">The operator.</param>
        /// <param name="transformation">The transformation.</param>
        /// <returns>A left-to-right associative operation.</returns>
        public static Operation<TExpression, TOperator> LeftToRight<TExpression, TOperator>(Parser<TOperator> op, Func<TExpression, TExpression, TExpression> transformation)
            => SingleOperation(Associativity.Left, op, transformation);

        /// <summary>
        /// Creates a left-to-right associative operation.
        /// </summary>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A left-to-right associative operation.</returns>
        public static Operation<TExpression, TOperator> LeftToRight<TExpression, TOperator>(IEnumerable<(Parser<TOperator>, Func<TExpression, TExpression, TExpression>)> transformations)
            => LeftToRight(transformations.ToDictionary(x => x.Item1, x => x.Item2));

        /// <summary>
        /// Creates a left-to-right associative operation.
        /// </summary>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A left-to-right associative operation.</returns>
        public static Operation<TExpression, TOperator> LeftToRight<TExpression, TOperator>(Dictionary<Parser<TOperator>, Func<TExpression, TExpression, TExpression>> transformations)
            => new Operation<TExpression, TOperator>(Associativity.Left, transformations);

        /// <summary>
        /// Creates a right-to-left associative operation.
        /// </summary>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <param name="op">The operator.</param>
        /// <param name="transformation">The transformation.</param>
        /// <returns>A right-to-left associative operation.</returns>
        public static Operation<TExpression, TOperator> RightToLeft<TExpression, TOperator>(Parser<TOperator> op, Func<TExpression, TExpression, TExpression> transformation)
            => SingleOperation(Associativity.Right, op, transformation);

        /// <summary>
        /// Creates a right-to-left associative operation.
        /// </summary>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A right-to-left associative operation.</returns>
        public static Operation<TExpression, TOperator> RightToLeft<TExpression, TOperator>(IEnumerable<(Parser<TOperator>, Func<TExpression, TExpression, TExpression>)> transformations)
            => RightToLeft(transformations.ToDictionary(x => x.Item1, x => x.Item2));

        /// <summary>
        /// Creates a right-to-left associative operation.
        /// </summary>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A right-to-left associative operation.</returns>
        public static Operation<TExpression, TOperator> RightToLeft<TExpression, TOperator>(Dictionary<Parser<TOperator>, Func<TExpression, TExpression, TExpression>> transformations)
            => new Operation<TExpression, TOperator>(Associativity.Right, transformations);

        /// <summary>
        /// Creates a parser which parses a binary expression recursively.
        /// </summary>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <param name="terminalParser">The terminal parser.</param>
        /// <param name="operations">The operations.</param>
        /// <returns>A parser parsing a binary expression recursively.</returns>
        public static Parser<TExpression> BinaryExpression<TExpression, TOperator>(Parser<TExpression> terminalParser, IEnumerable<Operation<TExpression, TOperator>> operations)
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

        private static Operation<TExpression, TOperator> SingleOperation<TExpression, TOperator>(Associativity associativity, Parser<TOperator> op, Func<TExpression, TExpression, TExpression> transformation)
            => new Operation<TExpression, TOperator>(associativity, new Dictionary<Parser<TOperator>, Func<TExpression, TExpression, TExpression>>
            {
                { op, transformation },
            });

        private static TExpression UnfoldExpression<TExpression, TOperator>(TExpression head, IEnumerable<(Parser<TOperator>, TExpression)> tail, IEnumerable<Operation<TExpression>> operations)
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

            foreach (Operation<TExpression> operation in operations)
            {
                if (operation.Associativity == Associativity.Left)
                {
                    UnfoldExpressionLeft<TExpression, TOperator>(list, operation);
                }
                else
                {
                    UnfoldExpressionRight<TExpression, TOperator>(list, operation);
                }
            }

            return (TExpression)list[0];
        }

        private static void UnfoldExpressionLeft<TExpression, TOperator>(List<object> list, Operation<TExpression> operation)
        {
            if (operation is Operation<TExpression, TOperator> binop)
            {
                int index = 1;
                while (index < list.Count)
                {
                    if (binop.Transformations.TryGetValue((Parser<TOperator>)list[index], out Func<TExpression, TExpression, TExpression> transformation))
                    {
                        ReplaceExpression(list, index, transformation((TExpression)list[index - 1], (TExpression)list[index + 1]));
                    }
                    else
                    {
                        index += 2;
                    }
                }
            }
        }

        private static void UnfoldExpressionRight<TExpression, TOperator>(List<object> list, Operation<TExpression> operation)
        {
            if (operation is Operation<TExpression, TOperator> binop)
            {
                int index = list.Count - 2;
                while (index > 0)
                {
                    if (binop.Transformations.TryGetValue((Parser<TOperator>)list[index], out Func<TExpression, TExpression, TExpression> transformation))
                    {
                        ReplaceExpression(list, index, transformation((TExpression)list[index - 1], (TExpression)list[index + 1]));
                    }
                    else
                    {
                        index -= 2;
                    }
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
    }
}
