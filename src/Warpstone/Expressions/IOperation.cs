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
    /// Delegate for operator transformations.
    /// </summary>
    /// <typeparam name="TExpression">The type of the expression.</typeparam>
    /// <param name="e">The argument of the operator.</param>
    /// <returns>The transformation which is applied.</returns>
    public delegate TExpression UnaryOperatorTransform<TExpression>(TExpression e);

    /// <summary>
    /// Base class for all expression patterns.
    /// </summary>
    /// <typeparam name="TOperator">The expression type.</typeparam>
    /// <typeparam name="TExpression">The type of the operator.</typeparam>
    public interface IOperation<out TOperator, TExpression>
    {
        /// <summary>
        /// Gets the associativity.
        /// </summary>
        Associativity Associativity { get; }

        /// <summary>
        /// Unfolds the expression.
        /// </summary>
        /// <param name="list">The list.</param>
        void UnfoldExpression(List<object?> list);
    }
}
