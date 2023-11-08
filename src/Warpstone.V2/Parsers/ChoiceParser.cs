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

    public override IterativeStep Eval(IParseInput input, int position, Func<IParser, int, IterativeStep> eval)
        => Iterative.More(
            () => eval(First, position),
            untypedFirst =>
            {
                if (untypedFirst is not IParseResult<T> first)
                {
                    throw new InvalidOperationException();
                }

                if (first.Status == ParseStatus.Match)
                {
                    return Iterative.Done(first);
                }

                return Iterative.More(
                    () => eval(Second, position),
                    untypedSecond =>
                    {
                        if (untypedSecond is not IParseResult<T> second)
                        {
                            throw new InvalidOperationException();
                        }

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