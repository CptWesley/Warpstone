namespace Warpstone.Expressions
{
    /// <summary>
    /// Represents a tuple of an operator and its used parser.
    /// </summary>
    /// <typeparam name="T">The type of operator.</typeparam>
    public class OperatorTuple<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorTuple{T}"/> class.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="operator">The operator.</param>
        public OperatorTuple(Parser<T> parser, T @operator)
            => (Parser, Operator) = (parser, @operator);

        /// <summary>
        /// Gets the parser.
        /// </summary>
        public Parser<T> Parser { get; }

        /// <summary>
        /// Gets the operator.
        /// </summary>
        public T Operator { get; }
    }
}
