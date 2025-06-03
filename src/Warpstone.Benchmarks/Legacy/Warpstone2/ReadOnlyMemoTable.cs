using Legacy.Warpstone2.Parsers;

namespace Legacy.Warpstone2;

/// <summary>
/// Used for wrapping memo-tables to prevent casting.
/// </summary>
public sealed class ReadOnlyMemoTable : IReadOnlyMemoTable
{
    private readonly IReadOnlyMemoTable memo;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyMemoTable"/> class.
    /// </summary>
    /// <param name="memo">The wrapped memo table.</param>
    public ReadOnlyMemoTable(IReadOnlyMemoTable memo)
    {
        this.memo = memo;
    }

    /// <inheritdoc />
    public IParseResult? this[(int, IParser) key] => memo[key];

    /// <inheritdoc />
    public IParseResult? this[int position, IParser parser] => memo[position, parser];

    /// <inheritdoc />
    public IEnumerable<(int, IParser)> Keys => memo.Keys;

    /// <inheritdoc />
    public IEnumerable<IParseResult?> Values => memo.Values;

    /// <inheritdoc />
    public int Count => memo.Count;

    /// <inheritdoc />
    public bool ContainsKey((int, IParser) key)
        => memo.ContainsKey(key);

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<(int, IParser), IParseResult?>> GetEnumerator()
        => memo.GetEnumerator();

    /// <inheritdoc />
    public bool TryGetValue((int, IParser) key, out IParseResult? value)
        => memo.TryGetValue(key, out value);

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => memo.GetEnumerator();
}
