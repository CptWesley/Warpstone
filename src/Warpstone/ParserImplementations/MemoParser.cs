namespace Warpstone.ParserImplementations;

internal sealed class MemoParser<T>(IParser<T> Parser) : IParser<T>
{
    public Type ResultType => typeof(T);

    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        if (context.MemoTable.TryGetValue(position, Parser, out var result))
        {
            return result;
        }

        context.MemoTable[position, Parser] = new(position, [new InfiniteRecursionError(context, this, position, 1)]);
        result = Parser.Apply(context, position);
        context.MemoTable[position, Parser] = result;
        return result;
    }

    public void Apply(IIterativeParseContext context, int position)
    {
        throw new NotImplementedException();
    }
}
