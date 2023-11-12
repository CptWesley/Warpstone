namespace Warpstone.Parsers;

/// <summary>
/// Parser which parses a regular expression.
/// </summary>
public sealed class RegexParser : ParserBase<string>, IEquatable<RegexParser>, IParserValue<string>
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
        Options = options;
        expected = Regex.Escape(pattern);
        regex = new Regex(@$"\G({pattern})", options);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegexParser"/> class.
    /// </summary>
    /// <param name="pattern">The pattern to be matched.</param>
    public RegexParser([StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
        : this(pattern, RegexOptions.Compiled | RegexOptions.ExplicitCapture)
    {
    }

    /// <summary>
    /// The expected pattern.
    /// </summary>
    public string Pattern { get; }

    /// <inheritdoc />
    string IParserValue<string>.Value => Pattern;

    /// <summary>
    /// Gets the string comparison method.
    /// </summary>
    public RegexOptions Options { get; }

    /// <inheritdoc />
    public override IterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IterativeStep> eval)
    {
        var input = context.Input.Content;

        if (position >= input.Length)
        {
            return Iterative.Done(this.Mismatch(context, position, 1, expected));
        }

        var match = regex.Match(input, position);

        if (match.Success && match.Index == position)
        {
            return Iterative.Done(this.Match(context, position, 1, match.Value));
        }
        else
        {
            return Iterative.Done(this.Mismatch(context, position, 1, expected));
        }
    }

    /// <inheritdoc />
    protected override string InternalToString(int depth)
        => expected;

    /// <inheritdoc />
    public override bool Equals(object obj)
        => obj is RegexParser other
        && Equals(other);

    /// <inheritdoc />
    public bool Equals(RegexParser other)
        => other is not null
        && Pattern == other.Pattern
        && Options == other.Options;

    /// <inheritdoc />
    public override int GetHashCode()
        => (typeof(RegexParser), Pattern, Options).GetHashCode();
}
