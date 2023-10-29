using Warpstone.V2.Errors;

namespace Warpstone.V2.Parsers;

public interface IParser
{
    public Type ResultType { get; }

    public void Step(IActiveParseContext context, int position, int phase);

    public IParseResult Fail(int position, IEnumerable<IParseError> errors);
}

public interface IParser<T> : IParser
{
    public new IParseResult<T> Fail(int position, IEnumerable<IParseError> errors);
}
