namespace Warpstone.ParserImplementations;

public sealed record OrParser<T>(IParser<T> Left, IParser<T> Right) : IParser<T>
{
    public Continuation Continue { get; } = new(Right);

    public Type ResultType => typeof(T);

    public void Apply(IIterativeParseContext context, int position)
    {
        context.ExecutionStack.Push((position, Continue));
        context.ExecutionStack.Push((position, Left));
    }

    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var left = Left.Apply(context, position);

        if (left.Success)
        {
            return left;
        }

        var right = Right.Apply(context, left.NextPosition);
        return right;
    }

    public sealed record Continuation(IParser Right) : IParser
    {
        public Type ResultType => throw new NotSupportedException();

        public void Apply(IIterativeParseContext context, int position)
        {
            var leftResult = context.ResultStack.Peek();

            if (leftResult.Success)
            {
                return;
            }

            context.ResultStack.Pop();
            context.ExecutionStack.Push((position, Right));
        }

        public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
            => throw new NotSupportedException();
    }
}
