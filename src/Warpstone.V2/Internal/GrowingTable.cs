using Warpstone.V2.Parsers;

namespace Warpstone.V2.Internal;

internal sealed class GrowingTable
{
    private readonly Dictionary<int, HashSet<IParser>> table = new();

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
