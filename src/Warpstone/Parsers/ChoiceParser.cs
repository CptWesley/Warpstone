namespace Warpstone.Parsers;

/// <summary>
/// Parser which attempts to parse the input in two different ways.
/// If the first parser fails, the second parser is used.
/// If the second parser also fails, this parser will fail.
/// </summary>
/// <typeparam name="T">The result type of the parsers.</typeparam>
public sealed class ChoiceParser<T> : ParserBase<T>, IParserSecond<T, T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChoiceParser{T}"/> class.
    /// </summary>
    /// <param name="first">The parser which is attempted first.</param>
    /// <param name="second">The parser which is attempted second.</param>
    public ChoiceParser(IParser<T> first, IParser<T> second)
    {
        First = first;
        Second = second;
    }

    /// <summary>
    /// The parser which is attempted first.
    /// </summary>
    public IParser<T> First { get; }

    /// <summary>
    /// The parser which is attempted second.
    /// </summary>
    public IParser<T> Second { get; }

    /// <inheritdoc />
    public override IterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IterativeStep> eval)
        => Iterative.More(
            () => eval(First, position),
            untypedFirst =>
            {
                var first = untypedFirst.AssertOfType<IParseResult<T>>();

                if (first.Status == ParseStatus.Match)
                {
                    return Iterative.Done(first);
                }

                return Iterative.More(
                    () => eval(Second, position),
                    untypedSecond =>
                    {
                        var second = untypedSecond.AssertOfType<IParseResult<T>>();

                        if (second.Status == ParseStatus.Match)
                        {
                            return Iterative.Done(second);
                        }

                        var errors = first.Errors.Concat(second.Errors);
                        return Iterative.Done(this.Mismatch(context, position, errors));
                    });
            });

    /// <inheritdoc />
    protected override string InternalToString(int depth)
        => $"({First.ToString(depth - 1)} | {Second.ToString(depth - 1)})";
}
