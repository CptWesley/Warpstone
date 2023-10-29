using System;
using System.Diagnostics.CodeAnalysis;

namespace Warpstone.ParseState;

/// <summary>
/// Represents a parse job.
/// </summary>
/// <typeparam name="T">The return type of the parser.</typeparam>
public class ParseJob<T> : IParseJob<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParseJob{T}"/> class.
    /// </summary>
    /// <param name="parser">The parser used for the job.</param>
    /// <param name="startingPosition">The starting position of the job.</param>
    /// <param name="maxLength">The maximum number of characters in the input that can be consumed by this job.</param>
    public ParseJob(IParser<T> parser, int startingPosition, int maxLength)
    {
        Parser = parser;
        StartingPosition = startingPosition;
        MaxLength = maxLength;
    }

    /// <inheritdoc/>
    public IParseResult<T>? Result { get; set; }

    /// <inheritdoc/>
    public IParser<T> Parser { get; }

    /// <inheritdoc/>
    public int StartingPosition { get; }

    /// <inheritdoc/>
    public int MaxLength { get; }

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(Result))]
    public bool Done => Result is not null;

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(Result))]
    public bool Success => Done && Result.Success;

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(Result))]
    public bool Failure => Done && !Result.Success;

    /// <inheritdoc/>
    IParseResult? IParseJob.Result
    {
        get => Result;
        set
        {
            if (value is not IParseResult<T> typedValue)
            {
                throw new InvalidOperationException($"Expected parse result of type '{typeof(T).FullName}'.");
            }

            Result = typedValue;
        }
    }

    /// <inheritdoc/>
    IParseResult<T>? IReadOnlyParseJob<T>.Result => Result;

    /// <inheritdoc/>
    IParseResult? IReadOnlyParseJob.Result => Result;

    /// <inheritdoc/>
    IParser IReadOnlyParseJob.Parser => Parser;
}
