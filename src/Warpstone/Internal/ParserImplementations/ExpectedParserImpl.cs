using Warpstone.Internal.ParserExpressions;

namespace Warpstone.Internal.ParserImplementations;

/// <summary>
/// Represents a parser that can override the expected token message.
/// </summary>
/// <typeparam name="T">The result type of the wrapped parser.</typeparam>
internal sealed class ExpectedParserImpl<T> : ParserImplementationBase<ExpectedParser<T>, T>
{
    private Continuation continuation = default!;
    private IParserImplementation<T> inner = default!;
    private string expected = default!;

    /// <inheritdoc />
    protected override void InitializeInternal(ExpectedParser<T> parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
    {
        expected = parser.Expected;
        inner = (IParserImplementation<T>)parserLookup[parser.Parser];
        continuation = new(expected);
    }

    /// <inheritdoc />
    public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var result = inner.Apply(context, position);

        if (result.Success)
        {
            return result;
        }
        else
        {
            return new(position, [new UnexpectedTokenError(context, this, position, 1, expected)]);
        }
    }

    /// <inheritdoc />
    public override void Apply(IIterativeParseContext context, int position)
    {
        context.ExecutionStack.Push((position, continuation));
        context.ExecutionStack.Push((position, inner));
    }

    private sealed class Continuation(string Expected) : ContinuationParserImplementationBase
    {
        /// <inheritdoc />
        public override void Apply(IIterativeParseContext context, int position)
        {
            var result = context.ResultStack.Peek();

            if (result.Success)
            {
                return;
            }

            context.ResultStack.Pop();
            context.ResultStack.Push(new(position, [new UnexpectedTokenError(context, this, position, 1, Expected)]));
        }
    }

    /// <inheritdoc />
    public override string ToString()
        => $"ExpectedParser({expected})";
}
