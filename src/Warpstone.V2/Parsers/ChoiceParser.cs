namespace Warpstone.V2.Parsers;

public sealed record ChoiceParser<T> : IParser<T>
{
    public ChoiceParser(IParser<T> first, IParser<T> second)
    {
        First = first;
        Second = second;
    }

    public IParser<T> First { get; }

    public IParser<T> Second { get; }

    public void Step(IActiveParsingContext context, int position, int phase)
    {
        if (context.MemoTable[position, this] is { })
        {
            return;
        }

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

    private void Step0(IActiveParsingContext context, int position)
    {
        context.Push(this, position, 1);
        context.Push(First, position);
    }

    private void Step1(IActiveParsingContext context, int position)
    {
        if (context.MemoTable[position, First] is not IParseResult<T> firstResult)
        {
            throw new InvalidOperationException();
        }

        if (firstResult.Success)
        {
            context.MemoTable[position, this] = this.Succeed(position, firstResult.Length, firstResult.Value);
        }
        else
        {
            context.Push(this, position, 2);
            context.Push(Second, position);
        }
    }

    private void Step2(IActiveParsingContext context, int position)
    {
        if (context.MemoTable[position, First] is not IParseResult<T> firstResult || firstResult.Success)
        {
            throw new InvalidOperationException();
        }

        if (context.MemoTable[position, Second] is not IParseResult<T> secondResult)
        {
            throw new InvalidOperationException();
        }

        if (secondResult.Success)
        {
            context.MemoTable[position, this] = this.Succeed(position, secondResult.Length, secondResult.Value);
        }
        else
        {
            var errors = firstResult.Errors.Concat(secondResult.Errors);
            context.MemoTable[position, this] = this.Fail(position, errors);
        }
    }
}