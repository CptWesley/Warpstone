namespace Legacy.Warpstone.Expressions
{
    /// <summary>
    /// Represents a tuple of an operator and its used parser.
    /// </summary>
    public class OperatorTuple
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorTuple"/> class.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="operator">The operator.</param>
        public OperatorTuple(IParser<object> parser, object @operator)
            => (Parser, Operator) = (parser, @operator);

        /// <summary>
        /// Gets the parser.
        /// </summary>
        public IParser<object> Parser { get; }

        /// <summary>
        /// Gets the operator.
        /// </summary>
        public object Operator { get; }
    }
}
