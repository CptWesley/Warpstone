#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high

using System;

namespace Legacy.Warpstone1
{
    /// <summary>
    /// Position of a parsed element in the source code.
    /// </summary>
    public struct SourcePosition : IEquatable<SourcePosition>
    {
        private UpgradedSourcePosition? upgraded;

        /// <summary>
        /// Initializes a new instance of the <see cref="SourcePosition"/> struct.
        /// </summary>
        /// <param name="input">The input text.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public SourcePosition(string input, int start, int end)
        {
            Input = input;
            Start = start;
            End = end;

            upgraded = null;
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
                return upgraded!.Value.StartLine;
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
                return upgraded!.Value.EndLine;
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
                return upgraded!.Value.StartLinePosition;
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
                return upgraded!.Value.EndLinePosition;
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
            if (upgraded.HasValue)
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

            upgraded = new UpgradedSourcePosition(startLine, endLine, startLinePosition, endLinePosition);
        }

        private struct UpgradedSourcePosition
        {
            public readonly int StartLine;
            public readonly int EndLine;
            public readonly int StartLinePosition;
            public readonly int EndLinePosition;

            public UpgradedSourcePosition(int startLine, int endLine, int startLinePosition, int endLinePosition)
            {
                StartLine = startLine;
                EndLine = endLine;
                StartLinePosition = startLinePosition;
                EndLinePosition = endLinePosition;
            }
        }
    }
}
