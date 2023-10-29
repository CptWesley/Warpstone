namespace Warpstone.V2.Parsers;

public sealed class CharParser : IParser<char>
{
    private readonly string stringValue;

    public CharParser(char value)
    {
        Value = value;
        stringValue = value.ToString();
    }

    public char Value { get; }

    public void Step(IActiveParsingContext context, int position, int phase)
    {
        if (context.Input.Input[position] == Value)
        {
            context.MemoTable[position, this] = this.Succeed(position, 1, Value);
        }
        else
        {
            context.MemoTable[position, this] = this.Fail(position, new UnexpectedTokenError(context.Input, this, position, 1, stringValue));
        }
    }
}
