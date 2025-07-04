namespace Warpstone
{
    /// <summary>
    /// Interface for inputs used by parsers.
    /// </summary>
    public interface IParseInput
    {
        /// <summary>
        /// The string content of the input.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// The source of the input.
        /// </summary>
        public IParseInputSource Source { get; }

        /// <summary>
        /// Gets line and column position of the given <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The found line number (starting at line number 1) and column for the given index.</returns>
        public ParseInputPosition GetPosition(int index);
    }
}
