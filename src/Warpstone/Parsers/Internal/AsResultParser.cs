namespace Warpstone.Parsers.Internal;

/// <summary>
/// A parser which wraps a parser and returns the internal parse result.
/// </summary>
/// <typeparam name="T">The type of the inner parser.</typeparam>
internal sealed class AsResultParser<T> : ParserBase<IParseResult<T>>, IParserFirst<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsResultParser{T}"/> class.
    /// </summary>
    /// <param name="first">The inner parser that is being wrapped.</param>
    public AsResultParser(IParser<T> first)
    {
        First = first;
    }

    /// <summary>
    /// The inner parser.
    /// </summary>
    public IParser<T> First { get; }

    /// <inheritdoc />
    public override IIterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IIterativeStep> eval)
        => Iterative.More(
            () => eval(First, position),
            untypedFirst =>
            {
                var first = untypedFirst.AssertOfType<IParseResult<T>>();
                return Iterative.Done(this.Match(context, position, first.Length, first));
            });

    /// <inheritdoc />
    protected override string InternalToString(int depth)
        => $"AsResult({First})";
}
