#nullable enable

using Legacy.Warpstone2.Internal;
using Legacy.Warpstone2.Parsers;

namespace Legacy.Warpstone2;

/// <summary>
/// Represents a packrat memotable.
/// </summary>
public sealed class MemoTable : IMemoTable
{
    private readonly Dictionary<int, Dictionary<IParser, IParseResult>> table = new();

    /// <inheritdoc />
    public IParseResult? this[(int, IParser) key] => this[key.Item1, key.Item2];

    /// <inheritdoc />
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

    /// <inheritdoc />
    IParseResult? IReadOnlyMemoTable.this[int position, IParser parser] => this[position, parser];

    /// <inheritdoc />
    public IEnumerable<(int, IParser)> Keys => table.SelectMany(x => x.Value.Select(y => (x.Key, y.Key)));

    /// <inheritdoc />
    public IEnumerable<IParseResult> Values => table.SelectMany(x => x.Value.Select(y => y.Value));

    /// <inheritdoc />
    public int Count => table.Sum(x => x.Value.Count);

    /// <inheritdoc />
    [MethodImpl(InternalMethodImplOptions.InlinedOptimized)]
    public bool ContainsKey((int, IParser) key)
        => this[key.Item1, key.Item2] is not null;

    /// <inheritdoc />
    [MethodImpl(InternalMethodImplOptions.InlinedOptimized)]
    public bool TryGetValue((int, IParser) key, [NotNullWhen(true)] out IParseResult? value)
    {
        value = this[key.Item1, key.Item2];
        return value is not null;
    }

    /// <inheritdoc />
    [MethodImpl(InternalMethodImplOptions.InlinedOptimized)]
    public IEnumerator<KeyValuePair<(int, IParser), IParseResult?>> GetEnumerator()
        => table.SelectMany(x => x.Value.Select(y => new KeyValuePair<(int, IParser), IParseResult?>((x.Key, y.Key), y.Value)))
            .GetEnumerator();

    /// <inheritdoc />
    [MethodImpl(InternalMethodImplOptions.InlinedOptimized)]
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
