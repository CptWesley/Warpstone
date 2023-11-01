namespace Warpstone.V2.Parsers;

public abstract class ParserBase<T> : IParser<T>
{
    public Type ResultType => typeof(T);

    public abstract void Step(IActiveParseContext context, int position, int phase);

    public IParseResult<T> Fail(int position, IParseInput input)
        => ParseResult.CreateFail(this, position, input);

    IParseResult IParser.Fail(int position, IParseInput input)
        => Fail(position, input);
}
