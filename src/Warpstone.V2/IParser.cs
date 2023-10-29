namespace Warpstone.V2;

public interface IParser
{
    void Step(IActiveParsingContext context, int position, int phase);
}

public interface IParser<T> : IParser
{

}

public sealed class CharParser : IParser<string>
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
            context.MemoTable[position, this] = this.Succeed(position, 1, stringValue);
        }
        else
        {
            context.MemoTable[position, this] = this.Fail(position, new UnexpectedTokenError(context.Input, this, position, 1, stringValue));
        }
    }
}

public sealed class SequenceParser<TFirst, TSecond> : IParser<(TFirst, TSecond)>
{
    public SequenceParser(IParser<TFirst> first, IParser<TSecond> second)
    {
        First = first;
        Second = second;
    }

    public IParser<TFirst> First { get; }

    public IParser<TSecond> Second { get; }

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

    private void Step2(IActiveParsingContext context, int position)
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