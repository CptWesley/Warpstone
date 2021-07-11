using System;

namespace Warpstone
{
    /// <summary>
    /// Position of a parsed element in the source code.
    /// </summary>
    public class SourcePosition : IEquatable<SourcePosition>
    {
        private bool upgraded = false;
        private int startLine = -1;
        private int endLine = -1;
        private int startLinePosition = -1;
        private int endLinePosition = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="SourcePosition"/> class.
        /// </summary>
        /// <param name="input">The input text.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public SourcePosition(string input, int start, int end)
        {
            Input = input;
            Start = start;
            End = end;
        }

        /// <summary>
        /// Gets the input.
        /// </summary>
        public string Input { get; }

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
        public int StartLine
        {
            get
            {
                Upgrade();
                return startLine;
            }
        }

        /// <summary>
        /// Gets the end line.
        /// </summary>
        public int EndLine
        {
            get
            {
                Upgrade();
                return endLine;
            }
        }

        /// <summary>
        /// Gets the start line position.
        /// </summary>
        public int StartLinePosition
        {
            get
            {
                Upgrade();
                return startLinePosition;
            }
        }

        /// <summary>
        /// Gets the end line position.
        /// </summary>
        public int EndLinePosition
        {
            get
            {
                Upgrade();
                return endLinePosition;
            }
        }

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

        /// <summary>
        /// Fills the source position with more detailed information.
        /// </summary>
        private void Upgrade()
        {
            if (upgraded)
            {
                return;
            }

            int startLine = 0;
            int startLinePosition = 0;
            int endLine = 0;
            int endLinePosition = 0;
            int startCountdown = Start;
            int endCountdown = End;
            int line = 1;
            int linePosition = 0;

            for (int i = 0; i < Input.Length; i++)
            {
                char c = Input[i];
                linePosition++;

                if (startCountdown >= 0)
                {
                    startLinePosition = linePosition;
                    startLine = line;
                }

                if (endCountdown >= 0)
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
                        if (i + 1 < Input.Length && Input[i + 1] == '\r')
                        {
                            startCountdown--;
                            endCountdown--;
                            i++;
                        }

                        break;
                    case '\r':
                        line++;
                        linePosition = 0;
                        if (i + 1 < Input.Length && Input[i + 1] == '\n')
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

            this.startLine = startLine;
            this.startLinePosition = startLinePosition;
            this.endLine = endLine;
            this.endLinePosition = endLinePosition;
            upgraded = true;
        }
    }
}
