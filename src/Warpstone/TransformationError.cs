using System;

namespace Warpstone
{
    /// <summary>
    /// Parse error that occurs when a transformation causes an exception.
    /// </summary>
    public class TransformationError : IParseError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationError"/> class.
        /// </summary>
        /// <param name="e">The exception that caused the parse error.</param>
        public TransformationError(Exception e)
            => Exception = e;

        /// <summary>
        /// Gets the exception that caused the parse error.
        /// </summary>
        public Exception Exception { get; }

        /// <inheritdoc/>
        public string GetMessage()
            => Exception.Message;
    }
}
