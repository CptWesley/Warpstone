using System.Collections.Generic;

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
        /// <param name="parser">The parser that produced the result.</param>
        /// <param name="startPosition">The start position.</param>
        /// <param name="position">The position.</param>
        /// <param name="error">The parse error that occured.</param>
        /// <param name="innerResults">The inner results that lead to this result.</param>
        public ParseResult(IParser<T> parser, int startPosition, int position, IParseError? error, IEnumerable<IParseResult> innerResults)
        {
            StartPosition = startPosition;
            Position = position;
            Error = error;
            Success = false;
            InnerResults = innerResults;
            Parser = parser;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseResult{T}"/> class.
        /// </summary>
        /// <param name="parser">The parser that produced the result.</param>
        /// <param name="value">The value.</param>
        /// <param name="startPosition">The start position of the parser.</param>
        /// <param name="position">The position of the parser.</param>
        /// <param name="innerResults">The inner results that lead to this result.</param>
        public ParseResult(IParser<T> parser, T? value, int startPosition, int position, IEnumerable<IParseResult> innerResults)
        {
            Value = value;
            StartPosition = startPosition;
            Position = position;
            Success = true;
            InnerResults = innerResults;
            Parser = parser;
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

        /// <inheritdoc/>
        public IEnumerable<IParseResult> InnerResults { get; }

        /// <inheritdoc/>
        public IParser<T> Parser { get; }

        /// <inheritdoc/>
        object? IParseResult.Value => Value;

        /// <inheritdoc/>
        IParser IParseResult.Parser => Parser;
    }
}
