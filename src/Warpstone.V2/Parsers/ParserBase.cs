using Warpstone.V2.Errors;

namespace Warpstone.V2.Parsers;

public abstract class ParserBase<T> : IParser<T>
{
    public Type ResultType => typeof(T);

    public abstract void Step(IActiveParseContext context, int position, int phase);

    public IParseResult<T> Fail(int position, IEnumerable<IParseError> errors)
        => ParseResult.CreateFailure(this, position, errors);

    IParseResult IParser.Fail(int position, IEnumerable<IParseError> errors)
        => Fail(position, errors);
}
