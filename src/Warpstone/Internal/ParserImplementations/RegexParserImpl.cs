namespace Warpstone.Internal.ParserImplementations;

/// <summary>
/// Represents a parser that matches regular expressions in the input.
/// </summary>
internal sealed class RegexParserImpl : IParserImplementation<string>
{
    private readonly string expected;
    private readonly Regex regex;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegexParserImpl"/> class.
    /// </summary>
    /// <param name="pattern">The pattern to be matched.</param>
    /// <param name="options">The options used by the regex engine.</param>
    public RegexParserImpl([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, RegexOptions options)
    {
        Pattern = pattern;
        Options = options | RegexOptions.ExplicitCapture;
        expected = Regex.Escape(pattern);
        regex = new Regex(@$"\G({pattern})", options);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegexParserImpl"/> class.
    /// </summary>
    /// <param name="pattern">The pattern to be matched.</param>
    public RegexParserImpl([StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
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
    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var input = context.Input.Content;
        var match = regex.Match(input, position);

        if (match.Success && match.Index == position)
        {
            return new(position, match.Length, match.Value);
        }
        else
        {
            return new(position, [new UnexpectedTokenError(context, this, position, 1, expected)]);
        }
    }

    /// <inheritdoc />
    public void Apply(IIterativeParseContext context, int position)
    {
        var input = context.Input.Content;
        var match = regex.Match(input, position);

        if (match.Success && match.Index == position)
        {
            context.ResultStack.Push(new(position, match.Length, match.Value));
        }
        else
        {
            context.ResultStack.Push(new(position, [new UnexpectedTokenError(context, this, position, 1, expected)]));
        }
    }

    /// <inheritdoc />
    public override string ToString()
        => $"RegexParser({expected}, {Options})";
}
