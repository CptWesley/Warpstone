using Warpstone.V2.Errors;

namespace Warpstone.V2.Parsers;

public sealed class EndOfFileParser : ParserBase<string>
{
    public static readonly EndOfFileParser Instance = new EndOfFileParser();

    private EndOfFileParser()
    {
    }

    public override void Step(IActiveParseContext context, int position, int phase)
    {
        if (position >= context.Input.Input.Length)
        {
            context.MemoTable[position, this] = this.Succeed(position, 0, string.Empty);
        }
        else
        {
            context.MemoTable[position, this] = this.Fail(position, new UnexpectedTokenError(context.Input, this, position, 1, "EOF"));
        }
    }
}
