namespace Warpstone.ParserImplementations;

public sealed record PositiveLookaheadParser<T>(IParser<T> Parser) : IParser<T>
{
    public Continuation Continue { get; } = new();

    public Type ResultType => typeof(T);

    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var result = Parser.Apply(context, position);

        if (!result.Success)
        {
            return result;
        }
        else
        {
            return new(position, 0, result.Value);
        }
    }

    public void Apply(IIterativeParseContext context, int position)
    {
        context.ExecutionStack.Push((position, Continue));
        context.ExecutionStack.Push((position, Parser));
    }

    public sealed record Continuation : IParser
    {
        /// <inheritdoc />
        public Type ResultType => throw new NotSupportedException();

        /// <inheritdoc />
        public void Apply(IIterativeParseContext context, int position)
        {
            var result = context.ResultStack.Peek();

            if (!result.Success)
            {
                return;
            }

            context.ResultStack.Pop();
            context.ResultStack.Push(new(result.Position, 0, result.Value));
        }

        /// <inheritdoc />
        public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
            => throw new NotSupportedException();
    }
}
