using Warpstone.Internal.ParserExpressions;

namespace Warpstone.Internal.ParserImplementations;

/// <summary>
/// Represents a parser which caches the result for a given parser at a given position
/// so that it can be reused later if the same parser is executed at the same position again.
/// </summary>
/// <typeparam name="T">The return type of the cached parser.</typeparam>
internal sealed class MemoParserImpl<T> : ParserImplementationBase<MemoParser<T>, T>
{
    private IParserImplementation<T> inner = default!;
    private Continuation continuation = default!;

    /// <inheritdoc />
    protected override void InitializeInternal(MemoParser<T> parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
    {
        inner = (IParserImplementation<T>)parserLookup[parser.Parser];
        continuation = new(inner);
    }

    /// <inheritdoc />
    public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        if (context.MemoTable.TryGetValue(position, inner, out var result))
        {
            return result;
        }

        context.MemoTable[position, inner] = new(position, [new InfiniteRecursionError(context, this, position, 1)]);
        result = inner.Apply(context, position);
        context.MemoTable[position, inner] = result;
        return result;
    }

    /// <inheritdoc />
    public override void Apply(IIterativeParseContext context, int position)
    {
        if (context.MemoTable.TryGetValue(position, inner, out var result))
        {
            context.ResultStack.Push(result);
            return;
        }

        context.MemoTable[position, inner] = new(position, [new InfiniteRecursionError(context, this, position, 1)]);
        context.ExecutionStack.Push((position, continuation));
        context.ExecutionStack.Push((position, inner));
    }

    private sealed class Continuation(IParserImplementation Parser) : ContinuationParserImplementationBase
    {
        /// <inheritdoc />
        public override void Apply(IIterativeParseContext context, int position)
        {
            var result = context.ResultStack.Peek();
            context.MemoTable[position, Parser] = result;
        }
    }
}
