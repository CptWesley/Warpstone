using Warpstone.V2.Parsers;

namespace Warpstone.V2.Errors;

public interface IParseError
{
    public IParseInput Input { get; }

    public IParser Parser { get; }

    public int Position { get; }

    public int Length { get; }
}
