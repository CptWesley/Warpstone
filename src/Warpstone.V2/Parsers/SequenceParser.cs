namespace Warpstone.V2.Parsers;

public sealed class SequenceParser<TFirst, TSecond> : ParserBase<(TFirst, TSecond)>
{
    private readonly Lazy<IParser<TFirst>> first;
    private readonly Lazy<IParser<TSecond>> second;

    public SequenceParser(Func<IParser<TFirst>> first, Func<IParser<TSecond>> second)
    {
        this.first = new(first);
        this.second = new(second);
    }

    public IParser<TFirst> First => first.Value;

    public IParser<TSecond> Second => second.Value;

    public override IParseResult<(TFirst, TSecond)> Eval(IParseInput input, int position, Func<IParser, int, IParseResult> eval)
    {
        if (eval(First, position) is not IParseResult<TFirst> first)
        {
            throw new InvalidOperationException();
        }

        if (first.Status != ParseStatus.Match)
        {
            return this.Mismatch(position, first.Errors);
        }

        if (eval(Second, first.NextPosition) is not IParseResult<TSecond> second)
        {
            throw new InvalidOperationException();
        }

        if (second.Status != ParseStatus.Match)
        {
            return this.Mismatch(first.Position, second.Errors);
        }

        var value = (first.Value, second.Value);
        var length = first.Length + second.Length;
        return this.Match(position, length, value);
    }

    public override void Step(IActiveParseContext context, int position, int phase)
    {
        switch (phase)
        {
            case 0:
                Step0(context, position);
                break;
            case 1:
                Step1(context, position);
                break;
            case 2:
                Step2(context, position);
                break;
            default:
                throw new InvalidOperationException();
        }
    }

    private void Step0(IActiveParseContext context, int position)
    {
        context.Push(this, position, 1);
        context.Push(First, position);
    }

    private void Step1(IActiveParseContext context, int position)
    {
        if (context.MemoTable[position, First] is not { } firstResult)
        {
            throw new InvalidOperationException();
        }

        if (firstResult.Status == ParseStatus.Match)
        {
            context.Push(this, position, 2);
            context.Push(Second, firstResult.NextPosition);
        }
        else
        {
            context.MemoTable[position, this] = this.Mismatch(position, firstResult.Errors);
        }
    }

    private void Step2(IActiveParseContext context, int position)
    {
        if (context.MemoTable[position, First] is not IParseResult<TFirst> firstSuccess || firstSuccess.Status != ParseStatus.Match)
        {
            throw new InvalidOperationException();
        }

        if (context.MemoTable[firstSuccess.NextPosition, Second] is not IParseResult<TSecond> secondResult)
        {
            throw new InvalidOperationException();
        }

        if (secondResult.Status == ParseStatus.Match)
        {
            var value = (firstSuccess.Value, secondResult.Value);
            var length = firstSuccess.Length + secondResult.Length;
            context.MemoTable[position, this] = this.Match(position, length, value);
        }
        else
        {
            context.MemoTable[position, this] = this.Mismatch(position, secondResult.Errors);
        }
    }

    protected override string InternalToString(int depth)
        => $"({First.ToString(depth - 1)} {Second.ToString(depth - 1)})";
}
