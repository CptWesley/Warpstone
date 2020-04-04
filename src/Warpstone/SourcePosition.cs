using System;

namespace Warpstone
{
    /// <summary>
    /// Position of a parsed element in the source code.
    /// </summary>
    public struct SourcePosition : IEquatable<SourcePosition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourcePosition"/> struct.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="length">The length.</param>
        public SourcePosition(int start, int length)
        {
            Start = start;
            Length = length;
        }

        /// <summary>
        /// Gets or sets the start position.
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Gets or sets the length of the parsed part in the source.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(SourcePosition left, SourcePosition right)
            => left.Equals(right);

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(SourcePosition left, SourcePosition right)
            => !(left == right);

        /// <inheritdoc/>
        public override bool Equals(object obj)
            => obj is SourcePosition sp ? Equals(sp) : false;

        /// <inheritdoc/>
        public bool Equals(SourcePosition other)
            => Start == other.Start && Length == other.Length;

        /// <inheritdoc/>
        public override int GetHashCode()
            => Start * Length;

        /// <inheritdoc/>
        public override string ToString()
            => $"{Start}:{Length}";
    }
}
