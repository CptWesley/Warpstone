namespace Warpstone.Internal.ParserImplementations;

/// <summary>
/// Represents a parser that parses a repeated series of elements and aggregates the results.
/// </summary>
/// <typeparam name="TSource">The element type.</typeparam>
/// <typeparam name="TAccumulator">The accumulator type.</typeparam>
internal sealed class AggregateParserImpl<TSource, TAccumulator> : ParserImplementationBase<AggregateParser<TSource, TAccumulator>, TAccumulator>
{
    private IParserImplementation<TSource> element = default!;
    private IParserImplementation? delimiter = default!;
    private int minCount = default!;
    private int maxCount = default!;
    private Func<TAccumulator> createSeed = default!;
    private Func<TAccumulator, TSource, TAccumulator> accumulate = default!;

    /// <inheritdoc />
    protected override void InitializeInternal(AggregateParser<TSource, TAccumulator> parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
    {
        element = (IParserImplementation<TSource>)parserLookup[parser.Element];
        delimiter = parser.Delimiter is null ? null : parserLookup[parser.Delimiter];
        minCount = parser.MinCount;
        maxCount = parser.MaxCount;
        createSeed = parser.CreateSeed;
        accumulate = parser.Accumulate;
    }

    /// <inheritdoc />
#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
    public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
    {
        var acc = createSeed();
        var min = minCount;
        var max = maxCount;
        var nextPos = position;
        var length = 0;

        var pastFirst = false;

        while (max > 0)
        {
            if (delimiter is { })
            {
                if (!pastFirst)
                {
                    pastFirst = true;
                }
                else
                {
                    var delimiterResult = delimiter.Apply(context, nextPos);

                    if (!delimiterResult.Success)
                    {
                        if (min <= 0)
                        {
                            break;
                        }
                        else
                        {
                            return delimiterResult;
                        }
                    }

                    nextPos = delimiterResult.NextPosition;
                }
            }

            var result = element.Apply(context, nextPos);

            if (!result.Success)
            {
                if (min <= 0)
                {
                    break;
                }
                else
                {
                    return result;
                }
            }

            nextPos = result.NextPosition;
            acc = accumulate(acc, (TSource)result.Value!);
            length = result.NextPosition - position;

            min--;
            max--;
        }

        return new(position, length, acc);
    }

    /// <inheritdoc />
    public override void Apply(IIterativeParseContext context, int position)
    {
        var acc = createSeed();
        context.ExecutionStack.Push((position, new ContinuationElement(
            Element: element,
            Delimiter: delimiter,
            StartPosition: position,
            Length: 0,
            MinCount: minCount,
            MaxCount: maxCount,
            Accumulator: acc,
            Accumulate: accumulate)));
        context.ExecutionStack.Push((position, element));
    }

#pragma warning disable S107 // Methods should not have too many parameters
    private sealed class ContinuationElement(
        IParserImplementation<TSource> Element,
        IParserImplementation? Delimiter,
        int StartPosition,
        int Length,
        int MinCount,
        int MaxCount,
        TAccumulator Accumulator,
        Func<TAccumulator, TSource, TAccumulator> Accumulate) : ContinuationParserImplementationBase
#pragma warning restore S107 // Methods should not have too many parameters
    {
        public override void Apply(IIterativeParseContext context, int position)
        {
            var result = context.ResultStack.Pop();

            if (!result.Success)
            {
                if (MinCount <= 0)
                {
                    context.ResultStack.Push(new(StartPosition, Length, Accumulator));
                }
                else
                {
                    context.ResultStack.Push(result);
                }

                return;
            }

            var nextPos = result.NextPosition;
            var newAccumulator = Accumulate(Accumulator, (TSource)result.Value!);
            var newLength = result.NextPosition - StartPosition;

            var newMin = MinCount - 1;
            var newMax = MaxCount - 1;

            if (newMax <= 0)
            {
                context.ResultStack.Push(new(StartPosition, newLength, newAccumulator));
                return;
            }

            if (Delimiter is { })
            {
                context.ExecutionStack.Push((nextPos, new ContinuationDelimiter(
                    Element: Element,
                    Delimiter: Delimiter,
                    StartPosition: StartPosition,
                    Length: newLength,
                    MinCount: newMin,
                    MaxCount: newMax,
                    Accumulator: newAccumulator,
                    Accumulate: Accumulate)));
                context.ExecutionStack.Push((nextPos, Delimiter));
            }
            else
            {
                context.ExecutionStack.Push((nextPos, new ContinuationElement(
                    Element: Element,
                    Delimiter: Delimiter,
                    StartPosition: StartPosition,
                    Length: newLength,
                    MinCount: newMin,
                    MaxCount: newMax,
                    Accumulator: newAccumulator,
                    Accumulate: Accumulate)));
                context.ExecutionStack.Push((nextPos, Element));
            }
        }
    }

#pragma warning disable S107 // Methods should not have too many parameters
    private sealed class ContinuationDelimiter(
        IParserImplementation<TSource> Element,
        IParserImplementation Delimiter,
        int StartPosition,
        int Length,
        int MinCount,
        int MaxCount,
        TAccumulator Accumulator,
        Func<TAccumulator, TSource, TAccumulator> Accumulate) : ContinuationParserImplementationBase
#pragma warning restore S107 // Methods should not have too many parameters
    {
        public override void Apply(IIterativeParseContext context, int position)
        {
            var result = context.ResultStack.Pop();

            if (!result.Success)
            {
                if (MinCount <= 0)
                {
                    context.ResultStack.Push(new(StartPosition, Length, Accumulator));
                }
                else
                {
                    context.ResultStack.Push(result);
                }

                return;
            }

            var nextPos = result.NextPosition;
            context.ExecutionStack.Push((nextPos, new ContinuationElement(
                Element: Element,
                Delimiter: Delimiter,
                StartPosition: StartPosition,
                Length: Length,
                MinCount: MinCount,
                MaxCount: MaxCount,
                Accumulator: Accumulator,
                Accumulate: Accumulate)));
            context.ExecutionStack.Push((nextPos, Element));
        }
    }
}
