using System;
using System.Collections.Generic;
using System.Linq;
using Warpstone.Errors;

namespace Warpstone
{
    /// <summary>
    /// Represents the result of parsing.
    /// </summary>
    /// <typeparam name="T">The type of the values obtained in the results.</typeparam>
    public sealed class ParseResult<T> : IParseResult<T>
    {
        private readonly T? value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseResult{T}"/> class.
        /// </summary>
        /// <param name="context">The parse context.</param>
        /// <param name="position">The start position of the result in the input.</param>
        /// <param name="length">The length of the result in the input.</param>
        /// <param name="success">Indicates whether or not the result was succesful.</param>
        /// <param name="value">The result value if succesful.</param>
        /// <param name="errors">The errors if parsing failed.</param>
        public ParseResult(
            IReadOnlyParseContext context,
            int position,
            int length,
            bool success,
            T? value,
            IReadOnlyList<IParseError> errors)
        {
            Context = context;
            Position = position;
            Length = length;
            Success = success;
            this.value = value;
            Errors = errors;
        }

        /// <inheritdoc />
        public T Value
        {
            get
            {
                ThrowIfUnsuccessful();
                return value!;
            }
        }

        /// <inheritdoc />
        public int Position { get; }

        /// <inheritdoc />
        public int NextPosition => Position + Length;

        /// <inheritdoc />
        public ParseInputPosition InputStartPosition => Context.Input.GetPosition(Position);

        /// <inheritdoc />
        public ParseInputPosition InputEndPosition => Context.Input.GetPosition(Length == 0 ? Position : NextPosition - 1);

        /// <inheritdoc />
        public bool Success { get; }

        /// <inheritdoc />
        public int Length { get; }

        /// <inheritdoc />
        public IReadOnlyList<IParseError> Errors { get; }

        /// <inheritdoc />
        object? IParseResult.Value => Value;

        /// <inheritdoc />
        public IReadOnlyParseContext Context { get; }

        /// <inheritdoc />
        public override string ToString()
            => Success
            ? Value?.ToString() ?? string.Empty
            : $"Error([{string.Join(", ", Errors)}])";

        /// <inheritdoc />
        public void ThrowIfUnsuccessful()
        {
            if (Success)
            {
                return;
            }

            var throwables = Errors.OfType<Exception>().ToArray();

            throw throwables.Length switch
            {
                0 => new InvalidOperationException("Can not read property Value of non-successful ParseResult."),
                1 => throwables[0],
                _ => new AggregateException(throwables),
            };
        }
    }
}
