using System.Diagnostics.CodeAnalysis;

namespace Warpstone
{
    /// <summary>
    /// Object representing the parsing result.
    /// </summary>
    /// <typeparam name="T">The output type of the parse process.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1716", Justification = "Deemed easier to understand for users.")]
    public interface IParseResult<out T>
    {
        /// <summary>
        /// Gets a value indicating whether the parsing was success.
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// Gets the parsed value.
        /// </summary>
        T Value { get; }

        /// <summary>
        /// Gets the start position of the parser.
        /// </summary>
        int StartPosition { get; }

        /// <summary>
        /// Gets the position of the parser.
        /// </summary>
        int Position { get; }

        /// <summary>
        /// Gets the parse error.
        /// </summary>
        IParseError Error { get; }
    }
}
