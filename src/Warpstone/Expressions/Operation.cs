using System;
using System.Collections.Generic;

namespace Warpstone.Expressions
{
    /// <summary>
    /// A pattern for matching expressions.
    /// </summary>
    /// <typeparam name="TExpression">The expression type.</typeparam>
    public class Operation<TExpression>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Operation{TOutput}"/> class.
        /// </summary>
        /// <param name="associativity">The associativity.</param>
        public Operation(Associativity associativity)
            => Associativity = associativity;

        /// <summary>
        /// Gets the associativity of the expression.
        /// </summary>
        public Associativity Associativity { get; }
    }

    /// <summary>
    /// A pattern for matching expressions with two sub-expressions and a single operator.
    /// </summary>
    /// <typeparam name="TExpression">The expression type.</typeparam>
    /// <typeparam name="TOperator">The type of the operator.</typeparam>
    public class Operation<TExpression, TOperator> : Operation<TExpression>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Operation{TExpression, TOperator}"/> class.
        /// </summary>
        /// <param name="associativity">The associativity.</param>
        /// <param name="transformations">The possible performed transformations.</param>
        public Operation(Associativity associativity, Dictionary<Parser<TOperator>, Func<TExpression, TExpression, TExpression>> transformations)
            : base(associativity)
            => Transformations = transformations;

        /// <summary>
        /// Gets the transformation.
        /// </summary>
        public Dictionary<Parser<TOperator>, Func<TExpression, TExpression, TExpression>> Transformations { get; }
    }
}
