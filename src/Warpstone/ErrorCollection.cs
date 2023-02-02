using System;
using System.Collections.Generic;
using System.Linq;

namespace Warpstone;

/// <summary>
/// Represents a collection of parse errors.
/// </summary>
public class ErrorCollection : IParseError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorCollection"/> class.
    /// </summary>
    /// <param name="errors">The errors inside the collection.</param>
    public ErrorCollection(IEnumerable<IParseError> errors)
    {
        Errors = errors.ToList();

        int minStart = Errors.Min(x => x.Position.Start);
        int maxEnd = Errors.Max(x => x.Position.End);
        string input = Errors.First().Position.Input;
        Position = new SourcePosition(input, minStart, maxEnd);
    }

    /// <summary>
    /// Gets the errors inside the collection.
    /// </summary>
    public IReadOnlyList<IParseError> Errors { get; }

    /// <inheritdoc/>
    public SourcePosition Position { get; }

    /// <inheritdoc/>
    public string GetMessage()
        => $"[{string.Join(Environment.NewLine, Errors.Select(x => x.GetMessage()))}]";
}
