namespace Warpstone.ParserImplementations;

/// <summary>
/// Parser which represents a positive lookahead (peek). Which does not consume any length of the input.
/// </summary>
/// <typeparam name="T">The type of the parser used to peek forward.</typeparam>
/// <param name="Parser">The parser used to peek forward.</param>
internal sealed class PositiveLookaheadParser<T>(IParser<T> Parser) : IParser<T>
{
    /// <inheritdoc />
    public Type ResultType => typeof(T);

    /// <inheritdoc />
    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var result = Parser.Apply(context, position);

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
    public void Apply(IIterativeParseContext context, int position)
    {
        context.ExecutionStack.Push((position, Continuation.Instance));
        context.ExecutionStack.Push((position, Parser));
    }

    private sealed class Continuation : IParser
    {
#pragma warning disable S2743 // Static fields should not be used in generic types
        public static readonly Continuation Instance = new();
#pragma warning restore S2743 // Static fields should not be used in generic types

        private Continuation()
        {
        }

        /// <inheritdoc />
        public Type ResultType => throw new NotSupportedException();

        /// <inheritdoc />
        public void Apply(IIterativeParseContext context, int position)
        {
            var result = context.ResultStack.Peek();

            if (!result.Success)
            {
                return;
            }

            context.ResultStack.Pop();
            context.ResultStack.Push(new(result.Position, 0, result.Value));
        }

        /// <inheritdoc />
        public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
            => throw new NotSupportedException();
    }
}
