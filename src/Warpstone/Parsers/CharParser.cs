namespace Warpstone.Parsers;

public sealed class CharParser : ParserBase<char>
{
    private readonly string expected;

    public CharParser(char value)
    {
        Value = value;
        expected = $"'{value}'";
    }

    public char Value { get; }

    public override IterativeStep Eval(IParseInput input, int position, Func<IParser, int, IterativeStep> eval)
    {
        if (position >= input.Input.Length)
        {
            return Iterative.Done(this.Mismatch(position, new UnexpectedTokenError(input, this, position, 1, expected)));
        }

        if (input.Input[position] == Value)
        {
            return Iterative.Done(this.Match(position, 1, Value));
        }
        else
        {
            return Iterative.Done(this.Mismatch(position, new UnexpectedTokenError(input, this, position, 1, expected)));
        }
    }

    protected override string InternalToString(int depth)
        => expected;
}
