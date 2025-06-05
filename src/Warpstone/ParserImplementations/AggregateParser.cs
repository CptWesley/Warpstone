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

        while (max < 0)
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
        throw new NotImplementedException();
    }
}
