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
    public delegate TExpression OperatorTransform<TOperator, TExpression>(TOperator @operator, TExpression l, TExpression r);

    /// <summary>
    /// A pattern for matching expressions with two sub-expressions and a single operator.
    /// </summary>
    /// <typeparam name="TOperator">The expression type.</typeparam>
    /// <typeparam name="TExpression">The type of the operator.</typeparam>
    public class Operation<TOperator, TExpression>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Operation{TExpression, TOperator}"/> class.
        /// </summary>
        /// <param name="associativity">The associativity.</param>
        /// <param name="transformations">The possible performed transformations.</param>
        public Operation(Associativity associativity, Dictionary<Parser<TOperator>, OperatorTransform<TOperator, TExpression>> transformations)
            => (Associativity, Transformations) = (associativity, transformations);

        /// <summary>
        /// Gets the associativity.
        /// </summary>
        public Associativity Associativity { get; }

        /// <summary>
        /// Gets the transformation.
        /// </summary>
        public Dictionary<Parser<TOperator>, OperatorTransform<TOperator, TExpression>> Transformations { get; }
    }
}
