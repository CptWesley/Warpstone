using System;
using System.Collections;
using System.Collections.Generic;

namespace Warpstone.ParseState;

/// <summary>
/// Provides an implementation for the <see cref="IParseQueue"/> interface.
/// </summary>
public sealed class ParseQueue : IParseQueue
{
    private readonly List<ParseJobInstance> queue = new List<ParseJobInstance>();

    /// <inheritdoc/>
    public bool HasNext => Count > 0;

    /// <inheritdoc/>
    public int Count => queue.Count;

    /// <inheritdoc/>
    public ParseJobInstance Enqueue(ParseJobInstance instance)
    {
        queue.Add(instance);
        return instance;
    }

    /// <inheritdoc/>
    public ParseJobInstance Enqueue(IParseJob job, object? data)
        => Enqueue(new ParseJobInstance(job, data));

    /// <inheritdoc/>
    public ParseJobInstance Dequeue()
    {
        if (!TryDequeue(out ParseJobInstance next))
        {
            throw new InvalidOperationException("Queue is empty.");
        }

        return next;
    }

    /// <inheritdoc/>
    public bool TryDequeue(out ParseJobInstance instance)
    {
        if (!TryPeek(out instance))
        {
            return false;
        }

        queue.RemoveAt(queue.Count - 1);
        return true;
    }

    /// <inheritdoc/>
    public ParseJobInstance Peek()
    {
        if (!TryPeek(out ParseJobInstance next))
        {
            throw new InvalidOperationException("Queue is empty.");
        }

        return next;
    }

    /// <inheritdoc/>
    public bool TryPeek(out ParseJobInstance next)
    {
        if (!HasNext)
        {
            next = default;
            return false;
        }

        next = queue[queue.Count - 1];
        return true;
    }

    /// <inheritdoc/>
    public IEnumerator<ParseJobInstance> GetEnumerator()
        => queue.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
