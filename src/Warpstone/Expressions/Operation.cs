using System.Collections.Generic;

namespace Warpstone.Expressions
{
    /// <summary>
    /// Base class for all expression patterns.
    /// </summary>
    /// <typeparam name="TOperator">The expression type.</typeparam>
    /// <typeparam name="TExpression">The type of the operator.</typeparam>
    public abstract class Operation<TOperator, TExpression> : IOperation<TOperator, TExpression>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Operation{TExpression, TOperator}"/> class.
        /// </summary>
        /// <param name="associativity">The associativity.</param>
        public Operation(Associativity associativity)
            => Associativity = associativity;

        /// <inheritdoc/>
        public Associativity Associativity { get; }

        /// <inheritdoc/>
        public void UnfoldExpression(List<object> list)
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

        /// <summary>
        /// Gets the parser from an object.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>The parser.</returns>
        internal static IParser<object> GetParser(object o)
            => ((OperatorTuple)o).Parser;

        /// <summary>
        /// Gets the operator from an object.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>The operator.</returns>
        internal static TOperator GetOperator(object o)
            => (TOperator)((OperatorTuple)o).Operator;

        /// <summary>
        /// Checks if an object is an operator.
        /// </summary>
        /// <param name="o">The object.</param>
        /// <returns><c>true</c> if the specified object is an operator; otherwise, <c>false</c>.</returns>
        internal static bool IsOperator(object o)
            => o is OperatorTuple;

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
    }
}
