namespace Warpstone.ParseState;

/// <summary>
/// Represents a queued job.
/// </summary>
/// <param name="Job">The queued job.</param>
/// <param name="Data">The data supplied to the queued job.</param>
public record struct ParseJobInstance(IParseJob Job, object? Data)
{
}
