namespace Warpstone.V2.Parsers;

public sealed class ChoiceParser<T> : ParserBase<T>
{
    private readonly Lazy<IParser<T>> first;
    private readonly Lazy<IParser<T>> second;

    public ChoiceParser(Func<IParser<T>> first, Func<IParser<T>> second)
    {
        this.first = new(first);
        this.second = new(second);
    }

    public IParser<T> First => first.Value;

    public IParser<T> Second => second.Value;

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

    private void Step2(IActiveParseContext context, int position)
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