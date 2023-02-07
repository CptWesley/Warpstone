using System;
using System.Diagnostics.CodeAnalysis;

namespace Warpstone;

/// <summary>
/// Object representing the parsing result.
/// </summary>
/// <typeparam name="T">The output type of the parse process.</typeparam>
public class ParseResult<T> : IParseResult<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParseResult{T}"/> class.
    /// </summary>
    /// <param name="parser">The parser that produced the result.</param>
    /// <param name="error">The parse error that occured.</param>
    /// <param name="next">The position in the input where the parser has to continue.</param>
    public ParseResult(IParser<T> parser, IParseError error, int next)
    {
        Error = error;
        Success = false;
        Parser = parser;
        Position = error.Position;
        Next = next;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParseResult{T}"/> class.
    /// </summary>
    /// <param name="parser">The parser that produced the result.</param>
    /// <param name="value">The value.</param>
    /// <param name="position">The position of the result in the input.</param>
    /// <param name="next">The position in the input where the parser has to continue.</param>
    public ParseResult(IParser<T> parser, T? value, SourcePosition position, int next)
    {
        Value = value;
        Success = true;
        Parser = parser;
        Position = position;
        Next = next;
    }

    /// <inheritdoc/>
    public Type OutputType => Parser.OutputType;

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool Success { get; }

    /// <inheritdoc/>
    public T? Value { get; }

    /// <summary>
    /// Gets the position of the parse result.
    /// </summary>
    public SourcePosition Position { get; }

    /// <summary>
    /// Gets the input of the result.
    /// </summary>
    public string Input => Position.Input;

    /// <summary>
    /// Gets the start position of the result.
    /// </summary>
    public int Start => Position.Start;

    /// <summary>
    /// Gets the end position of the result.
    /// </summary>
    public int End => Position.End;

    /// <summary>
    /// Gets the length of the result.
    /// </summary>
    public int Length => Position.Length;

    /// <inheritdoc/>
    public IParseError? Error { get; }

    /// <inheritdoc/>
    public IParser<T> Parser { get; }

    /// <inheritdoc/>
    object? IParseResult.Value => Value;

    /// <inheritdoc/>
    IParser IParseResult.Parser => Parser;

    /// <inheritdoc/>
    public int Next { get; }

    /// <inheritdoc/>
    public override string ToString()
        => ToString(2);

    /// <inheritdoc/>
    public string ToString(int depth)
    {
        if (Success)
        {
            return $"Value from {Parser.ToString(depth)} at {Position}: {Value!}";
        }

        return $"Error from {Parser.ToString(depth)} at {Position}: {Error.GetMessage()}";
    }
}
