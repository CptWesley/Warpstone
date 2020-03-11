using System.Collections.Generic;

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
        /// <param name="startPosition">The start position.</param>
        /// <param name="position">The position.</param>
        /// <param name="expected">The expected.</param>
        public ParseResult(int startPosition, int position, IEnumerable<string> expected)
        {
            StartPosition = startPosition;
            Position = position;
            Expected = expected;
            Success = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseResult{T}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="startPosition">The start position of the parser.</param>
        /// <param name="position">The position of the parser.</param>
        public ParseResult(T value, int startPosition, int position)
        {
            Value = value;
            StartPosition = startPosition;
            Position = position;
            Success = true;
        }

        /// <summary>
        /// Gets a value indicating whether the parsing was success.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Gets the parsed value.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Gets the start position of the parser.
        /// </summary>
        public int StartPosition { get; }

        /// <summary>
        /// Gets the position of the parser.
        /// </summary>
        public int Position { get; }

        /// <summary>
        /// Gets the possible expected characters.
        /// </summary>
        public IEnumerable<string> Expected { get; }
    }
}
