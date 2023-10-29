namespace Warpstone.V2;

public sealed class ParseInput : IParseInput
{
    public ParseInput(IParseInputSource source, string input)
    {
        Source = source;
        Input = input;
    }

    public ParseInput(string input)
        : this(FromMemorySource.Instance, input)
    {
    }

    public string Input { get; }

    public IParseInputSource Source { get; }
}
