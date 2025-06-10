namespace Warpstone.Internal.ParserImplementations;

/// <summary>
/// Parser which represents a positive lookahead (peek). Which does not consume any length of the input.
/// </summary>
/// <typeparam name="T">The type of the parser used to peek forward.</typeparam>
internal sealed class PositiveLookaheadParserImpl<T> : ParserImplementationBase<PositiveLookaheadParser<T>, T>
{
    private IParserImplementation<T> inner = default!;

    /// <inheritdoc />
    protected override void InitializeInternal(PositiveLookaheadParser<T> parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
    {
        inner = (IParserImplementation<T>)parserLookup[parser.Parser];
    }

    /// <inheritdoc />
    public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var result = inner.Apply(context, position);

        if (!result.Success)
        {
            return result;
        }
        else
        {
            return new(position, 0, result.Value);
        }
    }

    /// <inheritdoc />
    public override void Apply(IIterativeParseContext context, int position)
    {
        context.ExecutionStack.Push((position, Continuation.Instance));
        context.ExecutionStack.Push((position, inner));
    }

    private sealed class Continuation : ContinuationParserImplementationBase
    {
#pragma warning disable S2743 // Static fields should not be used in generic types
        public static readonly Continuation Instance = new();
#pragma warning restore S2743 // Static fields should not be used in generic types

        private Continuation()
        {
        }

        /// <inheritdoc />
        public override void Apply(IIterativeParseContext context, int position)
        {
            var result = context.ResultStack.Peek();

            if (!result.Success)
            {
                return;
            }

            context.ResultStack.Pop();
            context.ResultStack.Push(new(result.Position, 0, result.Value));
        }
    }
}
