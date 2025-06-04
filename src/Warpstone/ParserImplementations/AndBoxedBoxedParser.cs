namespace Warpstone.ParserImplementations;

/// <summary>
/// Represents a parser that performs two parse operations sequentially and combines the result.
/// </summary>
/// <typeparam name="TLeft">The result type of the <paramref name="Left"/> parser.</typeparam>
/// <typeparam name="TRight">The result type of the <paramref name="Right"/> parser.</typeparam>
/// <param name="Left">The parser that is executed first.</param>
/// <param name="Right">The parser that is executed after the first one has succeeded.</param>
public sealed record AndBoxedBoxedParser<TLeft, TRight>(IParser<TLeft> Left, IParser<TRight> Right) : IParser<(TLeft Left, TRight Right)>
    where TLeft : struct
    where TRight : struct
{
    /// <inheritdoc />
    public Type ResultType => typeof((TLeft, TRight));

    /// <summary>
    /// The first continuation of the sequential parser when executing in iterative mode.
    /// </summary>
    public Continuation Continue { get; } = new(Right);

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

        if (!left.Success)
        {
            return left;
        }

        var right = Right.Apply(context, left.NextPosition);

        if (!right.Success)
        {
            return right;
        }

#if NETCOREAPP3_0_OR_GREATER
        var leftValue = Unsafe.Unbox<TLeft>(left.Value!);
        var rightValue = Unsafe.Unbox<TRight>(right.Value!);
#else
        var leftValue = (TLeft)left.Value!;
        var rightValue = (TRight)right.Value!;
#endif
        var newValue = (leftValue, rightValue);

        var newLength = left.Length + right.Length;
        return new UnsafeParseResult(left.Position, newLength, newValue);
    }

    /// <summary>
    /// The first continuation of the sequential parser when executing in iterative mode.
    /// </summary>
    public sealed record Continuation(IParser<TRight> Right) : IParser
    {
        /// <summary>
        /// The second continuation of the sequential parser when executing in iterative mode.
        /// </summary>
        public SecondContinuation Continue { get; } = new();

        /// <inheritdoc />
        public Type ResultType => throw new NotSupportedException();

        /// <inheritdoc />
        public void Apply(IIterativeParseContext context, int position)
        {
            var leftResult = context.ResultStack.Peek();

            if (!leftResult.Success)
            {
                return;
            }

            var nextPos = leftResult.NextPosition;

            context.ExecutionStack.Push((nextPos, Continue));
            context.ExecutionStack.Push((nextPos, Right));
        }

        /// <inheritdoc />
        public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
            => throw new NotSupportedException();

        /// <summary>
        /// The second continuation of the sequential parser when executing in iterative mode.
        /// </summary>
        public sealed record SecondContinuation() : IParser
        {
            /// <inheritdoc />
            public Type ResultType => throw new NotSupportedException();

            /// <inheritdoc />
            public void Apply(IIterativeParseContext context, int position)
            {
                var right = context.ResultStack.Pop();
                var left = context.ResultStack.Pop();

                if (!right.Success)
                {
                    context.ResultStack.Push(right);
                    return;
                }

#if NETCOREAPP3_0_OR_GREATER
                var leftValue = Unsafe.Unbox<TLeft>(left.Value!);
                var rightValue = Unsafe.Unbox<TRight>(right.Value!);
#else
                var leftValue = (TLeft)left.Value!;
                var rightValue = (TRight)right.Value!;
#endif
                var newValue = (leftValue, rightValue);

                var newLength = left.Length + right.Length;
                context.ResultStack.Push(new UnsafeParseResult(left.Position, newLength, newValue));
            }

            /// <inheritdoc />
            public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
                => throw new NotSupportedException();
        }
    }
}
