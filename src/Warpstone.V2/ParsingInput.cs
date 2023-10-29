namespace Warpstone.V2;

public sealed class ParsingInput : IParsingInput
{
    public ParsingInput(IParsingInputSource source, string input)
    {
        Source = source;
        Input = input;
    }

    public ParsingInput(string input)
        : this(FromMemorySource.Instance, input)
    {
    }

    public string Input { get; }

    public IParsingInputSource Source { get; }
}
