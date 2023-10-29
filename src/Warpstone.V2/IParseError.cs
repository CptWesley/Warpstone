namespace Warpstone.V2;

public interface IParseError
{
    public IParsingInput Input { get; }

    public IParser Parser { get; }

    public int Position { get; }

    public int Length { get; }
}
