using System.Collections.Generic;

namespace Warpstone.Expressions
{
    /// <summary>
    /// A pattern for matching expressions with two sub-expressions and a single operator.
    /// </summary>
    /// <typeparam name="TOperator">The type of the operator.</typeparam>
    /// <typeparam name="TExpression">The type of the expression.</typeparam>
    /// <seealso cref="IOperation{TOperator, TExpression}" />
    public class BinaryOperation<TOperator, TExpression> : Operation<TOperator, TExpression>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryOperation{TOperator, TExpression}"/> class.
        /// </summary>
        /// <param name="associativity">The associativity.</param>
        /// <param name="transformations">The transformations.</param>
        public BinaryOperation(Associativity associativity, Dictionary<IParser<TOperator>, BinaryOperatorTransform<TOperator, TExpression>> transformations)
            : base(associativity)
            => Transformations = transformations;

        /// <summary>
        /// Gets the transformation.
        /// </summary>
        public Dictionary<IParser<TOperator>, BinaryOperatorTransform<TOperator, TExpression>> Transformations { get; }

        /// <inheritdoc/>
        internal override void UnfoldExpressionLeft(List<object?> list)
        {
            int index = 0;
            while (index < list.Count)
            {
                if (IsOperator(list[index]) && Transformations.TryGetValue(GetParser(list[index]!), out BinaryOperatorTransform<TOperator, TExpression> transformation))
                {
                    ReplaceExpression(list, index, transformation(GetOperator(list[index]!), (TExpression)list[index - 1]!, (TExpression)list[index + 1]!));
                }
                else
                {
                    index++;
                }
            }
        }

        /// <inheritdoc/>
        internal override void UnfoldExpressionRight(List<object?> list)
        {
            int index = list.Count - 1;
            while (index > 0)
            {
                if (IsOperator(list[index]) && Transformations.TryGetValue(GetParser(list[index]!), out BinaryOperatorTransform<TOperator, TExpression> transformation))
                {
                    ReplaceExpression(list, index, transformation(GetOperator(list[index]!), (TExpression)list[index - 1]!, (TExpression)list[index + 1]!));
                }

                index--;
            }
        }

        private static void ReplaceExpression(List<object?> list, int index, TExpression value)
        {
            list.RemoveAt(index + 1);
            list.RemoveAt(index);
            list.Insert(index, value!);
            list.RemoveAt(index - 1);
        }
    }
}
