namespace Warpstone.Parsers;

/// <summary>
/// Parser which parses a constant string.
/// </summary>
public sealed class StringParser : ParserBase<string>, IEquatable<StringParser>
{
    private readonly string expected;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringParser"/> class.
    /// </summary>
    /// <param name="value">The expected character.</param>
    /// <param name="stringComparison">The string comparison method to use.</param>
    public StringParser(string value, StringComparison stringComparison)
    {
        Value = value;
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
    public string Value { get; }

    /// <summary>
    /// Gets the string comparison method.
    /// </summary>
    public StringComparison StringComparison { get; }

    /// <inheritdoc />
    public override IterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IterativeStep> eval)
    {
        var input = context.Input.Content;

        if (position >= input.Length)
        {
            return Iterative.Done(this.Mismatch(context, position, new UnexpectedTokenError(context, this, position, 1, expected)));
        }

        if (StringAtIndex(input, position))
        {
            return Iterative.Done(this.Match(context, position, 1, Value));
        }
        else
        {
            return Iterative.Done(this.Mismatch(context, position, new UnexpectedTokenError(context, this, position, 1, expected)));
        }
    }

    private bool StringAtIndex(string input, int position)
    {
        if (position + Value.Length > input.Length)
        {
            return false;
        }

        return Value.Equals(input.Substring(position, Value.Length), StringComparison);
    }

    /// <inheritdoc />
    protected override string InternalToString(int depth)
        => expected;

    /// <inheritdoc />
    public override bool Equals(object obj)
        => obj is StringParser other
        && Equals(other);

    /// <inheritdoc />
    public bool Equals(StringParser other)
        => other is not null
        && Value == other.Value
        && Equals(StringComparison, other.StringComparison);

    /// <inheritdoc />
    public override int GetHashCode()
        => (typeof(StringParser), Value, StringComparison).GetHashCode();
}
