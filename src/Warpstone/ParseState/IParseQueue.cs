using System;

namespace Warpstone.ParseState;

/// <summary>
/// Provides write access to a parse queue.
/// </summary>
public interface IParseQueue : IReadOnlyParseQueue
{
    /// <summary>
    /// Creates a <see cref="ParseJobInstance"/> and adds it to the queue.
    /// </summary>
    /// <param name="instance">The job instance to enqueue.</param>
    /// <returns>The newly created <see cref="ParseJobInstance"/> instance.</returns>
    public ParseJobInstance Enqueue(ParseJobInstance instance);

    /// <summary>
    /// Creates a <see cref="ParseJobInstance"/> and adds it to the queue.
    /// </summary>
    /// <param name="job">The job.</param>
    /// <param name="data">The data given to the job.</param>
    /// <returns>The newly created <see cref="ParseJobInstance"/> instance.</returns>
    public ParseJobInstance Enqueue(IParseJob job, object? data);

    /// <summary>
    /// Gets the next element of the queue.
    /// </summary>
    /// <returns>The next element of the queue.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the queue is empty.</exception>
    public ParseJobInstance Dequeue();

    /// <summary>
    /// Gets the next element of the queue.
    /// </summary>
    /// <param name="instance">The next element of the queue.</param>
    /// <returns><c>true</c> if the queue was not empty, <c>false</c> otherwise.</returns>
    public bool TryDequeue(out ParseJobInstance instance);
}
