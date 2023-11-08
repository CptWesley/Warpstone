using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Warpstone.V2.Parsers;

namespace Warpstone.V2;

public sealed class MemoTable : IMemoTable
{
    private readonly Dictionary<int, Dictionary<IParser, IParseResult>> table = new();

    public IParseResult? this[(int, IParser) key] => this[key.Item1, key.Item2];

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

    public IEnumerable<(int, IParser)> Keys => table.SelectMany(x => x.Value.Select(y => (x.Key, y.Key)));

    public IEnumerable<IParseResult> Values => table.SelectMany(x => x.Value.Select(y => y.Value));

    public int Count => table.Sum(x => x.Value.Count);

    public bool ContainsKey((int, IParser) key)
        => this[key.Item1, key.Item2] is not null;

    public IEnumerator<KeyValuePair<(int, IParser), IParseResult>> GetEnumerator()
        => table.SelectMany(x => x.Value.Select(y => new KeyValuePair<(int, IParser), IParseResult>((x.Key, y.Key), y.Value)))
            .GetEnumerator();

    public bool TryGetValue((int, IParser) key, [NotNullWhen(true)] out IParseResult? value)
    {
        value = this[key.Item1, key.Item2];
        return value is not null;
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
