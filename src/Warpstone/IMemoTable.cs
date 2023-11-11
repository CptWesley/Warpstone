namespace Warpstone;

public interface IMemoTable : IReadOnlyMemoTable
{
    public new IParseResult? this[int position, IParser parser] { get; set; }
}
