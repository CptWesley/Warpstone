namespace Warpstone.ParserImplementations;

public sealed record AsResultParser<T>(IParser<T> Parser) : IParser<IParseResult<T>>
{
    public Continuation Continue { get; } = new();

    public Type ResultType => typeof(IParseResult<T>);

    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var result = Parser.Apply(context, position);
        var typed = result.AsSafe<T>(context);
        return new(position, result.Length, typed);
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
            var result = context.ResultStack.Pop();
            var typed = result.AsSafe<T>(context);
            context.ResultStack.Push(new(position, result.Length, typed));
        }

        /// <inheritdoc />
        public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
            => throw new NotSupportedException();
    }
}
