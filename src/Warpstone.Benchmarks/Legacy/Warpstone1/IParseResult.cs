using System.Collections.Generic;

namespace Legacy.Warpstone1
{
    /// <summary>
    /// Object representing the parsing result.
    /// </summary>
    /// <typeparam name="T">The output type of the parse process.</typeparam>
    public interface IParseResult<out T> : IParseResult
    {
        /// <summary>
        /// Gets a value indicating whether the parsing was success.
        /// </summary>
        new bool Success { get; }

        /// <summary>
        /// Gets the parsed value.
        /// </summary>
        new T? Value { get; }

        /// <summary>
        /// Gets the parser that produced this result.
        /// </summary>
        new IParser<T> Parser { get; }
    }

    /// <summary>
    /// Object representing the parsing result.
    /// </summary>
    public interface IParseResult : IBoundedToString
    {
        /// <summary>
        /// Gets a value indicating whether the parsing was success.
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// Gets the parsed value.
        /// </summary>
        object? Value { get; }

        /// <summary>
        /// Gets the position of the parse result.
        /// </summary>
        SourcePosition Position { get; }

        /// <summary>
        /// Gets the parse error.
        /// </summary>
        IParseError? Error { get; }

        /// <summary>
        /// Gets the parser that produced this result.
        /// </summary>
        IParser Parser { get; }

        /// <summary>
        /// Gets the inner results when.
        /// </summary>
        IEnumerable<IParseResult> InnerResults { get; }
    }
}
