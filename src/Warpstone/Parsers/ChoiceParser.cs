namespace Warpstone.Parsers;

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

    public override IterativeStep Eval(IParseInput input, int position, Func<IParser, int, IterativeStep> eval)
        => Iterative.More(
            () => eval(First, position),
            untypedFirst =>
            {
                Debug.Assert(untypedFirst is IParseResult<T>);
                var first = (IParseResult<T>)untypedFirst!;

                if (first.Status == ParseStatus.Match)
                {
                    return Iterative.Done(first);
                }

                return Iterative.More(
                    () => eval(Second, position),
                    untypedSecond =>
                    {
                        Debug.Assert(untypedSecond is IParseResult<T>);
                        var second = (IParseResult<T>)untypedSecond!;

                        if (second.Status == ParseStatus.Match)
                        {
                            return Iterative.Done(second);
                        }

                        var errors = first.Errors.Concat(second.Errors);
                        return Iterative.Done(this.Mismatch(position, errors));
                    });
            });

    protected override string InternalToString(int depth)
        => $"({First.ToString(depth - 1)} | {Second.ToString(depth - 1)})";
}
