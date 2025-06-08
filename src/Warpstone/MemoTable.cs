namespace Warpstone;

/// <summary>
/// Represents a packrat memotable.
/// </summary>
public sealed class MemoTable : IMemoTable
{
    private readonly Dictionary<int, Dictionary<IParser, UnsafeParseResult>> table = new();

    /// <inheritdoc />
    public UnsafeParseResult this[(int, IParser) key] => this[key.Item1, key.Item2];

    /// <inheritdoc />
    public UnsafeParseResult this[int position, IParser parser]
    {
        get
        {
            if (!table.TryGetValue(position, out var expressions) || !expressions.TryGetValue(parser, out var result))
            {
                throw new KeyNotFoundException("Could not find entry in memo table.");
            }

            return result;
        }

        set
        {
            if (!table.TryGetValue(position, out var expressions))
            {
                expressions = new();
                table.Add(position, expressions);
            }

            expressions[parser] = value;
        }
    }

    /// <inheritdoc />
    UnsafeParseResult IReadOnlyMemoTable.this[int position, IParser parser] => this[position, parser];

    /// <inheritdoc />
    public IEnumerable<(int, IParser)> Keys => table.SelectMany(x => x.Value.Select(y => (x.Key, y.Key)));

    /// <inheritdoc />
    public IEnumerable<UnsafeParseResult> Values => table.SelectMany(x => x.Value.Select(y => y.Value));

    /// <inheritdoc />
    public int Count => table.Sum(x => x.Value.Count);

    /// <inheritdoc />
    public bool ContainsKey((int Position, IParser Parser) key)
        => TryGetValue(key, out _);

    /// <inheritdoc />
    public bool TryGetValue((int Position, IParser Parser) key, [NotNullWhen(true)] out UnsafeParseResult value)
        => TryGetValue(key.Position, key.Parser, out value);

    /// <inheritdoc />
    public bool TryGetValue(int position, IParser parser, [NotNullWhen(true)] out UnsafeParseResult value)
    {
        if (!table.TryGetValue(position, out var expressions) || !expressions.TryGetValue(parser, out value))
        {
            value = default!;
            return false;
        }

        return true;
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<(int Position, IParser Parser), UnsafeParseResult>> GetEnumerator()
        => table.SelectMany(x => x.Value.Select(y => new KeyValuePair<(int, IParser), UnsafeParseResult>((x.Key, y.Key), y.Value)))
            .GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
