namespace Warpstone.Internal.ParserExpressions;

/// <summary>
/// Represents a parser that matches regular expressions in the input.
/// </summary>
internal sealed class RegexParser : IParser<string>
{
    private readonly string expected;
    private readonly Regex regex;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegexParser"/> class.
    /// </summary>
    /// <param name="pattern">The pattern to be matched.</param>
    /// <param name="options">The options used by the regex engine.</param>
    public RegexParser([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, RegexOptions options)
    {
        Pattern = pattern;
        Options = options | RegexOptions.ExplicitCapture;
        expected = Regex.Escape(pattern);
        regex = new Regex(@$"\G({pattern})", options);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegexParser"/> class.
    /// </summary>
    /// <param name="pattern">The pattern to be matched.</param>
    public RegexParser([StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
        : this(pattern, RegexOptions.Compiled)
    {
    }

    /// <summary>
    /// The expected pattern.
    /// </summary>
    public string Pattern { get; }

    /// <summary>
    /// Gets the string comparison method.
    /// </summary>
    public RegexOptions Options { get; }

    /// <inheritdoc />
    public Type ResultType => typeof(string);
}
