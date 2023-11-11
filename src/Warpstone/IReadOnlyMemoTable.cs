namespace Warpstone;

public interface IReadOnlyMemoTable : IReadOnlyDictionary<(int, IParser), IParseResult?>
{
    public IParseResult? this[int position, IParser parser] { get; }
}
