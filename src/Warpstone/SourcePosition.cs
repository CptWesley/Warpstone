using System;

namespace Warpstone
{
    /// <summary>
    /// Position of a parsed element in the source code.
    /// </summary>
    public class SourcePosition : IEquatable<SourcePosition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourcePosition"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public SourcePosition(int start, int end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Gets the start position.
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// Gets the end.
        /// </summary>
        public int End { get; }

        /// <summary>
        /// Gets the start line.
        /// </summary>
        public int StartLine { get; private set; } = -1;

        /// <summary>
        /// Gets the end line.
        /// </summary>
        public int EndLine { get; private set; } = -1;

        /// <summary>
        /// Gets the start line position.
        /// </summary>
        public int StartLinePosition { get; private set; } = -1;

        /// <summary>
        /// Gets the end line position.
        /// </summary>
        public int EndLinePosition { get; private set; } = -1;

        /// <summary>
        /// Gets a value indicating whether this <see cref="SourcePosition"/> is detailed.
        /// </summary>
        public bool Upgraded { get; private set; } = false;

        /// <summary>
        /// Gets the length of the parsed part in the source.
        /// </summary>
        public int Length => End - Start + 1;

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(SourcePosition left, SourcePosition right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

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
            => !(other is null) && Start == other.Start && Length == other.Length;

        /// <inheritdoc/>
        public override int GetHashCode()
            => Start * Length;

        /// <summary>
        /// Fills the source position with more detailed information.
        /// </summary>
        /// <param name="input">The input.</param>
        public void Upgrade(string input)
        {
            int startLine = 0;
            int startLinePosition = 0;
            int endLine = 0;
            int endLinePosition = 0;
            int startCountdown = Start;
            int endCountdown = End;
            int line = 1;
            int linePosition = 0;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                linePosition++;

                if (startCountdown == 0)
                {
                    startLinePosition = linePosition;
                    startLine = line;
                }

                if (endCountdown == 0)
                {
                    endLinePosition = linePosition;
                    endLine = line;
                }

                if (startCountdown < 0 && endCountdown < 0)
                {
                    break;
                }

                switch (c)
                {
                    case '\n':
                        line++;
                        linePosition = 0;
                        if (i + 1 < input.Length && input[i + 1] == '\r')
                        {
                            startCountdown--;
                            endCountdown--;
                            i++;
                        }

                        break;
                    case '\r':
                        line++;
                        linePosition = 0;
                        if (i + 1 < input.Length && input[i + 1] == '\n')
                        {
                            startCountdown--;
                            endCountdown--;
                            i++;
                        }

                        break;
                    default:
                        break;
                }

                startCountdown--;
                endCountdown--;
            }

            StartLine = startLine;
            StartLinePosition = startLinePosition;
            EndLine = endLine;
            EndLinePosition = endLinePosition;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (StartLine == EndLine)
            {
                if (StartLinePosition == EndLinePosition)
                {
                    return $"{StartLine}:{StartLinePosition}";
                }

                return $"{StartLine}:{StartLinePosition}-{EndLinePosition}";
            }

            return $"{StartLine}:{StartLinePosition}-{EndLine}:{EndLinePosition}";
        }
    }
}
