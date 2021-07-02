namespace Warpstone
{
    /// <summary>
    /// Object representing the parsing result.
    /// </summary>
    /// <typeparam name="T">The output type of the parse process.</typeparam>
    public class ParseResult<T> : IParseResult<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseResult{T}"/> class.
        /// </summary>
        /// <param name="startPosition">The start position.</param>
        /// <param name="position">The position.</param>
        /// <param name="error">The parse error that occured.</param>
        public ParseResult(int startPosition, int position, IParseError? error)
        {
            StartPosition = startPosition;
            Position = position;
            Error = error;
            Success = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseResult{T}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="startPosition">The start position of the parser.</param>
        /// <param name="position">The position of the parser.</param>
        public ParseResult(T? value, int startPosition, int position)
        {
            Value = value;
            StartPosition = startPosition;
            Position = position;
            Success = true;
        }

        /// <inheritdoc/>
        public bool Success { get; }

        /// <inheritdoc/>
        public T? Value { get; }

        /// <inheritdoc/>
        public int StartPosition { get; }

        /// <inheritdoc/>
        public int Position { get; }

        /// <inheritdoc/>
        public IParseError? Error { get; }
    }
}
