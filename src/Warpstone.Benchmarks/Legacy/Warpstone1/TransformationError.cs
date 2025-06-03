using System;

namespace Legacy.Warpstone1
{
    /// <summary>
    /// Parse error that occurs when a transformation causes an exception.
    /// </summary>
    public class TransformationError : ParseError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationError"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="e">The exception that caused the parse error.</param>
        public TransformationError(SourcePosition position, Exception e)
            : base(position)
            => Exception = e;

        /// <summary>
        /// Gets the exception that caused the parse error.
        /// </summary>
        public Exception Exception { get; }

        /// <inheritdoc/>
        protected override string GetSimpleMessage()
            => Exception.Message;
    }
}
