namespace Warpstone.ParseState;

/// <summary>
/// Provides an interface for parse jobs.
/// </summary>
public interface IParseJob : IReadOnlyParseJob
{
    /// <summary>
    /// Gets or sets the parse result.
    /// </summary>
    public new IParseResult? Result { get; set; }
}

/// <summary>
/// Provides an interface for parse jobs.
/// </summary>
/// <typeparam name="T">The output type of the parser.</typeparam>
public interface IParseJob<T> : IParseJob, IReadOnlyParseJob<T>
{
    /// <summary>
    /// Gets or sets the parse result.
    /// </summary>
    public new IParseResult<T>? Result { get; set; }
}