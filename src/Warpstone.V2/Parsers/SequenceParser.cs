namespace Warpstone.V2.Parsers;

public sealed class SequenceParser<TFirst, TSecond> : IParser<(TFirst, TSecond)>
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

    public void Step(IActiveParseContext context, int position, int phase)
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

        if (firstResult.Success)
        {
            context.Push(this, position, 2);
            context.Push(Second, firstResult.NextPosition);
        }
        else
        {
            context.MemoTable[position, this] = this.Fail(position, firstResult.Errors);
        }
    }

    private void Step2(IActiveParseContext context, int position)
    {
        if (context.MemoTable[position, First] is not IParseResult<TFirst> firstSuccess || !firstSuccess.Success)
        {
            throw new InvalidOperationException();
        }

        if (context.MemoTable[firstSuccess.NextPosition, Second] is not IParseResult<TSecond> secondResult)
        {
            throw new InvalidOperationException();
        }

        if (secondResult.Success)
        {
            var value = (firstSuccess.Value, secondResult.Value);
            var length = firstSuccess.Length + secondResult.Length;
            context.MemoTable[position, this] = this.Succeed(position, length, value);
        }
        else
        {
            context.MemoTable[position, this] = this.Fail(position, secondResult.Errors);
        }
    }
}
