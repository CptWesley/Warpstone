namespace Warpstone.ParserImplementations;

/// <summary>
/// Represents a parser that performs two parse operations sequentially and combines the result.
/// </summary>
/// <typeparam name="TFirst">The result type of the <paramref name="First"/> parser.</typeparam>
/// <typeparam name="TSecond">The result type of the <paramref name="Second"/> parser.</typeparam>
/// <param name="First">The parser that is executed first.</param>
/// <param name="Second">The parser that is executed after the first one has succeeded.</param>
public sealed record AndBoxedBoxedParser<TFirst, TSecond>(IParser<TFirst> First, IParser<TSecond> Second) : IParser<(TFirst First, TSecond Second)>
    where TFirst : struct
    where TSecond : struct
{
    /// <inheritdoc />
    public Type ResultType => typeof((TFirst, TSecond));

    /// <summary>
    /// The first continuation of the sequential parser when executing in iterative mode.
    /// </summary>
    public Continuation Continue { get; } = new(Second);

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

        if (!left.Success)
        {
            return left;
        }

        var right = Second.Apply(context, left.NextPosition);

        if (!right.Success)
        {
            return right;
        }

#if NETCOREAPP3_0_OR_GREATER
        var leftValue = Unsafe.Unbox<TFirst>(left.Value!);
        var rightValue = Unsafe.Unbox<TSecond>(right.Value!);
#else
        var leftValue = (TFirst)left.Value!;
        var rightValue = (TSecond)right.Value!;
#endif
        var newValue = (leftValue, rightValue);

        var newLength = left.Length + right.Length;
        return new UnsafeParseResult(left.Position, newLength, newValue);
    }

    /// <summary>
    /// The first continuation of the sequential parser when executing in iterative mode.
    /// </summary>
    public sealed record Continuation(IParser<TSecond> Second) : IParser
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
            context.ExecutionStack.Push((nextPos, Second));
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
                var leftValue = Unsafe.Unbox<TFirst>(left.Value!);
                var rightValue = Unsafe.Unbox<TSecond>(right.Value!);
#else
                var leftValue = (TFirst)left.Value!;
                var rightValue = (TSecond)right.Value!;
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
