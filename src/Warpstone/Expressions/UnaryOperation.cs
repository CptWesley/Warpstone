using System.Collections.Generic;

namespace Warpstone.Expressions
{
    /// <summary>
    /// A pattern for matching expressions with one sub-expression and a single operator.
    /// </summary>
    /// <typeparam name="TOperator">The type of the operator.</typeparam>
    /// <typeparam name="TExpression">The type of the expression.</typeparam>
    /// <seealso cref="IOperation{TOperator, TExpression}" />
    public class UnaryOperation<TOperator, TExpression> : Operation<TOperator, TExpression>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnaryOperation{TOperator, TExpression}"/> class.
        /// </summary>
        /// <param name="associativity">The associativity.</param>
        /// <param name="transformations">The transformations.</param>
        public UnaryOperation(Associativity associativity, Dictionary<IParser<TOperator>, UnaryOperatorTransform<TOperator, TExpression>> transformations)
            : base(associativity)
            => Transformations = transformations;

        /// <summary>
        /// Gets the transformation.
        /// </summary>
        public Dictionary<IParser<TOperator>, UnaryOperatorTransform<TOperator, TExpression>> Transformations { get; }

        /// <inheritdoc/>
        internal override void UnfoldExpressionLeft(List<object> list)
        {
            int index = 0;
            while (index < list.Count)
            {
                if (IsOperator(list[index]) && Transformations.TryGetValue(GetParser(list[index]), out UnaryOperatorTransform<TOperator, TExpression> transformation))
                {
                    ReplaceExpression(list, index, transformation(GetOperator(list[index]), (TExpression)list[index - 1]), true);
                }
                else
                {
                    index++;
                }
            }
        }

        /// <inheritdoc/>
        internal override void UnfoldExpressionRight(List<object> list)
        {
            int index = list.Count - 2;
            while (index >= 0)
            {
                if (IsOperator(list[index]) && Transformations.TryGetValue(GetParser(list[index]), out UnaryOperatorTransform<TOperator, TExpression> transformation))
                {
                    ReplaceExpression(list, index, transformation(GetOperator(list[index]), (TExpression)list[index + 1]), false);
                }

                index--;
            }
        }

        private static void ReplaceExpression(List<object> list, int index, object value, bool left)
        {
            list.RemoveAt(index);
            list.Insert(index, value);
            list.RemoveAt(index + (left ? -1 : 1));
        }
    }
}
