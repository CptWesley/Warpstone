using Warpstone.V2.Parsers;

namespace Warpstone.V2;

public interface IReadOnlyMemoTable : IReadOnlyDictionary<(int, IParser), IParseResult?>
{
    public IParseResult? this[int position, IParser parser] { get; }
}
