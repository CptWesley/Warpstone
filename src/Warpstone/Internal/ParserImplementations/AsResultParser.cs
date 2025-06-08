namespace Warpstone.ParserImplementations;

/// <summary>
/// Represents a parser that unwraps the result as a <see cref="IParseResult{T}"/> during the parsing.
/// </summary>
/// <typeparam name="T">The type of the result value.</typeparam>
/// <param name="Parser">The internal parser.</param>
internal sealed class AsResultParser<T>(IParserImplementation<T> Parser) : IParserImplementation<IParseResult<T>>
{
    /// <inheritdoc />
    public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
    {
        var result = Parser.Apply(context, position);
        var typed = result.AsSafe<T>(context);
        return new(position, result.Length, typed);
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
            var typed = result.AsSafe<T>(context);
            context.ResultStack.Push(new(position, result.Length, typed));
        }

        /// <inheritdoc />
        public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
            => throw new NotSupportedException();
    }
}
