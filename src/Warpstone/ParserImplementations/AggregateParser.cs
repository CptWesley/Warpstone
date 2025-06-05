namespace Warpstone.ParserImplementations;

public sealed record AggregateParser<TSource, TDelimiter, TAccumulator>(
    IParser<TSource> Element,
    IParser<TDelimiter>? Delimiter,
    int MinCount,
    int MaxCount,
    Func<TAccumulator> CreateSeed,
    Func<TAccumulator, TSource, TAccumulator> Accumulate) : IParser<TAccumulator>
{
    /// <inheritdoc />
    public Type ResultType => typeof(TAccumulator);

    /// <inheritdoc />
    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var acc = CreateSeed();
        var min = MinCount;
        var max = MaxCount;
        var nextPos = position;
        var length = 0;

        var pastFirst = false;

        while (max > 0)
        {
            if (Delimiter is { })
            {
                if (!pastFirst)
                {
                    pastFirst = true;
                }
                else
                {
                    var delimiterResult = Delimiter.Apply(context, nextPos);

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

            var result = Element.Apply(context, nextPos);

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
            acc = Accumulate(acc, (TSource)result.Value!);
            length = result.NextPosition - position;

            min--;
            max--;
        }

        return new(position, length, acc);
    }

    /// <inheritdoc />
    public void Apply(IIterativeParseContext context, int position)
    {
        var acc = CreateSeed();
        context.ExecutionStack.Push((position, new ContinuationElement(
            Element: Element,
            Delimiter: Delimiter,
            StartPosition: position,
            LengthWithDelimiter: 0,
            LengthWithoutDelimiter: 0,
            MinCount: MinCount,
            MaxCount: MaxCount,
            Accumulator: acc,
            Accumulate: Accumulate)));
        context.ExecutionStack.Push((position, Element));
    }

    public sealed record ContinuationElement(
        IParser<TSource> Element,
        IParser<TDelimiter>? Delimiter,
        int StartPosition,
        int LengthWithDelimiter,
        int LengthWithoutDelimiter,
        int MinCount,
        int MaxCount,
        TAccumulator Accumulator,
        Func<TAccumulator, TSource, TAccumulator> Accumulate) : IParser
    {
        public Type ResultType => throw new NotImplementedException();

        public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
        {
            throw new NotImplementedException();
        }

        public void Apply(IIterativeParseContext context, int position)
        {
            var result = context.ResultStack.Pop();

            if (!result.Success)
            {
                throw new NotImplementedException();
                /*
                if (MinCount <= 0)
                {
                    break;
                }
                else
                {
                    return result;
                }
                */
            }
        }
    }

    public sealed record ContinuationDelimiter(
        IParser<TSource> Element,
        IParser<TDelimiter> Delimiter,
        int StartPosition,
        int LengthWithDelimiter,
        int LengthWithoutDelimiter,
        int MinCount,
        int MaxCount,
        TAccumulator Accumulator,
        Func<TAccumulator, TSource, TAccumulator> Accumulate) : IParser
    {
        public Type ResultType => throw new NotImplementedException();

        public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
        {
            throw new NotImplementedException();
        }

        public void Apply(IIterativeParseContext context, int position)
        {
            throw new NotImplementedException();
        }
    }
}
