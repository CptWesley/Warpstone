namespace Warpstone.Parsers.Internal;

/// <summary>
/// Represents a parser which performs a transformation on the result of an internal parser.
/// </summary>
/// <typeparam name="TIn">The result type of the inner parser.</typeparam>
/// <typeparam name="TOut">The result type of the transformation.</typeparam>
internal sealed class TransformationParser<TIn, TOut> : ParserBase<TOut>, IParserFirst<TIn>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TransformationParser{TIn, TOut}"/> class.
    /// </summary>
    /// <param name="first">The inner parser.</param>
    /// <param name="transformation">The transformation function.</param>
    public TransformationParser(IParser<TIn> first, Func<TIn, TOut> transformation)
    {
        First = first;
        Transformation = transformation;
    }

    /// <summary>
    /// Gets the inner parser.
    /// </summary>
    public IParser<TIn> First { get; }

    /// <summary>
    /// Gets the transformation function.
    /// </summary>
    public Func<TIn, TOut> Transformation { get; }

    /// <inheritdoc />
    public override IterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IterativeStep> eval)
        => Iterative.More(
            () => eval(First, position),
            untypedInner =>
            {
                var inner = untypedInner.AssertOfType<IParseResult<TIn>>();

                if (!inner.Success)
                {
                    return Iterative.Done(this.Mismatch(context, position, inner.Errors));
                }

                try
                {
                    var transformed = Transformation(inner.Value);
                    return Iterative.Done(this.Match(context, position, inner.Length, transformed));
                }
                catch (Exception e)
                {
                    var error = new TransformationError(context, this, position, 0, e.Message, e);
                    return Iterative.Done(this.Mismatch(context, position, error));
                }
            });

    /// <inheritdoc />
    protected override string InternalToString(int depth)
        => $"Transform({First.ToString(depth - 1)})";
}
