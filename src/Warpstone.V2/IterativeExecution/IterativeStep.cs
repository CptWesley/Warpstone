namespace Warpstone.V2.IterativeExecution;

public readonly record struct IterativeStep
{
    public required IterativeStepType Type { get; init; }

    public required object? Value { get; init; }

    public required Func<IterativeStep>? First { get; init; }

    public required Func<object?, IterativeStep>? More { get; init; }
}
