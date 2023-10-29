namespace Warpstone.V2;

public interface IParseInput
{
    public string Input { get; }

    public IParseInputSource Source { get; }
}
