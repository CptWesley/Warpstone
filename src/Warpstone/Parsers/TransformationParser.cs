namespace Warpstone.Parsers;

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
                Debug.Assert(untypedInner is IParseResult<TIn>);
                var inner = (IParseResult<TIn>)untypedInner!;

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

    protected override string InternalToString(int depth)
        => $"Transform({First.ToString(depth - 1)})";
}
