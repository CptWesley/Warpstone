using System.Collections.Generic;

namespace Warpstone.Expressions
{
    /// <summary>
    /// Delegate for operator transformations.
    /// </summary>
    /// <typeparam name="TOperator">The type of the operator.</typeparam>
    /// <typeparam name="TExpression">The type of the expression.</typeparam>
    /// <param name="operator">The operator.</param>
    /// <param name="l">The left argument of the operator.</param>
    /// <param name="r">The right argument of the operator.</param>
    /// <returns>The transformation which is applied.</returns>
    public delegate TExpression BinaryOperatorTransform<TOperator, TExpression>(TOperator @operator, TExpression l, TExpression r);

    /// <summary>
    /// Delegate for operator transformations.
    /// </summary>
    /// <typeparam name="TExpression">The type of the expression.</typeparam>
    /// <param name="l">The left argument of the operator.</param>
    /// <param name="r">The right argument of the operator.</param>
    /// <returns>The transformation which is applied.</returns>
    public delegate TExpression BinaryOperatorTransform<TExpression>(TExpression l, TExpression r);

    /// <summary>
    /// Delegate for operator transformations.
    /// </summary>
    /// <typeparam name="TOperator">The type of the operator.</typeparam>
    /// <typeparam name="TExpression">The type of the expression.</typeparam>
    /// <param name="operator">The operator.</param>
    /// <param name="e">The argument of the operator.</param>
    /// <returns>The transformation which is applied.</returns>
    public delegate TExpression UnaryOperatorTransform<TOperator, TExpression>(TOperator @operator, TExpression e);

    /// <summary>
    /// Base class for all expression patterns.
    /// </summary>
    /// <typeparam name="TOperator">The expression type.</typeparam>
    /// <typeparam name="TExpression">The type of the operator.</typeparam>
    public abstract class Operation<TOperator, TExpression>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Operation{TExpression, TOperator}"/> class.
        /// </summary>
        /// <param name="associativity">The associativity.</param>
        public Operation(Associativity associativity)
            => Associativity = associativity;

        /// <summary>
        /// Gets the associativity.
        /// </summary>
        public Associativity Associativity { get; }

        /// <summary>
        /// Gets the parser from an object.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>The parser.</returns>
        internal static Parser<TOperator> GetParser(object o)
            => ((OperatorTuple<TOperator>)o).Parser;

        /// <summary>
        /// Gets the operator from an object.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>The operator.</returns>
        internal static TOperator GetOperator(object o)
            => ((OperatorTuple<TOperator>)o).Operator;

        /// <summary>
        /// Checks if an object is an operator.
        /// </summary>
        /// <param name="o">The object.</param>
        /// <returns><c>true</c> if the specified object is an operator; otherwise, <c>false</c>.</returns>
        internal static bool IsOperator(object o)
            => o is OperatorTuple<TOperator>;

        /// <summary>
        /// Unfolds the expression right-to-left.
        /// </summary>
        /// <param name="list">The list of all expression parts.</param>
        internal abstract void UnfoldExpressionRight(List<object> list);

        /// <summary>
        /// Unfolds the expression left-to-right.
        /// </summary>
        /// <param name="list">The list of all expression parts.</param>
        internal abstract void UnfoldExpressionLeft(List<object> list);

        /// <summary>
        /// Unfolds the expression.
        /// </summary>
        /// <param name="list">The list.</param>
        internal void UnfoldExpression(List<object> list)
        {
            if (Associativity == Associativity.Left)
            {
                UnfoldExpressionLeft(list);
            }
            else
            {
                UnfoldExpressionRight(list);
            }
        }
    }
}
