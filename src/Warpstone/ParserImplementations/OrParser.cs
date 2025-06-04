namespace Warpstone.ParserImplementations;

/// <summary>
/// Represents a parser that parses either the provided <paramref name="Left"/> or <paramref name="Right"/> option.
/// </summary>
/// <typeparam name="T">The result type of the parsers.</typeparam>
/// <param name="Left">The first parser to try.</param>
/// <param name="Right">The second parser to try.</param>
public sealed record OrParser<T>(IParser<T> Left, IParser<T> Right) : IParser<T>
{
    /// <summary>
    /// The continuation parser when executing in iterative mode.
    /// </summary>
    public Continuation Continue { get; } = new(Right);

    /// <inheritdoc />
    public Type ResultType => typeof(T);

    /// <inheritdoc />
    public void Apply(IIterativeParseContext context, int position)
    {
        context.ExecutionStack.Push((position, Continue));
        context.ExecutionStack.Push((position, Left));
    }

    /// <inheritdoc />
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

    /// <summary>
    /// The continuation parser when executing in iterative mode.
    /// </summary>
    /// <param name="Right">The second parser to try.</param>
    public sealed record Continuation(IParser Right) : IParser
    {
        /// <inheritdoc />
        public Type ResultType => throw new NotSupportedException();

        /// <inheritdoc />
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

        /// <inheritdoc />
        public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
            => throw new NotSupportedException();
    }
}
