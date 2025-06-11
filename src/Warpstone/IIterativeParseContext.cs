namespace Warpstone
{
    /// <summary>
    /// Represents the context necessary for parsing iteratively.
    /// </summary>
    public interface IIterativeParseContext : IParseContext
    {
        /// <summary>
        /// The result stack.
        /// </summary>
        public Stack<UnsafeParseResult> ResultStack { get; }

        /// <summary>
        /// The execution stack.
        /// </summary>
        public Stack<(int Position, IParserImplementation Parser)> ExecutionStack { get; }
    }
}
