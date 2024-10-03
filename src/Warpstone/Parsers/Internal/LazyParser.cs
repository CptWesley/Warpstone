namespace Warpstone.Parsers.Internal;

/// <summary>
/// Parser which lazily constructs the inner parser.
/// </summary>
/// <typeparam name="T">The type of the wrapped parser.</typeparam>
internal sealed class LazyParser<T> : ParserBase<T>
{
    private readonly Lazy<IParser<T>> lazyFirst;

    /// <summary>
    /// Initializes a new instance of the <see cref="LazyParser{T}"/> class.
    /// </summary>
    /// <param name="first">The inner parser that is being wrapped.</param>
    public LazyParser(Func<IParser<T>> first)
    {
        lazyFirst = new(first);
    }

    /// <inheritdoc />
    public override IterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IterativeStep> eval)
        => Iterative.More(
            () => eval(lazyFirst.Value, position),
            untypedFirst =>
            {
                var first = untypedFirst.AssertOfType<IParseResult<T>>();

                if (first.Success)
                {
                    return Iterative.Done(this.Match(context, position, first.Length, first.Value));
                }
                else
                {
                    return Iterative.Done(this.Mismatch(context, position, first.Errors));
                }
            });

    /// <inheritdoc />
    protected override string InternalToString(int depth)
        => $"Lazy({(lazyFirst.IsValueCreated ? lazyFirst.Value : "<not-evaluated>")})";
}
