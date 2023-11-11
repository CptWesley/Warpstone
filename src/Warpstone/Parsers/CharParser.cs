namespace Warpstone.Parsers;

/// <summary>
/// Parser which parses a single character.
/// </summary>
public sealed class CharParser : ParserBase<char>
{
    private readonly string expected;

    /// <summary>
    /// Initializes a new instance of the <see cref="CharParser"/> class.
    /// </summary>
    /// <param name="value">The expected character.</param>
    public CharParser(char value)
    {
        Value = value;
        expected = $"'{value}'";
    }

    /// <summary>
    /// The expected character.
    /// </summary>
    public char Value { get; }

    /// <inheritdoc />
    public override IterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IterativeStep> eval)
    {
        var input = context.Input.Content;

        if (position >= input.Length)
        {
            return Iterative.Done(this.Mismatch(context, position, new UnexpectedTokenError(context, this, position, 1, expected)));
        }

        if (input[position] == Value)
        {
            return Iterative.Done(this.Match(context, position, 1, Value));
        }
        else
        {
            return Iterative.Done(this.Mismatch(context, position, new UnexpectedTokenError(context, this, position, 1, expected)));
        }
    }

    /// <inheritdoc />
    protected override string InternalToString(int depth)
        => expected;
}
