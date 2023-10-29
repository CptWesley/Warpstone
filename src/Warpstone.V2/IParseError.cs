namespace Warpstone.V2;

public interface IParseError
{
    public IParseInput Input { get; }

    public IParser Parser { get; }

    public int Position { get; }

    public int Length { get; }
}
