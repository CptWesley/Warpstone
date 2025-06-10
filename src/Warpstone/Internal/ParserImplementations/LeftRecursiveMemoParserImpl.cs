using Warpstone.Internal.ParserExpressions;

namespace Warpstone.Internal.ParserImplementations;

/// <summary>
/// Represents a parser which caches the result for a given parser at a given position
/// so that it can be reused later if the same parser is executed at the same position again.
/// This variant of the <see cref="MemoParserImpl{T}"/> keeps growing the result while possible.
/// </summary>
/// <typeparam name="T">The return type of the cached parser.</typeparam>
internal sealed class LeftRecursiveMemoParserImpl<T> : ParserImplementationBase<LeftRecursiveMemoParser<T>, T>
{
    private Continuation continuation = default!;
    private IParserImplementation<T> inner = default!;

    /// <inheritdoc />
    protected override void InitializeInternal(LeftRecursiveMemoParser<T> parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
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

        var lastRes = new UnsafeParseResult(position, [new InfiniteRecursionError(context, this, position, 1)]);
        context.MemoTable[position, inner] = lastRes;

        while (true)
        {
            var newResult = inner.Apply(context, position);
            if (!newResult.Success || newResult.NextPosition <= lastRes.NextPosition)
            {
                break;
            }

            lastRes = newResult;
            context.MemoTable[position, inner] = newResult;
        }

        result = lastRes;
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

            if (!newResult.Success || newResult.NextPosition <= lastResult.NextPosition)
            {
                context.ResultStack.Push(lastResult);
                return;
            }

            if (newResult.NextPosition <= lastResult.NextPosition)
            {
                return;
            }

            context.MemoTable[position, Parser] = newResult;
            context.ExecutionStack.Push((position, this));
            context.ExecutionStack.Push((position, Parser));
        }
    }
}
