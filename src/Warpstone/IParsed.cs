namespace Warpstone
{
    /// <summary>
    /// Class representing the start position and length of a parsed object in the source.
    /// </summary>
    public interface IParsed
    {
        /// <summary>
        /// Gets or sets the source position of the parsed object.
        /// </summary>
        SourcePosition Position { get; set; }
    }
}
