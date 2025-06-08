namespace Warpstone.ParserImplementations;

/// <summary>
/// Represents a parser that can override the expected token message.
/// </summary>
/// <typeparam name="T">The result type of the wrapped parser.</typeparam>
/// <param name="Parser">The wrapped parser.</param>
/// <param name="Expected">The expected string.</param>
internal sealed class ExpectedParser<T>(IParserImplementation<T> Parser, string Expected) : IParserImplementation<T>
{
    private Continuation Continue { get; } = new(Expected);

    /// <inheritdoc />
    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var result = Parser.Apply(context, position);

        if (result.Success)
        {
            return result;
        }
        else
        {
            return new(position, [new UnexpectedTokenError(context, this, position, 1, Expected)]);
        }
    }

    /// <inheritdoc />
    public void Apply(IIterativeParseContext context, int position)
    {
        context.ExecutionStack.Push((position, Continue));
        context.ExecutionStack.Push((position, Parser));
    }

    private sealed class Continuation(string Expected) : IParserImplementation
    {
        /// <inheritdoc />
        public void Apply(IIterativeParseContext context, int position)
        {
            var result = context.ResultStack.Peek();

            if (result.Success)
            {
                return;
            }

            context.ResultStack.Pop();
            context.ResultStack.Push(new(position, [new UnexpectedTokenError(context, this, position, 1, Expected)]));
        }

        /// <inheritdoc />
        public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
            => throw new NotSupportedException();
    }

    /// <inheritdoc />
    public override string ToString()
        => $"ExpectedParser({Expected})";
}
