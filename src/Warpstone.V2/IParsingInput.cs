namespace Warpstone.V2;

public interface IParsingInput
{
    public string Input { get; }

    public IParsingInputSource Source { get; }
}
