namespace Warpstone.Parsers;

/// <summary>
/// A parser which runs two parsers in sequence where both parsers should succeed
/// in order for this parser to succeed.
/// </summary>
/// <typeparam name="TFirst">The result type of the first parser.</typeparam>
/// <typeparam name="TSecond">The result type of the second parser.</typeparam>
public sealed class SequenceParser<TFirst, TSecond> : ParserBase<(TFirst, TSecond)>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SequenceParser{TFirst, TSecond}"/> class.
    /// </summary>
    /// <param name="first">The parser which is attempted first.</param>
    /// <param name="second">The parser which is attempted second.</param>
    public SequenceParser(IParser<TFirst> first, IParser<TSecond> second)
    {
        First = first;
        Second = second;
    }

    /// <summary>
    /// The parser which is attempted first.
    /// </summary>
    public IParser<TFirst> First { get; }

    /// <summary>
    /// The parser which is attempted second.
    /// </summary>
    public IParser<TSecond> Second { get; }

    /// <inheritdoc />
    public override IterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IterativeStep> eval)
        => Iterative.More(
            () => eval(First, position),
            untypedFirst =>
            {
                var first = untypedFirst.AssertOfType<IParseResult<TFirst>>();

                if (first.Status != ParseStatus.Match)
                {
                    return Iterative.Done(this.Mismatch(context, position, first.Errors));
                }

                return Iterative.More(
                    () => eval(Second, first.NextPosition),
                    untypedSecond =>
                    {
                        var second = untypedSecond.AssertOfType<IParseResult<TSecond>>();

                        if (second.Status != ParseStatus.Match)
                        {
                            return Iterative.Done(this.Mismatch(context, first.Position, second.Errors));
                        }

                        var value = (first.Value, second.Value);
                        var length = first.Length + second.Length;
                        return Iterative.Done(this.Match(context, position, length, value));
                    });
            });

    /// <inheritdoc />
    protected override string InternalToString(int depth)
        => $"({First.ToString(depth - 1)} {Second.ToString(depth - 1)})";
}
