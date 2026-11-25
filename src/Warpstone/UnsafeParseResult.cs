using System;
using System.Collections.Generic;
using System.Threading;
using Warpstone.Errors;
using Warpstone.Internal.Input;

namespace Warpstone
{
    /// <summary>
    /// Represents an internal untyped, unsafe parse result.
    /// A failed result carries no errors: parsers report why they failed to <see cref="IParseContext.Failures"/>.
    /// </summary>
    public readonly struct UnsafeParseResult
    {
        private static readonly object InProgressMarker = new();

        /// <summary>
        /// The position of the result.
        /// </summary>
        public readonly int Position;

        /// <summary>
        /// The length of the result.
        /// </summary>
        public readonly int Length;

        /// <summary>
        /// The next position to parse.
        /// </summary>
        public int NextPosition => Position + Length;

        /// <summary>
        /// Indicates whether or not the result was succesful.
        /// </summary>
        public readonly bool Success;

        /// <summary>
        /// The parsed value if successful.
        /// </summary>
        public readonly object? Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsafeParseResult"/> struct.
        /// </summary>
        /// <param name="position">The position where the result was obtained.</param>
        /// <param name="length">The length of the value in the input.</param>
        /// <param name="value">The found value.</param>
        public UnsafeParseResult(int position, int length, object? value)
            : this(position, length, value, success: true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsafeParseResult"/> struct, successful or not.
        /// </summary>
        /// <param name="position">The position where the result was obtained.</param>
        /// <param name="length">The length of the value in the input.</param>
        /// <param name="value">The found value, or the in-progress marker of a failure.</param>
        /// <param name="success">Whether parsing succeeded.</param>
        internal UnsafeParseResult(int position, int length, object? value, bool success)
        {
            Position = position;
            Length = length;
            Value = value;
            Success = success;
        }

        /// <summary>
        /// Gets whether this is the placeholder stored by <see cref="InProgress"/>.
        /// </summary>
        internal bool IsInProgress => !Success && ReferenceEquals(Value, InProgressMarker);

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="position">The position where the failure occurred.</param>
        /// <returns>The failed result.</returns>
        public static UnsafeParseResult Failure(int position)
            => new(position, 0, null, success: false);

        /// <summary>
        /// Creates the failed result a memoizing parser stores while it is still parsing at <paramref name="position"/>,
        /// so that a call reaching it again can tell infinite recursion apart from an ordinary failure.
        /// </summary>
        /// <param name="position">The position being parsed.</param>
        /// <returns>The placeholder result.</returns>
        internal static UnsafeParseResult InProgress(int position)
            => new(position, 0, InProgressMarker, success: false);
    }

    /// <summary>
    /// Provides extension methods for dealing with <see cref="UnsafeParseResult"/> instances.
    /// </summary>
    public static class UnsafeParseResultExtensions
    {
        /// <summary>
        /// Converts the given unsafe <paramref name="result"/> to a safe variant.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="result">The unsafe result.</param>
        /// <param name="context">The parsing context.</param>
        /// <returns>The safe result.</returns>
        public static ParseResult<T> AsSafe<T>(this in UnsafeParseResult result, IParseContext context)
        {
            if (result.Success)
            {
                return new ParseResult<T>(
                    context: context,
                    position: result.Position,
                    length: result.Length,
                    success: true,
                    value: (T)result.Value!,
                    errors: Array.Empty<IParseError>());
            }
            else
            {
                var errors = context.Failures.Enabled
                    ? context.Failures.CreateErrors(context, context.Parser.GetImplementation(context.Options), result.Position)
                    : ParseAgainForErrors(context, result.Position);
                return new ParseResult<T>(
                    context: context,
                    position: result.Position,
                    length: result.Length,
                    success: false,
                    value: default,
                    errors: errors);
            }
        }

        /// <summary>
        /// Parses the input of a failed parse that didn't track its failures again, now tracking them, to find its errors.
        /// Parsing is deterministic, so the second parse fails the same way.
        /// </summary>
        private static IReadOnlyList<IParseError> ParseAgainForErrors(IParseContext context, int position)
        {
            // The text the first parse read is kept, so the second parse needs no more and can read synchronously.
#pragma warning disable S3215 // Only the streaming input reads asynchronously; other inputs need nothing here.
            var reader = context.Input as TextReaderParseInput;
#pragma warning restore S3215
            var asynchronous = reader?.IsAsynchronous ?? false;
            if (reader is not null)
            {
                reader.IsAsynchronous = false;
            }

            try
            {
                var options = context.Options with { FailureTracking = FailureTracking.Always };
                var errors = ParseContext.Create(context.Input, context.Parser, options).RunToEnd(CancellationToken.None).Errors;

                // Only a transformation with side effects could make the second parse succeed; then report a bare failure.
                return errors.Count > 0
                    ? errors
                    : context.Failures.CreateErrors(context, context.Parser.GetImplementation(context.Options), position);
            }
            finally
            {
                if (reader is not null)
                {
                    reader.IsAsynchronous = asynchronous;
                }
            }
        }
    }
}
