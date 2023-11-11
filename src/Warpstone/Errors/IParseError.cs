namespace Warpstone.Errors;

public interface IParseError
{
    public IParseInput Input { get; }

    public IParser Parser { get; }

    public int Position { get; }

    public int Length { get; }
}
