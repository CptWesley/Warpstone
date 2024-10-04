namespace Warpstone;

public interface IParseInstruction
{
    public void Execute();
}

public abstract class ParseInstruction : IParseInstruction
{
    public required IReadOnlyParseContext Context { get; init; }

    public required int Position { get; init; }

    public required IParseStack Stack { get; init; }

    public abstract void Execute();
}

public sealed class ApplyParserInstruction : ParseInstruction
{
    public required IParser Parser { get; init; }

    public override void Execute()
        => Parser.Eval(Context, Position, Stack);

    public override string ToString()
        => $"Apply {Parser} @ {Position}";
}
