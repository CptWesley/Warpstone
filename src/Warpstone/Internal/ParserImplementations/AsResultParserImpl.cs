using Warpstone.Internal.ParserExpressions;
using Warpstone.Internal.ParserImplementations;

namespace Warpstone.Internal.ParserImplementations;

/// <summary>
/// Represents a parser that unwraps the result as a <see cref="IParseResult{T}"/> during the parsing.
/// </summary>
/// <typeparam name="T">The type of the result value.</typeparam>
internal sealed class AsResultParserImpl<T> : ParserImplementationBase<AsResultParser<T>, IParseResult<T>>
{
    private IParserImplementation<T> inner = default!;

    /// <inheritdoc />
    protected override void InitializeInternal(AsResultParser<T> parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
    {
        inner = (IParserImplementation<T>)parserLookup[parser.Parser];
    }

    /// <inheritdoc />
    public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var result = inner.Apply(context, position);
        var typed = result.AsSafe<T>(context);
        return new(position, result.Length, typed);
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
            var typed = result.AsSafe<T>(context);
            context.ResultStack.Push(new(position, result.Length, typed));
        }
    }
}
