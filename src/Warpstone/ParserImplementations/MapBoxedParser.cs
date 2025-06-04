namespace Warpstone.ParserImplementations;

/// <summary>
/// Represents a parser that converts the value of the given <paramref name="Element"/> parser
/// using the provided <paramref name="Map"/> function.
/// </summary>
/// <typeparam name="TIn">The result type of the <paramref name="Element"/> parser.</typeparam>
/// <typeparam name="TOut">The result type of the <paramref name="Map"/> function.</typeparam>
/// <param name="Element">The input parser.</param>
/// <param name="Map">The map function.</param>
public sealed record MapBoxedParser<TIn, TOut>(IParser<TIn> Element, Func<TIn, TOut> Map) : IParser<TOut>
    where TIn : struct
{
    /// <summary>
    /// The continuation function when running in iterative mode.
    /// </summary>
    public Continuation Continue { get; } = new(Map);

    /// <inheritdoc />
    public Type ResultType => typeof(TOut);

    /// <inheritdoc />
    public void Apply(IIterativeParseContext context, int position)
    {
        context.ExecutionStack.Push((position, Continue));
        context.ExecutionStack.Push((position, Element));
    }

    /// <inheritdoc />
    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var prevResult = Element.Apply(context, position);
        if (!prevResult.Success)
        {
            return prevResult;
        }

#if NETCOREAPP3_0_OR_GREATER
        var value = Unsafe.Unbox<TIn>(prevResult.Value!);
#else
        var value = (TIn)prevResult.Value!;
#endif

        try
        {
            var modified = Map(value);
            return new UnsafeParseResult(prevResult.Position, prevResult.Length, modified!);
        }
        catch (Exception e)
        {
            return new UnsafeParseResult(prevResult.Position, [new TransformationError(context, this, position, 0, null, e)]);
        }
    }

    /// <summary>
    /// The continuation function when running in iterative mode.
    /// </summary>
    /// <param name="Map">The map function.</param>
    public sealed record Continuation(Func<TIn, TOut> Map) : IParser
    {
        /// <inheritdoc />
        public Type ResultType => throw new NotSupportedException();

        /// <inheritdoc />
        public void Apply(IIterativeParseContext context, int position)
        {
            var prevResult = context.ResultStack.Peek();

            if (!prevResult.Success)
            {
                return;
            }

            context.ResultStack.Pop();

#if NETCOREAPP3_0_OR_GREATER
            var value = Unsafe.Unbox<TIn>(prevResult.Value!);
#else
            var value = (TIn)prevResult.Value!;
#endif

            try
            {
                var modified = Map(value);
                context.ResultStack.Push(new UnsafeParseResult(prevResult.Position, prevResult.Length, modified!));
            }
            catch (Exception e)
            {
                context.ResultStack.Push(new UnsafeParseResult(prevResult.Position, [new TransformationError(context, this, position, 0, null, e)]));
            }
        }

        /// <inheritdoc />
        public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
            => throw new NotSupportedException();
    }
}
