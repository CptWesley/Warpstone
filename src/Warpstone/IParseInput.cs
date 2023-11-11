namespace Warpstone;

public interface IParseInput
{
    public string Input { get; }

    public IParseInputSource Source { get; }
}
