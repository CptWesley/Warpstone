namespace Warpstone;

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
    public UnsafeParseResult this[(int, IParserImplementation) key] => memo[key];

    /// <inheritdoc />
    public UnsafeParseResult this[int position, IParserImplementation parser] => memo[position, parser];

    /// <inheritdoc />
    public IEnumerable<(int Position, IParserImplementation Parser)> Keys => memo.Keys;

    /// <inheritdoc />
    public IEnumerable<UnsafeParseResult> Values => memo.Values;

    /// <inheritdoc />
    public int Count => memo.Count;

    /// <inheritdoc />
    public bool ContainsKey((int Position, IParserImplementation Parser) key)
        => memo.ContainsKey(key);

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<(int Position, IParserImplementation Parser), UnsafeParseResult>> GetEnumerator()
        => memo.GetEnumerator();

    /// <inheritdoc />
    public bool TryGetValue((int Position, IParserImplementation Parser) key, out UnsafeParseResult value)
        => memo.TryGetValue(key, out value);

    /// <inheritdoc />
    public bool TryGetValue(int position, IParserImplementation parser, [NotNullWhen(true)] out UnsafeParseResult value)
        => memo.TryGetValue(position, parser, out value);

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => memo.GetEnumerator();
}
