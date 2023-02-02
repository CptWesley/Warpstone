using System;
using System.Collections.Generic;
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
    /// <param name="innerResults">The inner results that lead to this result.</param>
    public ParseResult(IParser<T> parser, IParseError error, IEnumerable<IParseResult> innerResults)
    {
        Error = error;
        Success = false;
        InnerResults = innerResults;
        Parser = parser;
        Position = error.Position;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParseResult{T}"/> class.
    /// </summary>
    /// <param name="parser">The parser that produced the result.</param>
    /// <param name="value">The value.</param>
    /// <param name="input">The input text.</param>
    /// <param name="startPosition">The start position of the parser.</param>
    /// <param name="length">The length of the result.</param>
    /// <param name="innerResults">The inner results that lead to this result.</param>
    public ParseResult(IParser<T> parser, T? value, string input, int startPosition, int length, IEnumerable<IParseResult> innerResults)
    {
        Value = value;
        Success = true;
        InnerResults = innerResults;
        Parser = parser;
        Position = new SourcePosition(input, startPosition, length);
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
    public IEnumerable<IParseResult> InnerResults { get; }

    /// <inheritdoc/>
    public IParser<T> Parser { get; }

    /// <inheritdoc/>
    object? IParseResult.Value => Value;

    /// <inheritdoc/>
    IParser IParseResult.Parser => Parser;

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
