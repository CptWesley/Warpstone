namespace Warpstone.Internal.ParserImplementations;

/// <summary>
/// Parser which represents a positive lookahead (not). Which does not consume any length of the input.
/// </summary>
/// <typeparam name="T">The type of the parser used to peek forward.</typeparam>
/// <param name="Parser">The parser used to peek forward.</param>
public sealed class NegativeLookaheadParserImpl<T>(IParserImplementation<T> Parser) : IParserImplementation<T?>
{
    /// <inheritdoc />
    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var result = Parser.Apply(context, position);

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
    public void Apply(IIterativeParseContext context, int position)
    {
        context.ExecutionStack.Push((position, Continuation.Instance));
        context.ExecutionStack.Push((position, Parser));
    }

    private sealed class Continuation : IParserImplementation
    {
#pragma warning disable S2743 // Static fields should not be used in generic types
        public static readonly Continuation Instance = new();
#pragma warning restore S2743 // Static fields should not be used in generic types

        private Continuation()
        {
        }

        /// <inheritdoc />
        public void Apply(IIterativeParseContext context, int position)
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

        /// <inheritdoc />
        public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
            => throw new NotSupportedException();
    }
}
