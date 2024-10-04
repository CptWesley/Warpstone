namespace Warpstone.Parsers.Internal;

/// <summary>
/// Parser which parses a constant string.
/// </summary>
internal sealed class StringParser : ParserBase<string>, IEquatable<StringParser>, IParserValue<string>
{
    private readonly string expected;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringParser"/> class.
    /// </summary>
    /// <param name="value">The expected character.</param>
    /// <param name="stringComparison">The string comparison method to use.</param>
    public StringParser(string value, StringComparison stringComparison)
    {
        String = value;
        StringComparison = stringComparison;
        expected = $"\"{value}\"";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringParser"/> class.
    /// </summary>
    /// <param name="value">The expected character.</param>
    public StringParser(string value)
        : this(value, StringComparison.Ordinal)
    {
    }

    /// <summary>
    /// The expected string.
    /// </summary>
    public string String { get; }

    /// <inheritdoc />
    string IParserValue<string>.Value => String;

    /// <summary>
    /// Gets the string comparison method.
    /// </summary>
    public StringComparison StringComparison { get; }

    /// <inheritdoc />
    public override IIterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IIterativeStep> eval)
    {
        var input = context.Input.Content;

        if (position >= input.Length)
        {
            return Iterative.Done(this.Mismatch(context, position, 1, expected));
        }

        if (StringAtIndex(input, position))
        {
            return Iterative.Done(this.Match(context, position, String.Length, String));
        }
        else
        {
            return Iterative.Done(this.Mismatch(context, position, 1, expected));
        }
    }

    private bool StringAtIndex(string input, int position)
    {
        if (position + String.Length > input.Length)
        {
            return false;
        }

        return String.Equals(input.Substring(position, String.Length), StringComparison);
    }

    /// <inheritdoc />
    protected override string InternalToString(int depth)
        => expected;

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is StringParser other
        && Equals(other);

    /// <inheritdoc />
    public bool Equals(StringParser? other)
        => other is not null
        && String == other.String
        && Equals(StringComparison, other.StringComparison);

    /// <inheritdoc />
    public override int GetHashCode()
        => (typeof(StringParser), String, StringComparison).GetHashCode();
}
