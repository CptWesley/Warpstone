namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Parser used for detecting the end of the input stream.
/// </summary>
internal sealed class EndParser : ParserBase<string>
{
    /// <summary>
    /// Singleton instance of the parser.
    /// </summary>
    public static readonly EndParser Instance = new();

    private EndParser()
    {
    }
}
