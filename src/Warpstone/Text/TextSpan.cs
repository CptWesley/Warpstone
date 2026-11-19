using System;
using System.Runtime.Serialization;

namespace Warpstone.Text;

/// <summary>Represents of a span of text.</summary>
[DataContract]
public readonly struct TextSpan : IEquatable<TextSpan>, IComparable<TextSpan>
{
    /// <summary>Initializes a new instance of the <see cref="TextSpan"/> struct.</summary>
    /// <param name="start">Start position of the span.</param>
    /// <param name="length">Length of the span.</param>
    public TextSpan(int start, int length)
    {
        if (start < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(start));
        }

        if (start + length < start)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        Start = start;
        Length = length;
    }

    /// <summary>
    /// Start point of the span.
    /// </summary>
    [DataMember(Order = 0)]
    public int Start { get; }

    /// <summary>
    /// End of the span.
    /// </summary>
    public int End => Start + Length;

    /// <summary>
    /// Length of the span.
    /// </summary>
    [DataMember(Order = 1)]
    public int Length { get; }

    /// <summary>
    /// Determines whether or not the span is empty.
    /// </summary>
    public bool IsEmpty => Length == 0;

    /// <summary>
    /// Determines if two instances of <see cref="TextSpan"/> are the same.
    /// </summary>
    /// <param name="left">Left instance to compare.</param>
    /// <param name="right">Right instance to compare.</param>
    public static bool operator ==(TextSpan left, TextSpan right) => left.Equals(right);

    /// <summary>
    /// Determines if two instances of <see cref="TextSpan"/> are different.
    /// </summary>
    /// <param name="left">Left instance to compare.</param>
    /// <param name="right">Right instance to compare.</param>
    public static bool operator !=(TextSpan left, TextSpan right) => !left.Equals(right);

    /// <inheritdoc />
    public bool Equals(TextSpan other)
        => Start == other.Start
        && Length == other.Length;

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is TextSpan span && Equals(span);

    /// <inheritdoc />
    public override int GetHashCode() => Start ^ (Length << 16);

    /// <inheritdoc />
    public override string ToString() => $"[{Start}..{End})";

    /// <inheritdoc />
    public int CompareTo(TextSpan other)
    {
        var diff = Start - other.Start;
        return diff is 0
            ? Length - other.Length
            : diff;
    }
}
