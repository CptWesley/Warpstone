namespace Warpstone.ParserImplementations;

public sealed record ExpectedParser<T>(IParser<T> Parser, string Expected) : IParser<T>
{
    public Continuation Continue { get; } = new(Expected);

    public Type ResultType => typeof(T);

    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var result = Parser.Apply(context, position);

        if (result.Success)
        {
            return result;
        }
        else
        {
            return new(position, [new UnexpectedTokenError(context, this, position, 1, Expected)]);
        }
    }

    public void Apply(IIterativeParseContext context, int position)
    {
        context.ExecutionStack.Push((position, Continue));
        context.ExecutionStack.Push((position, Parser));
    }

    public sealed record Continuation(string Expected) : IParser
    {
        /// <inheritdoc />
        public Type ResultType => throw new NotSupportedException();

        /// <inheritdoc />
        public void Apply(IIterativeParseContext context, int position)
        {
            var result = context.ResultStack.Peek();

            if (result.Success)
            {
                return;
            }

            context.ResultStack.Pop();
            context.ResultStack.Push(new(position, [new UnexpectedTokenError(context, this, position, 1, Expected)]));
        }

        /// <inheritdoc />
        public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
            => throw new NotSupportedException();
    }
}
