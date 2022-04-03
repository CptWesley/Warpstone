namespace Warpstone
{
    /// <summary>
    /// Interface for parse errors.
    /// </summary>
    public interface IParseError
    {
        /// <summary>
        /// Gets the position.
        /// </summary>
        SourcePosition Position { get; }

        /// <summary>
        /// Gets a value indicating whether or not backtracking is allowed from this error.
        /// </summary>
        bool AllowBacktracking { get; }

        /// <summary>
        /// Turns the error into a readable message.
        /// </summary>
        /// <returns>A string representing the error.</returns>
        string GetMessage();

        /// <summary>
        /// Creates a variant of the error that doesn't allow backtracking.
        /// </summary>
        /// <returns>The new parse error that doesn't allow backtracking.</returns>
        IParseError DisallowBacktracking();
    }
}
