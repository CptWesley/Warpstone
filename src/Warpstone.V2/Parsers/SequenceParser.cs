using System.Diagnostics;

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

    public override IterativeStep Eval(IParseInput input, int position, Func<IParser, int, IterativeStep> eval)
        => Iterative.More(
            () => eval(First, position),
            untypedFirst =>
            {
                Debug.Assert(untypedFirst is IParseResult<TFirst>);
                var first = (IParseResult<TFirst>)untypedFirst;

                if (first.Status != ParseStatus.Match)
                {
                    return Iterative.Done(this.Mismatch(position, first.Errors));
                }

                return Iterative.More(
                    () => eval(Second, first.NextPosition),
                    untypedSecond =>
                    {
                        Debug.Assert(untypedSecond is IParseResult<TSecond>);
                        var second = (IParseResult<TSecond>)untypedSecond;

                        if (second.Status != ParseStatus.Match)
                        {
                            return Iterative.Done(this.Mismatch(first.Position, second.Errors));
                        }

                        var value = (first.Value, second.Value);
                        var length = first.Length + second.Length;
                        return Iterative.Done(this.Match(position, length, value));
                    });
            });

    protected override string InternalToString(int depth)
        => $"({First.ToString(depth - 1)} {Second.ToString(depth - 1)})";
}
