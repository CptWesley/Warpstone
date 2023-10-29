using System;
using System.Collections.Generic;

namespace Warpstone.ParseState;

/// <summary>
/// Provides read-only access to parse queues.
/// </summary>
public interface IReadOnlyParseQueue : IEnumerable<ParseJobInstance>
{
    /// <summary>
    /// Gets a value indicating whether or not there is an element in the queue.
    /// </summary>
    public bool HasNext { get; }

    /// <summary>
    /// Gets the number of elements in the queue.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Gets the first element in the queue without removing it from the queue.
    /// </summary>
    /// <param name="next">The next element if found.</param>
    /// <returns><c>true</c> if the queue contains any elements, <c>false</c> otherwise.</returns>
    public bool TryPeek(out ParseJobInstance next);

    /// <summary>
    /// Gets the first element in the queue without removing it from the queue.
    /// </summary>
    /// <returns>The next element in the queue.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the queue is empty.</exception>
    public ParseJobInstance Peek();
}
