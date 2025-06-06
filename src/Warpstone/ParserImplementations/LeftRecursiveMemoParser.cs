namespace Warpstone.ParserImplementations;

/// <summary>
/// Represents a parser which caches the result for a given parser at a given position
/// so that it can be reused later if the same parser is executed at the same position again.
/// This variant of the <see cref="MemoParser{T}"/> keeps growing the result while possible.
/// </summary>
/// <typeparam name="T">The return type of the cached parser.</typeparam>
/// <param name="Parser">The parser to be cached.</param>
internal sealed class LeftRecursiveMemoParser<T>(IParser<T> Parser) : IParser<T>
{
    private Continuation Continue { get; } = new(Parser);

    /// <inheritdoc />
    public Type ResultType => typeof(T);

    /// <inheritdoc />
    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        if (context.MemoTable.TryGetValue(position, Parser, out var result))
        {
            return result;
        }

        var lastRes = new UnsafeParseResult(position, [new InfiniteRecursionError(context, this, position, 1)]);
        context.MemoTable[position, Parser] = lastRes;

        while (true)
        {
            var newResult = Parser.Apply(context, position);
            if (!newResult.Success || newResult.NextPosition <= lastRes.NextPosition)
            {
                break;
            }

            lastRes = newResult;
            context.MemoTable[position, Parser] = newResult;
        }

        result = lastRes;
        return result;
    }

    /// <inheritdoc />
    public void Apply(IIterativeParseContext context, int position)
    {
        if (context.MemoTable.TryGetValue(position, Parser, out var result))
        {
            context.ResultStack.Push(result);
            return;
        }

        context.MemoTable[position, Parser] = new(position, [new InfiniteRecursionError(context, this, position, 1)]);
        context.ExecutionStack.Push((position, Continue));
        context.ExecutionStack.Push((position, Parser));
    }

    private sealed class Continuation(IParser Parser) : IParser
    {
        public Type ResultType => throw new NotImplementedException();

        public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
        {
            throw new NotImplementedException();
        }

        public void Apply(IIterativeParseContext context, int position)
        {
            var result = context.ResultStack.Peek();
            context.MemoTable[position, Parser] = result;
        }
    }
}
