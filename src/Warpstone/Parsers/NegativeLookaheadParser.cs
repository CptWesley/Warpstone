namespace Warpstone.Parsers;

/// <summary>
/// Parser which succeeds if the inner parser fails, but fails if the inner parser succeeds.
/// </summary>
/// <typeparam name="T">The type of the wrapped parser.</typeparam>
public sealed class NegativeLookaheadParser<T> : ParserBase<T?>, IParserFirst<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NegativeLookaheadParser{T}"/> class.
    /// </summary>
    /// <param name="first">The inner parser that is being wrapped.</param>
    public NegativeLookaheadParser(IParser<T> first)
    {
        First = first;
    }

    /// <summary>
    /// The inner parser.
    /// </summary>
    public IParser<T> First { get; }

    /// <inheritdoc />
    public override IterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IterativeStep> eval)
        => Iterative.More(
            () => eval(First, position),
            untypedFirst =>
            {
                var first = untypedFirst.AssertOfType<IParseResult<T>>();

                if (first.Success)
                {
                    return Iterative.Done(this.Mismatch(context, position, new UnexpectedTokenError(context, this, position, 1, $"<not>")));
                }
                else
                {
                    return Iterative.Done(this.Match(context, position, first.Length, default));
                }
            });

    /// <inheritdoc />
    protected override string InternalToString(int depth)
        => $"Not({First.ToString(depth - 1)})";
}
