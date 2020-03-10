namespace Warpstone
{
    /// <summary>
    /// Object representing the parsing result.
    /// </summary>
    /// <typeparam name="T">The output type of the parse process.</typeparam>
    public class ParseResult<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseResult{T}"/> class.
        /// </summary>
        public ParseResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseResult{T}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="position">The position of the parser.</param>
        public ParseResult(T value, int position)
        {
            Value = value;
            Position = position;
        }

        /// <summary>
        /// Gets a value indicating whether the parsing was success.
        /// </summary>
        public bool Success => Value != null;

        /// <summary>
        /// Gets the parsed value.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Gets the position of the parser.
        /// </summary>
        public int Position { get; }
    }
}
