using System.Diagnostics.CodeAnalysis;

namespace Warpstone.ParseState;

/// <summary>
/// Provides a read-only interface for parse jobs.
/// </summary>
public interface IReadOnlyParseJob
{
    /// <summary>
    /// Gets the parser for the job.
    /// </summary>
    public IParser Parser { get; }

    /// <summary>
    /// Gets the starting position of the job.
    /// </summary>
    public int StartingPosition { get; }

    /// <summary>
    /// Gets the maximum length of the input parsed by this job.
    /// </summary>
    public int MaxLength { get; }

    /// <summary>
    /// Gets the parse result.
    /// </summary>
    public IParseResult? Result { get; }

    /// <summary>
    /// Gets a value indicating whether or not the job was done.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Result))]
    public bool Done { get; }

    /// <summary>
    /// Gets a value indicating whether or not the job was successful.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Result))]
    public bool Success { get; }

    /// <summary>
    /// Gets a value indicating whether or not the job failed.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Result))]
    public bool Failure { get; }
}

/// <summary>
/// Provides a read-only interface for parse jobs.
/// </summary>
/// <typeparam name="T">The return type of the parser.</typeparam>
public interface IReadOnlyParseJob<T> : IReadOnlyParseJob
{
    /// <summary>
    /// Gets the parser for the job.
    /// </summary>
    public new IParser<T> Parser { get; }

    /// <summary>
    /// Gets the parse result.
    /// </summary>
    public new IParseResult<T>? Result { get; }
}
