using Warpstone.V2.Errors;

namespace Warpstone.V2.Parsers;

public sealed class TransformationParser<TIn, TOut> : ParserBase<TOut>
{
    private readonly Lazy<IParser<TIn>> first;

    public TransformationParser(Func<IParser<TIn>> first, Func<TIn, TOut> transformation)
    {
        this.first = new(first);
        Transformation = transformation;
    }

    public IParser<TIn> First => first.Value;

    public Func<TIn, TOut> Transformation { get; }

    public override IterativeStep Eval(IParseInput input, int position, Func<IParser, int, IterativeStep> eval)
        => Iterative.More(
            () => eval(First, position),
            untypedInner =>
            {
                if (untypedInner is not IParseResult<TIn> inner)
                {
                    throw new InvalidOperationException();
                }

                if (inner.Status != ParseStatus.Match)
                {
                    return Iterative.Done(this.Mismatch(position, inner.Errors));
                }

                try
                {
                    var transformed = Transformation(inner.Value);
                    return Iterative.Done(this.Match(position, inner.Length, transformed));
                }
                catch (Exception e)
                {
                    var error = new TransformationError(input, this, position, 0, e.Message, e);
                    return Iterative.Done(this.Mismatch(position, error));
                }
            });

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

        if (inner.Status != ParseStatus.Match)
        {
            context.MemoTable[position, this] = this.Mismatch(position, inner.Errors);
            return;
        }

        try
        {
            var transformed = Transformation(inner.Value);
            context.MemoTable[position, this] = this.Match(position, inner.Length, transformed);
        }
        catch (Exception e)
        {
            var error = new TransformationError(context.Input, this, position, 0, e.Message, e);
            context.MemoTable[position, this] = this.Mismatch(position, error);
        }
    }

    protected override string InternalToString(int depth)
        => $"Transform({First.ToString(depth - 1)})";
}
