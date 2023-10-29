namespace Warpstone.V2;

public sealed class MemoTable : IMemoTable
{
    private readonly Dictionary<int, Dictionary<IParser, IParseResult>> table = new();

    public IParseResult? this[int position, IParser parser]
    {
        get
        {
            if (!table.TryGetValue(position, out var expressions) || !expressions.TryGetValue(parser, out var result))
            {
                return null;
            }

            return result;
        }

        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (!table.TryGetValue(position, out var expressions))
            {
                expressions = new();
                table.Add(position, expressions);
            }

            expressions[parser] = value;
        }
    }

    IParseResult? IReadOnlyMemoTable.this[int position, IParser parser] => this[position, parser];
}
