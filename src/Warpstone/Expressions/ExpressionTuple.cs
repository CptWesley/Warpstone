using System.Collections.Generic;

namespace Warpstone.Expressions
{
    /// <summary>
    /// Represents a parsed expression tuple and its operators.
    /// </summary>
    /// <typeparam name="TOperator">The type of the operator.</typeparam>
    /// <typeparam name="TExpression">The type of the expression.</typeparam>
    public class ExpressionTuple<TOperator, TExpression>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTuple{TOperator, TExpression}"/> class.
        /// </summary>
        /// <param name="preOperators">The pre operators.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="postOperators">The post operators.</param>
        public ExpressionTuple(IEnumerable<OperatorTuple> preOperators, TExpression expression, IEnumerable<OperatorTuple> postOperators)
            => (PreOperators, Expression, PostOperators) = (preOperators, expression, postOperators);

        /// <summary>
        /// Gets the post operators.
        /// </summary>
        public IEnumerable<OperatorTuple> PreOperators { get; }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        public TExpression Expression { get; }

        /// <summary>
        /// Gets the post operators.
        /// </summary>
        public IEnumerable<OperatorTuple> PostOperators { get; }
    }
}
