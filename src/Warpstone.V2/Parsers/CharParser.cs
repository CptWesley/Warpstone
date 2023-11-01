using Warpstone.V2.Errors;

namespace Warpstone.V2.Parsers;

public sealed class CharParser : ParserBase<char>
{
    private readonly string expected;

    public CharParser(char value)
    {
        Value = value;
        expected = $"'{value}'";
    }

    public char Value { get; }

    public override void Step(IActiveParseContext context, int position, int phase)
    {
        if (context.Input.Input[position] == Value)
        {
            context.MemoTable[position, this] = this.Match(position, 1, Value);
        }
        else
        {
            context.MemoTable[position, this] = this.Mismatch(position, new UnexpectedTokenError(context.Input, this, position, 1, expected));
        }
    }
}
