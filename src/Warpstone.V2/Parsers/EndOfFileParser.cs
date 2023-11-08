using Warpstone.V2.Errors;

namespace Warpstone.V2.Parsers;

public sealed class EndOfFileParser : ParserBase<string>
{
    public static readonly EndOfFileParser Instance = new EndOfFileParser();

    private EndOfFileParser()
    {
    }

    public override IterativeStep Eval(IParseInput input, int position, Func<IParser, int, IterativeStep> eval)
    {
        if (position >= input.Input.Length)
        {
            return Iterative.Done(this.Match(position, 0, string.Empty));
        }
        else
        {
            return Iterative.Done(this.Mismatch(position, new UnexpectedTokenError(input, this, position, 1, "EOF")));
        }
    }

    public override void Step(IActiveParseContext context, int position, int phase)
    {
        if (position >= context.Input.Input.Length)
        {
            context.MemoTable[position, this] = this.Match(position, 0, string.Empty);
        }
        else
        {
            context.MemoTable[position, this] = this.Mismatch(position, new UnexpectedTokenError(context.Input, this, position, 1, "EOF"));
        }
    }

    protected override string InternalToString(int depth)
        => "EOF";
}
