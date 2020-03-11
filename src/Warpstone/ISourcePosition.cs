namespace Warpstone
{
    /// <summary>
    /// Class representing the start position and length of a parsed object in the source.
    /// </summary>
    public interface ISourcePosition
    {
        /// <summary>
        /// Gets or sets the start position.
        /// </summary>
        int Start { get; set; }

        /// <summary>
        /// Gets or sets the length of the parsed part in the source.
        /// </summary>
        int Length { get; set; }
    }
}
