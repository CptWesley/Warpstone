namespace Legacy.Warpstone2;

/// <summary>
/// Indicates what type of <see cref="IParseResult"/> a result is.
/// </summary>
public enum ParseStatus
{
    /// <summary>
    /// Indicates that the parser failed due to issues with the parser definition.
    /// </summary>
    Fail = 0,

    /// <summary>
    /// Indicates that the parser successfully parsed the input.
    /// </summary>
    Match = 1,

    /// <summary>
    /// Indicates that the parser failed due it not being able to match the input.
    /// </summary>
    Mismatch = 2,
}
