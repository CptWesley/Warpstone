namespace Warpstone.ParserImplementations;

public sealed record NegativeLookaheadParser<T>(IParser<T> Parser) : IParser<T?>
{
    public Continuation Continue { get; } = new();

    public Type ResultType => typeof(T);

    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var result = Parser.Apply(context, position);

        if (result.Success)
        {
            return new(position, [new UnexpectedTokenError(context, this, position, 1, "<not>")]);
        }
        else
        {
            return new(position, 0, default(T?));
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
            var result = context.ResultStack.Pop();

            if (result.Success)
            {
                context.ResultStack.Push(new(result.Position, [new UnexpectedTokenError(context, this, position, 1, "<not>")]));
            }
            else
            {
                context.ResultStack.Push(new(result.Position, 0, default(T?)));
            }
        }

        /// <inheritdoc />
        public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
            => throw new NotSupportedException();
    }
}
