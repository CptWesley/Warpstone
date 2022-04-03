namespace Warpstone
{
    /// <summary>
    /// Represents parse errors.
    /// </summary>
    /// <seealso cref="IParseError" />
    public abstract class ParseError : IParseError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseError"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="allowBacktracking">Whether or not backtracking is allowed from this error.</param>
        public ParseError(SourcePosition position, bool allowBacktracking)
            => (Position, AllowBacktracking) = (position, allowBacktracking);

        /// <inheritdoc/>
        public SourcePosition Position { get; }

        /// <inheritdoc/>
        public bool AllowBacktracking { get; }

        /// <inheritdoc/>
        public abstract IParseError DisallowBacktracking();

        /// <inheritdoc/>
        public string GetMessage()
            => $"{GetSimpleMessage()} At {Position}.";

        /// <summary>
        /// Gets the simple message without positional information.
        /// </summary>
        /// <returns>The message.</returns>
        protected abstract string GetSimpleMessage();
    }
}
