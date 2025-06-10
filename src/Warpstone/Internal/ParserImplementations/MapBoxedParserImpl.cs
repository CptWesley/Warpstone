using Warpstone.Internal.ParserExpressions;

namespace Warpstone.Internal.ParserImplementations;

/// <summary>
/// Represents a parser that converts the value of the given element parser
/// using the provided map function.
/// </summary>
/// <typeparam name="TIn">The result type of the element parser.</typeparam>
/// <typeparam name="TOut">The result type of the map function.</typeparam>
internal sealed class MapBoxedParserImpl<TIn, TOut> : ParserImplementationBase<MapParser<TIn, TOut>, TOut>
    where TIn : struct
{
    private Continuation continuation = default!;
    private IParserImplementation<TIn> element = default!;
    private Func<TIn, TOut> map = default!;

    /// <inheritdoc />
    protected override void InitializeInternal(MapParser<TIn, TOut> parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
    {
        element = (IParserImplementation<TIn>)parserLookup[parser.Element];
        map = parser.Map;
        continuation = new(map);
    }

    /// <inheritdoc />
    public override void Apply(IIterativeParseContext context, int position)
    {
        context.ExecutionStack.Push((position, continuation));
        context.ExecutionStack.Push((position, element));
    }

    /// <inheritdoc />
    public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var prevResult = element.Apply(context, position);
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
            var modified = map(value);
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
    private sealed class Continuation(Func<TIn, TOut> Map) : ContinuationParserImplementationBase
    {
        /// <inheritdoc />
        public override void Apply(IIterativeParseContext context, int position)
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
    }
}
