namespace Warpstone.Parsers.Internal;

/// <summary>
/// A parser which runs a single parser multiple times with optional
/// delimiter and terminator parsers.
/// </summary>
/// <typeparam name="TElement">The parser for the elements.</typeparam>
/// <typeparam name="TDelimiter">The parser for the delimiter.</typeparam>
/// <typeparam name="TTerminator">The parser for the terminator.</typeparam>
internal sealed class MultipleParser<TElement, TDelimiter, TTerminator> :
    ParserBase<IImmutableList<TElement>>,
    IParserThird<TElement, TDelimiter, TTerminator>,
    IParserValue<(ulong Min, ulong Max)>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleParser{TElement, TDelimiter, TTerminator}"/> class.
    /// </summary>
    /// <param name="element">The parser for the elements.</param>
    /// <param name="delimiter">The parser for the delimiter.</param>
    /// <param name="terminator">The parser for the terminator.</param>
    /// <param name="min">The minimum number of matched elements.</param>
    /// <param name="max">The maximum number of matched elements.</param>
    public MultipleParser(
        IParser<TElement> element,
        IParser<TDelimiter> delimiter,
        IParser<TTerminator> terminator,
        ulong min,
        ulong max)
    {
        Element = element;
        Delimiter = delimiter;
        Terminator = terminator;
        Min = min;
        Max = max;

        if (Max < Min)
        {
            throw new ArgumentException($"Max ({Max}) may not me less than Min ({Min}).", nameof(max));
        }
    }

    /// <summary>
    /// The parser for the elements.
    /// </summary>
    public IParser<TElement> Element { get; }

    /// <summary>
    /// The parser for the delimiter.
    /// </summary>
    public IParser<TDelimiter> Delimiter { get; }

    /// <summary>
    /// The parser for the terminator.
    /// </summary>
    public IParser<TTerminator> Terminator { get; }

    /// <summary>
    /// The minimum number of matched elements.
    /// </summary>
    public ulong Min { get; }

    /// <summary>
    /// The maximum number of matched elements.
    /// </summary>
    public ulong Max { get; }

    /// <inheritdoc />
    IParser<TElement> IParserFirst<TElement>.First => Element;

    /// <inheritdoc />
    IParser<TDelimiter> IParserSecond<TDelimiter>.Second => Delimiter;

    /// <inheritdoc />
    IParser<TTerminator> IParserThird<TTerminator>.Third => Terminator;

    /// <inheritdoc />
    (ulong Min, ulong Max) IParserValue<(ulong Min, ulong Max)>.Value => (Min, Max);

    /// <inheritdoc />
    public override IIterativeStep Eval(IReadOnlyParseContext context, int position, Func<IParser, int, IIterativeStep> eval)
        => Min > 0
        ? EvalMin(context, position, position, eval, Min, new List<TElement>())
        : EvalMax(context, position, position, position, eval, Max, new List<TElement>());

    private IIterativeStep EvalMin(
        IReadOnlyParseContext context,
        int initialPosition,
        int position,
        Func<IParser, int, IIterativeStep> eval,
        ulong rem,
        List<TElement> acc)
        => Iterative.More(
            () => eval(Element, position),
            untypedElement =>
            {
                var element = untypedElement.AssertOfType<IParseResult<TElement>>();

                if (!element.Success)
                {
                    return Iterative.Done(this.Mismatch(context, element.Position, element.Errors));
                }

                acc.Add(element.Value);

                return Iterative.More(
                    () => eval(Delimiter, element.NextPosition),
                    untypedDelimiter =>
                    {
                        var delimiter = untypedDelimiter.AssertOfType<IParseResult<TDelimiter>>();

                        if (rem > 1)
                        {
                            if (!delimiter.Success)
                            {
                                return Iterative.Done(this.Mismatch(context, initialPosition, delimiter.Errors));
                            }
                            else
                            {
                                return EvalMin(context, initialPosition, delimiter.NextPosition, eval, rem - 1, acc);
                            }
                        }

                        var maxRem = Max - Min;
                        if (!delimiter.Success || maxRem == 0)
                        {
                            return EvalTerminator(context, initialPosition, element.NextPosition, eval, acc);
                        }

                        return EvalMax(context, initialPosition, delimiter.NextPosition, element.NextPosition, eval, maxRem, acc);
                    });
            });

    private IIterativeStep EvalMax(
        IReadOnlyParseContext context,
        int initialPosition,
        int position,
        int lastElementPosition,
        Func<IParser, int, IIterativeStep> eval,
        ulong rem,
        List<TElement> acc)
        => Iterative.More(
            () => eval(Element, position),
            untypedElement =>
            {
                var element = untypedElement.AssertOfType<IParseResult<TElement>>();

                if (!element.Success)
                {
                    return EvalTerminator(context, initialPosition, lastElementPosition, eval, acc);
                }

                acc.Add(element.Value);

                if (rem == 0)
                {
                    return EvalTerminator(context, initialPosition, element.NextPosition, eval, acc);
                }

                return Iterative.More(
                    () => eval(Delimiter, element.NextPosition),
                    untypedDelimiter =>
                    {
                        var delimiter = untypedDelimiter.AssertOfType<IParseResult<TDelimiter>>();

                        if (!delimiter.Success)
                        {
                            return EvalTerminator(context, initialPosition, element.NextPosition, eval, acc);
                        }

                        return EvalMax(context, initialPosition, delimiter.NextPosition, element.NextPosition, eval, rem - 1, acc);
                    });
            });

    private IIterativeStep EvalTerminator(
        IReadOnlyParseContext context,
        int initialPosition,
        int position,
        Func<IParser, int, IIterativeStep> eval,
        List<TElement> acc)
        => Iterative.More(
            () => eval(Terminator, position),
            untypedTerminator =>
            {
                var terminator = untypedTerminator.AssertOfType<IParseResult<TTerminator>>();

                if (!terminator.Success)
                {
                    return Iterative.Done(this.Mismatch(context, initialPosition, terminator.Errors));
                }

                return Iterative.Done(this.Match(context, initialPosition, terminator.NextPosition - initialPosition, acc.ToImmutableArray()));
            });

    /// <inheritdoc />
    protected override string InternalToString(int depth)
        => $"Multiple({Element.ToString(depth - 1)}, {Delimiter.ToString(depth - 1)}, {Terminator.ToString(depth - 1)}, {Min}, {Max})";
}
