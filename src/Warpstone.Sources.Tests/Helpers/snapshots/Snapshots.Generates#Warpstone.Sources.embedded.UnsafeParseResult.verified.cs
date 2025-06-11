//HintName: Warpstone.Sources.embedded.UnsafeParseResult.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Warpstone.Errors;

namespace Warpstone
{
    /// <summary>
    /// Represents an internal untyped, unsafe parse result.
    /// </summary>
    public readonly struct UnsafeParseResult
    {
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
        /// The parsed value if successful; an error otherwise.
        /// </summary>
        public readonly object? Value;

        /// <summary>
        /// The encountered errors.
        /// </summary>
        public readonly IEnumerable<IParseError>? Errors;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsafeParseResult"/> struct.
        /// </summary>
        /// <param name="position">The position where the error occurred.</param>
        /// <param name="errors">The encountered errors.</param>
        public UnsafeParseResult(int position, IEnumerable<IParseError> errors)
        {
            Position = position;
            Length = 0;
            Value = null;
            Success = false;
            Errors = errors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsafeParseResult"/> struct.
        /// </summary>
        /// <param name="position">The position where the error occurred.</param>
        /// <param name="error">The encountered error.</param>
        public UnsafeParseResult(int position, IParseError error)
            : this(position, new[] { error })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsafeParseResult"/> struct.
        /// </summary>
        /// <param name="position">The position where the result was obtained.</param>
        /// <param name="length">The length of the value in the input.</param>
        /// <param name="value">The found value.</param>
        public UnsafeParseResult(int position, int length, object? value)
        {
            Position = position;
            Length = length;
            Value = value;
            Success = true;
            Errors = null;
        }
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
                return new ParseResult<T>(
                    context: context,
                    position: result.Position,
                    length: result.Length,
                    success: false,
                    value: default,
                    errors: result.Errors?.ToArray() ?? Array.Empty<IParseError>());
            }
        }
    }
}
