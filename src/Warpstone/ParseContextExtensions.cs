namespace Warpstone
{
    /// <summary>
    /// Provides extension methods for <see cref="IParseContext"/>
    /// <see cref="IParseContext{T}"/>, <see cref="IReadOnlyParseContext"/>
    /// and <see cref="IReadOnlyParseContext{T}"/> interface implementations.
    /// </summary>
    public static class ParseContextExtensions
    {
        /// <summary>
        /// Runs the parser to completion.
        /// </summary>
        /// <param name="context">The context used for parsing.</param>
        /// <returns>The result of parsing.</returns>
        /// <typeparam name="T">The result type of the parsing process.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParseResult<T> RunToEnd<T>(this IParseContext<T> context)
            => context.RunToEnd(default);

        /// <summary>
        /// Runs the parser to completion.
        /// </summary>
        /// <param name="context">The context used for parsing.</param>
        /// <returns>The result of parsing.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParseResult RunToEnd(this IParseContext context)
            => context.RunToEnd(default);

        /// <summary>
        /// Retrieves a read-only variant of the given <paramref name="context"/> table.
        /// </summary>
        /// <param name="context">The parse context to wrap.</param>
        /// <returns>A read-only variant of the given <paramref name="context"/>.</returns>
        /// <typeparam name="T">The result type of the parsing process.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IReadOnlyParseContext<T> AsReadOnly<T>(this IReadOnlyParseContext<T> context)
            => context is ReadOnlyParseContext<T>
            ? context
            : new ReadOnlyParseContext<T>(context);

        /// <summary>
        /// Retrieves a read-only variant of the given <paramref name="context"/> table.
        /// </summary>
        /// <param name="context">The parse context to wrap.</param>
        /// <returns>A read-only variant of the given <paramref name="context"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IReadOnlyParseContext AsReadOnly(this IReadOnlyParseContext context)
            => context is ReadOnlyParseContext
            ? context
            : new ReadOnlyParseContext(context);
    }
}
