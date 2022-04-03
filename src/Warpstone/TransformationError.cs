using System;

namespace Warpstone
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
        /// <param name="allowBacktracking">Indicates whether or not we can backtrack from this error.</param>
        /// <param name="e">The exception that caused the parse error.</param>
        public TransformationError(SourcePosition position, bool allowBacktracking, Exception e)
            : base(position, allowBacktracking)
            => Exception = e;

        /// <summary>
        /// Gets the exception that caused the parse error.
        /// </summary>
        public Exception Exception { get; }

        /// <inheritdoc/>
        public override IParseError DisallowBacktracking()
            => new TransformationError(Position, false, Exception);

        /// <inheritdoc/>
        protected override string GetSimpleMessage()
            => Exception.Message;
    }
}
