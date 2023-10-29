namespace Warpstone.V2;

public interface IParser
{
    void Step(IActiveParsingContext context, int position, int phase);
}

public interface IParser<T> : IParser
{

}
