using System;
using System.Diagnostics.CodeAnalysis;

namespace Warpstone;

/// <summary>
/// Object representing the parsing result.
/// </summary>
/// <typeparam name="T">The output type of the parse process.</typeparam>
public interface IParseResult<out T> : IParseResult
{
    /// <summary>
    /// Gets a value indicating whether the parsing was success.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    new bool Success { get; }

    /// <summary>
    /// Gets the parsed value.
    /// </summary>
    new T? Value { get; }

    /// <summary>
    /// Gets the parse error.
    /// </summary>
    new IParseError? Error { get; }

    /// <summary>
    /// Gets the parser that produced this result.
    /// </summary>
    new IParser<T> Parser { get; }
}

/// <summary>
/// Object representing the parsing result.
/// </summary>
public interface IParseResult : IBoundedToString
{
    /// <summary>
    /// Gets a value indicating whether the parsing was success.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    bool Success { get; }

    /// <summary>
    /// Gets the parsed value.
    /// </summary>
    object? Value { get; }

    /// <summary>
    /// Gets the position of the parse result.
    /// </summary>
    SourcePosition Position { get; }

    /// <summary>
    /// Gets the input of the result.
    /// </summary>
    public string Input { get; }

    /// <summary>
    /// Gets the start position of the result.
    /// </summary>
    public int Start { get; }

    /// <summary>
    /// Gets the end position of the result.
    /// </summary>
    public int End { get; }

    /// <summary>
    /// Gets the next position for the parser.
    /// </summary>
    public int Next { get; }

    /// <summary>
    /// Gets the length of the result.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Gets the parse error.
    /// </summary>
    IParseError? Error { get; }

    /// <summary>
    /// Gets the parser that produced this result.
    /// </summary>
    IParser Parser { get; }

    /// <summary>
    /// Gets the output type of the parser.
    /// </summary>
    Type OutputType { get; }
}
