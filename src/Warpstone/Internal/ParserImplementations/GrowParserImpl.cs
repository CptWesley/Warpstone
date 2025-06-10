namespace Warpstone.Internal.ParserImplementations;

/// <summary>
/// Represents a parser which caches the result for a given parser at a given position
/// so that it can be reused later if the same parser is executed at the same position again.
/// This variant of the <see cref="MemoParserImpl{T}"/> keeps growing the result while possible.
/// </summary>
/// <typeparam name="T">The return type of the cached parser.</typeparam>
/// <seealso href="https://medium.com/@gvanrossum_83706/left-recursive-peg-grammars-65dab3c580e1"/>
internal sealed class GrowParserImpl<T> : ParserImplementationBase<GrowParser<T>, T>
{
    private Continuation continuation = default!;
    private IParserImplementation<T> inner = default!;

    /// <inheritdoc />
    protected override void InitializeInternal(GrowParser<T> parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
    {
        inner = (IParserImplementation<T>)parserLookup[parser.Parser];
        continuation = new(inner);
    }

    /// <inheritdoc />
    public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        if (context.MemoTable.TryGetValue(position, inner, out var result))
        {
            return result;
        }

        var lastResult = new UnsafeParseResult(position, [new InfiniteRecursionError(context, this, position, 1)]);
        context.MemoTable[position, inner] = lastResult;

        while (true)
        {
            var newResult = inner.Apply(context, position);
            if (!ResultHasImproved(lastResult, newResult))
            {
                break;
            }

            lastResult = newResult;
            context.MemoTable[position, inner] = newResult;
        }

        result = lastResult;
        return result;
    }

    /// <inheritdoc />
    public override void Apply(IIterativeParseContext context, int position)
    {
        if (context.MemoTable.TryGetValue(position, inner, out var result))
        {
            context.ResultStack.Push(result);
            return;
        }

        context.MemoTable[position, inner] = new(position, [new InfiniteRecursionError(context, this, position, 1)]);
        context.ExecutionStack.Push((position, continuation));
        context.ExecutionStack.Push((position, inner));
    }

    private sealed class Continuation(IParserImplementation Parser) : ContinuationParserImplementationBase
    {
        public override void Apply(IIterativeParseContext context, int position)
        {
            var newResult = context.ResultStack.Pop();
            var lastResult = context.MemoTable[position, Parser];

            if (!ResultHasImproved(lastResult, newResult))
            {
                context.ResultStack.Push(lastResult);
                return;
            }

            context.MemoTable[position, Parser] = newResult;
            context.ExecutionStack.Push((position, this));
            context.ExecutionStack.Push((position, Parser));
        }
    }

    private static bool ResultHasImproved(in UnsafeParseResult prev, in UnsafeParseResult cur)
    {
        if (!prev.Success)
        {
            return cur.Success;
        }

        if (!cur.Success)
        {
            return false;
        }

        return cur.NextPosition > prev.NextPosition;
    }
}
