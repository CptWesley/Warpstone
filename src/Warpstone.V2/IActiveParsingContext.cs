namespace Warpstone.V2;

public interface IActiveParsingContext : IReadOnlyParsingContext
{
    public new IMemoTable MemoTable { get; }

    public void Push(IParser parser, int position, int phase);
}
