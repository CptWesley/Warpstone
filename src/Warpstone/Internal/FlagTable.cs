namespace Warpstone.Internal;

/// <summary>
/// Represents a table that holds boolean flags.
/// </summary>
internal sealed class FlagTable
{
    private readonly Dictionary<int, HashSet<IParser>> table = new();

    /// <summary>
    /// Gets or sets the flag at the given <paramref name="position"/> and <paramref name="parser"/>.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="parser">The parser.</param>
    /// <returns><c>true</c> if the flag was set, <c>false</c> otherwise.</returns>
    public bool this[int position, IParser parser]
    {
        get => table.TryGetValue(position, out var expressions) && expressions.Contains(parser);
        set
        {
            if (value)
            {
                Enable(position, parser);
            }
            else
            {
                Disable(position, parser);
            }
        }
    }

    private void Enable(int position, IParser parser)
    {
        if (!table.TryGetValue(position, out var expressions))
        {
            expressions = new();
            table[position] = expressions;
        }

        expressions.Add(parser);
    }

    private void Disable(int position, IParser parser)
    {
        if (table.TryGetValue(position, out var expressions))
        {
            expressions.Remove(parser);
            if (expressions.Count == 0)
            {
                table.Remove(position);
            }
        }
    }
}
