using Warpstone.V2.Errors;

namespace Warpstone.V2.Parsers;

public sealed class TransformationParser<TIn, TOut> : IParser<TOut>
{
    private readonly Lazy<IParser<TIn>> first;

    public TransformationParser(Func<IParser<TIn>> first, Func<TIn, TOut> transformation)
    {
        this.first = new(first);
        Transformation = transformation;
    }

    public IParser<TIn> First => first.Value;

    public Func<TIn, TOut> Transformation { get; }

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
        if (context.MemoTable[position, First] is not ParseResult<TIn> inner)
        {
            throw new InvalidOperationException();
        }

        if (!inner.Success)
        {
            context.MemoTable[position, this] = this.Fail(position, inner.Errors);
            return;
        }

        try
        {
            var transformed = Transformation(inner.Value);
            context.MemoTable[position, this] = this.Succeed(position, inner.Length, transformed);
        }
        catch (Exception e)
        {
            var error = new TransformationError(context.Input, this, position, 0, e.Message, e);
            context.MemoTable[position, this] = this.Fail(position, error);
        }
    }
}
