using Warpstone.Internal.ParserExpressions;

namespace Warpstone.Internal.ParserImplementations;

/// <summary>
/// Parser which represents a positive lookahead (not). Which does not consume any length of the input.
/// </summary>
/// <typeparam name="T">The type of the parser used to peek forward.</typeparam>
internal sealed class NegativeLookaheadParserImpl<T> : ParserImplementationBase<NegativeLookaheadParser<T>, T?>
{
    private IParserImplementation<T> inner = default!;

    /// <inheritdoc />
    protected override void InitializeInternal(NegativeLookaheadParser<T> parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
    {
        inner = (IParserImplementation<T>)parserLookup[parser.Parser];
    }

    /// <inheritdoc />
    public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var result = inner.Apply(context, position);

        if (result.Success)
        {
            return new(position, [new UnexpectedTokenError(context, this, position, 1, "<not>")]);
        }
        else
        {
            return new(position, 0, default(T?));
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
    }
}
