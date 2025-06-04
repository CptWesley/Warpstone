namespace Warpstone.ParserImplementations;

/// <summary>
/// Represents a parser that parses either the provided <paramref name="First"/> or <paramref name="Second"/> option.
/// </summary>
/// <typeparam name="T">The result type of the parsers.</typeparam>
/// <param name="First">The first parser to try.</param>
/// <param name="Second">The second parser to try.</param>
public sealed record OrParser<T>(IParser<T> First, IParser<T> Second) : IParser<T>
{
    /// <summary>
    /// The continuation parser when executing in iterative mode.
    /// </summary>
    public Continuation Continue { get; } = new(Second);

    /// <inheritdoc />
    public Type ResultType => typeof(T);

    /// <inheritdoc />
    public void Apply(IIterativeParseContext context, int position)
    {
        context.ExecutionStack.Push((position, Continue));
        context.ExecutionStack.Push((position, First));
    }

    /// <inheritdoc />
    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var left = First.Apply(context, position);

        if (left.Success)
        {
            return left;
        }

        var right = Second.Apply(context, left.NextPosition);
        return right;
    }

    /// <summary>
    /// The continuation parser when executing in iterative mode.
    /// </summary>
    /// <param name="Second">The second parser to try.</param>
    public sealed record Continuation(IParser Second) : IParser
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
            context.ExecutionStack.Push((position, Second));
        }

        /// <inheritdoc />
        public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
            => throw new NotSupportedException();
    }
}
