using System.Collections.Immutable;
using Warpstone.V2.Internal;
using Warpstone.V2.Parsers;

namespace Warpstone.V2.Alt;

public sealed class AltParseContext
{
    private readonly MemoTable memo = new();
    private readonly FlagTable growing = new();
    private readonly IParseInput input;

    public AltParseContext(string input)
    {
        this.input = new ParseInput(input);
    }

    public IParseResult<T> Parse<T>(IParser<T> parser, int p)
    {
        lock (memo)
        {
            var result = ApplyRule(parser, p);
            Log($"Final result ({result.Length}): {result}");
            return (IParseResult<T>)result;
        }
    }

    private IParseResult ApplyRule(IParser parser, int p)
    {
        Log($"ApplyRule({parser}, {p})");
        if (memo[p, parser] is not { } m)
        {
            memo[p, parser] = parser.Fail(p, input);
            m = Eval(parser, p);
            memo[p, parser] = m;
            if (growing[p, parser])
            {
                GrowLR(parser, p);
                growing[p, parser] = false;
                m = memo[p, parser]!;
            }

            return m;
        }

        if (m.Status == ParseStatus.Fail)
        {
            m = parser.Mismatch(p, m.Errors);
            memo[p, parser] = m;
            growing[p, parser] = true;
            return m;
        }

        return m;
    }

    private IParseResult ApplyRuleGrow(IParser parser, int p, IImmutableSet<IParser> limits)
    {
        Log($"ApplyRuleGrow({parser}, {p}, {{{string.Join(", ", limits)}}})");
        limits = limits.Add(parser);
        var ans = EvalGrow(parser, p, limits);

        if (memo[p, parser] is not { } prev)
        {
            throw new InvalidOperationException();
        }

        if (ans.Status != ParseStatus.Match || ans.NextPosition <= prev.NextPosition)
        {
            return prev;
        }

        memo[p, parser] = ans;
        return ans;
    }

    private void GrowLR(IParser parser, int p)
    {
        var prevPos = -1;
        while (true)
        {
            var ans = EvalGrow(parser, p, ImmutableHashSet.Create(parser));
            if (ans.Status != ParseStatus.Match || ans.NextPosition <= prevPos)
            {
                break;
            }

            memo[p, parser] = ans;
            prevPos = ans.NextPosition;
        }
    }

    private IParseResult Eval(IParser parser, int position)
        => parser.Eval(input, position, ApplyRule);

    private IParseResult EvalGrow(IParser parser, int position, IImmutableSet<IParser> limits)
        => parser.Eval(input, position, (calledParser, calledPosition) =>
        {
            if (calledPosition == position && !limits.Contains(calledParser))
            {
                return ApplyRuleGrow(calledParser, calledPosition, limits);
            }

            return ApplyRule(calledParser, calledPosition);
        });

    private void Log(string message)
    {
        Console.WriteLine(message);

        foreach (var entry in memo)
        {
            Console.WriteLine($"[{entry.Key.Item1}, {entry.Key.Item2}] = {entry.Value}");
        }

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
    }
}
