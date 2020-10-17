using System;
using System.Collections.Generic;
using System.Linq;
using Warpstone.Expressions;
using Warpstone.Parsers.InternalParsers;
using static Warpstone.Parsers.BasicParsers;

namespace Warpstone.Parsers
{
    /// <summary>
    /// Static class for dealing with expression parsers.
    /// </summary>
    public static class ExpressionParsers
    {
        /// <summary>
        /// Creates a unary operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="op">The operator.</param>
        /// <param name="transformation">The transformation.</param>
        /// <returns>A unary operation.</returns>
        public static IOperation<TOperator, TExpression> Post<TOperator, TExpression>(IParser<TOperator> op, UnaryOperatorTransform<TOperator, TExpression> transformation)
            => SingleUnary(Associativity.Left, op, transformation);

        /// <summary>
        /// Creates a unary operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="op">The operator.</param>
        /// <param name="transformation">The transformation.</param>
        /// <returns>A unary operation.</returns>
        public static IOperation<TOperator, TExpression> Post<TOperator, TExpression>(IParser<TOperator> op, UnaryOperatorTransform<TExpression> transformation)
            => Post(op, transformation.ExpandTransform<TOperator, TExpression>());

        /// <summary>
        /// Creates a unary operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A unary operation.</returns>
        public static IOperation<TOperator, TExpression> Post<TOperator, TExpression>(IEnumerable<(IParser<TOperator>, UnaryOperatorTransform<TOperator, TExpression>)> transformations)
            => Post(transformations.ToDictionary(x => x.Item1, x => x.Item2));

        /// <summary>
        /// Creates a unary operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A unary operation.</returns>
        public static IOperation<TOperator, TExpression> Post<TOperator, TExpression>(IEnumerable<(IParser<TOperator>, UnaryOperatorTransform<TExpression>)> transformations)
            => Post(transformations.Select(x => x.ExpandTransform()));

        /// <summary>
        /// Creates a unary operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="first">The first transformation.</param>
        /// <param name="others">The other transformations.</param>
        /// <returns>A unary operation.</returns>
        public static IOperation<TOperator, TExpression> Post<TOperator, TExpression>((IParser<TOperator>, UnaryOperatorTransform<TOperator, TExpression>) first, params (IParser<TOperator>, UnaryOperatorTransform<TOperator, TExpression>)[] others)
            => Post(others.Prepend(first));

        /// <summary>
        /// Creates a unary operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="first">The first transformation.</param>
        /// <param name="others">The other transformations.</param>
        /// <returns>A unary operation.</returns>
        public static IOperation<TOperator, TExpression> Post<TOperator, TExpression>((IParser<TOperator>, UnaryOperatorTransform<TExpression>) first, params (IParser<TOperator>, UnaryOperatorTransform<TExpression>)[] others)
            => Post(others.Prepend(first).Select(x => x.ExpandTransform()));

        /// <summary>
        /// Creates a unary operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A unary operation.</returns>
        public static IOperation<TOperator, TExpression> Post<TOperator, TExpression>(Dictionary<IParser<TOperator>, UnaryOperatorTransform<TOperator, TExpression>> transformations)
            => new UnaryOperation<TOperator, TExpression>(Associativity.Left, transformations);

        /// <summary>
        /// Creates a unary operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A unary operation.</returns>
        public static IOperation<TOperator, TExpression> Post<TOperator, TExpression>(Dictionary<IParser<TOperator>, UnaryOperatorTransform<TExpression>> transformations)
            => Post(transformations.ToDictionary(x => x.Key, x => x.Value.ExpandTransform<TOperator, TExpression>()));

        /// <summary>
        /// Creates a unary operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="op">The operator.</param>
        /// <param name="transformation">The transformation.</param>
        /// <returns>A unary operation.</returns>
        public static IOperation<TOperator, TExpression> Pre<TOperator, TExpression>(IParser<TOperator> op, UnaryOperatorTransform<TOperator, TExpression> transformation)
            => SingleUnary(Associativity.Right, op, transformation);

        /// <summary>
        /// Creates a unary operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="op">The operator.</param>
        /// <param name="transformation">The transformation.</param>
        /// <returns>A unary operation.</returns>
        public static IOperation<TOperator, TExpression> Pre<TOperator, TExpression>(IParser<TOperator> op, UnaryOperatorTransform<TExpression> transformation)
            => Pre(op, transformation.ExpandTransform<TOperator, TExpression>());

        /// <summary>
        /// Creates a unary operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A unary operation.</returns>
        public static IOperation<TOperator, TExpression> Pre<TOperator, TExpression>(IEnumerable<(IParser<TOperator>, UnaryOperatorTransform<TOperator, TExpression>)> transformations)
            => Pre(transformations.ToDictionary(x => x.Item1, x => x.Item2));

        /// <summary>
        /// Creates a unary operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A unary operation.</returns>
        public static IOperation<TOperator, TExpression> Pre<TOperator, TExpression>(IEnumerable<(IParser<TOperator>, UnaryOperatorTransform<TExpression>)> transformations)
            => Pre(transformations.Select(x => x.ExpandTransform()));

        /// <summary>
        /// Creates a unary operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="first">The first transformation.</param>
        /// <param name="others">The other transformations.</param>
        /// <returns>A unary operation.</returns>
        public static IOperation<TOperator, TExpression> Pre<TOperator, TExpression>((IParser<TOperator>, UnaryOperatorTransform<TOperator, TExpression>) first, params (IParser<TOperator>, UnaryOperatorTransform<TOperator, TExpression>)[] others)
            => Pre(others.Prepend(first));

        /// <summary>
        /// Creates a unary operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="first">The first transformation.</param>
        /// <param name="others">The other transformations.</param>
        /// <returns>A unary operation.</returns>
        public static IOperation<TOperator, TExpression> Pre<TOperator, TExpression>((IParser<TOperator>, UnaryOperatorTransform<TExpression>) first, params (IParser<TOperator>, UnaryOperatorTransform<TExpression>)[] others)
            => Pre(others.Prepend(first).Select(x => x.ExpandTransform()));

        /// <summary>
        /// Creates a unary operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A unary operation.</returns>
        public static IOperation<TOperator, TExpression> Pre<TOperator, TExpression>(Dictionary<IParser<TOperator>, UnaryOperatorTransform<TOperator, TExpression>> transformations)
            => new UnaryOperation<TOperator, TExpression>(Associativity.Right, transformations);

        /// <summary>
        /// Creates a unary operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A unary operation.</returns>
        public static IOperation<TOperator, TExpression> Pre<TOperator, TExpression>(Dictionary<IParser<TOperator>, UnaryOperatorTransform<TExpression>> transformations)
            => Pre(transformations.ToDictionary(x => x.Key, x => x.Value.ExpandTransform<TOperator, TExpression>()));

        /// <summary>
        /// Creates a left-to-right associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="op">The operator.</param>
        /// <param name="transformation">The transformation.</param>
        /// <returns>A left-to-right associative operation.</returns>
        public static IOperation<TOperator, TExpression> LeftToRight<TOperator, TExpression>(IParser<TOperator> op, BinaryOperatorTransform<TOperator, TExpression> transformation)
            => SingleOperation(Associativity.Left, op, transformation);

        /// <summary>
        /// Creates a left-to-right associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="op">The operator.</param>
        /// <param name="transformation">The transformation.</param>
        /// <returns>A left-to-right associative operation.</returns>
        public static IOperation<TOperator, TExpression> LeftToRight<TOperator, TExpression>(IParser<TOperator> op, BinaryOperatorTransform<TExpression> transformation)
            => LeftToRight(op, transformation.ExpandTransform<TOperator, TExpression>());

        /// <summary>
        /// Creates a left-to-right associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A left-to-right associative operation.</returns>
        public static IOperation<TOperator, TExpression> LeftToRight<TOperator, TExpression>(IEnumerable<(IParser<TOperator>, BinaryOperatorTransform<TOperator, TExpression>)> transformations)
            => LeftToRight(transformations.ToDictionary(x => x.Item1, x => x.Item2));

        /// <summary>
        /// Creates a left-to-right associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A left-to-right associative operation.</returns>
        public static IOperation<TOperator, TExpression> LeftToRight<TOperator, TExpression>(IEnumerable<(IParser<TOperator>, BinaryOperatorTransform<TExpression>)> transformations)
            => LeftToRight(transformations.Select(x => x.ExpandTransform()));

        /// <summary>
        /// Creates a left-to-right associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="first">The first transformation.</param>
        /// <param name="others">The other transformations.</param>
        /// <returns>A left-to-right associative operation.</returns>
        public static IOperation<TOperator, TExpression> LeftToRight<TOperator, TExpression>((IParser<TOperator>, BinaryOperatorTransform<TOperator, TExpression>) first, params (IParser<TOperator>, BinaryOperatorTransform<TOperator, TExpression>)[] others)
            => LeftToRight(others.Prepend(first));

        /// <summary>
        /// Creates a left-to-right associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="first">The first transformation.</param>
        /// <param name="others">The other transformations.</param>
        /// <returns>A left-to-right associative operation.</returns>
        public static IOperation<TOperator, TExpression> LeftToRight<TOperator, TExpression>((IParser<TOperator>, BinaryOperatorTransform<TExpression>) first, params (IParser<TOperator>, BinaryOperatorTransform<TExpression>)[] others)
            => LeftToRight(others.Prepend(first).Select(x => x.ExpandTransform()));

        /// <summary>
        /// Creates a left-to-right associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A left-to-right associative operation.</returns>
        public static IOperation<TOperator, TExpression> LeftToRight<TOperator, TExpression>(Dictionary<IParser<TOperator>, BinaryOperatorTransform<TOperator, TExpression>> transformations)
            => new BinaryOperation<TOperator, TExpression>(Associativity.Left, transformations);

        /// <summary>
        /// Creates a left-to-right associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A left-to-right associative operation.</returns>
        public static IOperation<TOperator, TExpression> LeftToRight<TOperator, TExpression>(Dictionary<IParser<TOperator>, BinaryOperatorTransform<TExpression>> transformations)
            => LeftToRight(transformations.ToDictionary(x => x.Key, x => x.Value.ExpandTransform<TOperator, TExpression>()));

        /// <summary>
        /// Creates a right-to-left associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="op">The operator.</param>
        /// <param name="transformation">The transformation.</param>
        /// <returns>A right-to-left associative operation.</returns>
        public static IOperation<TOperator, TExpression> RightToLeft<TOperator, TExpression>(IParser<TOperator> op, BinaryOperatorTransform<TOperator, TExpression> transformation)
            => SingleOperation(Associativity.Right, op, transformation);

        /// <summary>
        /// Creates a right-to-left associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="op">The operator.</param>
        /// <param name="transformation">The transformation.</param>
        /// <returns>A right-to-left associative operation.</returns>
        public static IOperation<TOperator, TExpression> RightToLeft<TOperator, TExpression>(IParser<TOperator> op, BinaryOperatorTransform<TExpression> transformation)
            => RightToLeft(op, transformation.ExpandTransform<TOperator, TExpression>());

        /// <summary>
        /// Creates a right-to-left associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A right-to-left associative operation.</returns>
        public static IOperation<TOperator, TExpression> RightToLeft<TOperator, TExpression>(IEnumerable<(IParser<TOperator>, BinaryOperatorTransform<TOperator, TExpression>)> transformations)
            => RightToLeft(transformations.ToDictionary(x => x.Item1, x => x.Item2));

        /// <summary>
        /// Creates a right-to-left associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A right-to-left associative operation.</returns>
        public static IOperation<TOperator, TExpression> RightToLeft<TOperator, TExpression>(IEnumerable<(IParser<TOperator>, BinaryOperatorTransform<TExpression>)> transformations)
            => RightToLeft(transformations.Select(x => x.ExpandTransform()));

        /// <summary>
        /// Creates a right-to-left associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="first">The first transformation.</param>
        /// <param name="others">The other transformations.</param>
        /// <returns>A right-to-left associative operation.</returns>
        public static IOperation<TOperator, TExpression> RightToLeft<TOperator, TExpression>((IParser<TOperator>, BinaryOperatorTransform<TOperator, TExpression>) first, params (IParser<TOperator>, BinaryOperatorTransform<TOperator, TExpression>)[] others)
            => RightToLeft(others.Prepend(first));

        /// <summary>
        /// Creates a right-to-left associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="first">The first transformation.</param>
        /// <param name="others">The other transformations.</param>
        /// <returns>A right-to-left associative operation.</returns>
        public static IOperation<TOperator, TExpression> RightToLeft<TOperator, TExpression>((IParser<TOperator>, BinaryOperatorTransform<TExpression>) first, params (IParser<TOperator>, BinaryOperatorTransform<TExpression>)[] others)
            => RightToLeft(others.Prepend(first).Select(x => x.ExpandTransform()));

        /// <summary>
        /// Creates a right-to-left associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A right-to-left associative operation.</returns>
        public static IOperation<TOperator, TExpression> RightToLeft<TOperator, TExpression>(Dictionary<IParser<TOperator>, BinaryOperatorTransform<TOperator, TExpression>> transformations)
            => new BinaryOperation<TOperator, TExpression>(Associativity.Right, transformations);

        /// <summary>
        /// Creates a right-to-left associative operation.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="transformations">The transformations.</param>
        /// <returns>A right-to-left associative operation.</returns>
        public static IOperation<TOperator, TExpression> RightToLeft<TOperator, TExpression>(Dictionary<IParser<TOperator>, BinaryOperatorTransform<TExpression>> transformations)
            => RightToLeft(transformations.ToDictionary(x => x.Key, x => x.Value.ExpandTransform<TOperator, TExpression>()));

        /// <summary>
        /// Creates a parser which parses a binary expression recursively.
        /// </summary>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="terminalParser">The terminal parser.</param>
        /// <param name="operations">The operations.</param>
        /// <returns>A parser parsing a binary expression recursively.</returns>
        public static IParser<TExpression> BuildExpression<TOperator, TExpression>(IParser<TExpression> terminalParser, IEnumerable<IOperation<TOperator, TExpression>> operations)
        {
            if (operations == null)
            {
                throw new ArgumentNullException(nameof(operations));
            }

            List<IParser<(IParser<TOperator>, TOperator)>> binaryOperators = new List<IParser<(IParser<TOperator>, TOperator)>>();
            List<IParser<(IParser<TOperator>, TOperator)>> preUnaryOperators = new List<IParser<(IParser<TOperator>, TOperator)>>();
            List<IParser<(IParser<TOperator>, TOperator)>> postUnaryOperators = new List<IParser<(IParser<TOperator>, TOperator)>>();
            foreach (var operation in operations)
            {
                if (operation is BinaryOperation<TOperator, TExpression> binaryOperation)
                {
                    foreach (var transformation in binaryOperation.Transformations)
                    {
                        binaryOperators.Add(transformation.Key.Transform(x => (transformation.Key, x)));
                    }
                }
                else if (operation is UnaryOperation<TOperator, TExpression> unaryOperation)
                {
                    if (operation.Associativity == Associativity.Right)
                    {
                        foreach (var transformation in unaryOperation.Transformations)
                        {
                            preUnaryOperators.Add(transformation.Key.Transform(x => (transformation.Key, x)));
                        }
                    }
                    else
                    {
                        foreach (var transformation in unaryOperation.Transformations)
                        {
                            postUnaryOperators.Add(transformation.Key.Transform(x => (transformation.Key, x)));
                        }
                    }
                }
            }

            IParser<(IParser<TOperator>, TOperator)> binaryOperatorParser = new FailureParser<(IParser<TOperator>, TOperator)>();

            for (int i = 0; i < binaryOperators.Count; i++)
            {
                binaryOperatorParser = Or(binaryOperatorParser, binaryOperators[i]);
            }

            IParser<(IParser<TOperator>, TOperator)> preUnaryOperatorParser = new FailureParser<(IParser<TOperator>, TOperator)>();

            for (int i = 0; i < preUnaryOperators.Count; i++)
            {
                preUnaryOperatorParser = Or(preUnaryOperatorParser, preUnaryOperators[i]);
            }

            IParser<(IParser<TOperator>, TOperator)> postUnaryOperatorParser = new FailureParser<(IParser<TOperator>, TOperator)>();

            for (int i = 0; i < postUnaryOperators.Count; i++)
            {
                postUnaryOperatorParser = Or(postUnaryOperatorParser, postUnaryOperators[i]);
            }

            IParser<OperatorTuple<TOperator>> binOpParser = binaryOperatorParser.Transform((x, y) => new OperatorTuple<TOperator>(x, y));
            IParser<OperatorTuple<TOperator>> preOpParser = preUnaryOperatorParser.Transform((x, y) => new OperatorTuple<TOperator>(x, y));
            IParser<OperatorTuple<TOperator>> postOpParser = postUnaryOperatorParser.Transform((x, y) => new OperatorTuple<TOperator>(x, y));
            IParser<ExpressionTuple<TOperator, TExpression>> expParser
                = Many(preOpParser)
                .ThenAdd(terminalParser)
                .ThenAdd(Many(postOpParser))
                .Transform((pre, e, post) => new ExpressionTuple<TOperator, TExpression>(pre, e, post));

            return expParser.ThenAdd(Many(binOpParser.ThenAdd(expParser)))
                .Transform((x, y) => UnfoldExpression(CreateList(x, y), operations))
                .WithName("expression");
        }

        private static IOperation<TOperator, TExpression> SingleOperation<TOperator, TExpression>(Associativity associativity, IParser<TOperator> op, BinaryOperatorTransform<TOperator, TExpression> transformation)
            => new BinaryOperation<TOperator, TExpression>(associativity, new Dictionary<IParser<TOperator>, BinaryOperatorTransform<TOperator, TExpression>>
            {
                { op, transformation },
            });

        private static IOperation<TOperator, TExpression> SingleUnary<TOperator, TExpression>(Associativity associativity, IParser<TOperator> op, UnaryOperatorTransform<TOperator, TExpression> transformation)
            => new UnaryOperation<TOperator, TExpression>(associativity, new Dictionary<IParser<TOperator>, UnaryOperatorTransform<TOperator, TExpression>>
            {
                { op, transformation },
            });

        private static List<object> CreateList<TOperator, TExpression>(ExpressionTuple<TOperator, TExpression> head, IEnumerable<(OperatorTuple<TOperator>, ExpressionTuple<TOperator, TExpression>)> tail)
        {
            List<object> list = new List<object>();
            list.AddRange(head.PreOperators);
            list.Add(head.Expression);
            list.AddRange(head.PostOperators);
            foreach ((OperatorTuple<TOperator> op, ExpressionTuple<TOperator, TExpression> exp) in tail)
            {
                list.Add(op);
                list.AddRange(exp.PreOperators);
                list.Add(exp.Expression);
                list.AddRange(exp.PostOperators);
            }

            return list;
        }

        private static TExpression UnfoldExpression<TOperator, TExpression>(List<object> list, IEnumerable<IOperation<TOperator, TExpression>> operations)
        {
            foreach (IOperation<TOperator, TExpression> operation in operations)
            {
                operation.UnfoldExpression(list);
            }

            return (TExpression)list[0];
        }

        private static BinaryOperatorTransform<TOperator, TExpression> ExpandTransform<TOperator, TExpression>(this BinaryOperatorTransform<TExpression> transformation)
            => (d, l, r) => transformation(l, r);

        private static (IParser<TOperator>, BinaryOperatorTransform<TOperator, TExpression>) ExpandTransform<TOperator, TExpression>(this (IParser<TOperator> op, BinaryOperatorTransform<TExpression> transformation) pair)
            => (pair.op, (d, l, r) => pair.transformation(l, r));

        private static UnaryOperatorTransform<TOperator, TExpression> ExpandTransform<TOperator, TExpression>(this UnaryOperatorTransform<TExpression> transformation)
            => (d, e) => transformation(e);

        private static (IParser<TOperator>, UnaryOperatorTransform<TOperator, TExpression>) ExpandTransform<TOperator, TExpression>(this (IParser<TOperator> op, UnaryOperatorTransform<TExpression> transformation) pair)
            => (pair.op, (d, e) => pair.transformation(e));
    }
}
