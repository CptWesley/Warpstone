using Warpstone.V2.Parsers;

namespace Warpstone.V2;

public interface IActiveParseContext : IReadOnlyParseContext
{
    public new IMemoTable MemoTable { get; }

    public void Push(IParser parser, int position, int phase);
}
