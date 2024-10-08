using BenchmarkDotNet.Attributes;
using DotNetProjectFile.Resx;
using Legacy.Warpstone.Parsers;
using Pidgin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warpstone.Parsers;

using BasicParsers1 = Legacy.Warpstone.Parsers.BasicParsers;
using BasicParsers2 = Warpstone.Parsers.BasicParsers;

namespace Warpstone.Benchmarks;

[MemoryDiagnoser(true)]
public class RightRecursiveSimpleString
{
    private static readonly IParser<string> Warpstone2Parser
        = BasicParsers2.Or(BasicParsers2.String("a").ThenAdd(BasicParsers2.Lazy(() => Warpstone2Parser!)).Transform((x, y) => x + y), BasicParsers2.End);

    private static readonly Legacy.Warpstone.IParser<string> Warpstone1Parser
        = BasicParsers1.Or(BasicParsers1.String("a").ThenAdd(BasicParsers1.Lazy(() => Warpstone1Parser!)).Transform((x, y) => x + y), BasicParsers1.End.Transform(_ => string.Empty));

    private static readonly Pidgin.Parser<char, string> PidginParser
        = Pidgin.Parser.Map((x, y) => x + y, Pidgin.Parser.String("a"), Pidgin.Parser.Rec(() => PidginParser!)).Or(Pidgin.Parser<char>.End.Map(_ => string.Empty));

    [Params(1, 10, 100, 1_000, 10_000, 100_000)]
    public int N;

    private string input = string.Empty;

    [GlobalSetup]
    public void Setup()
    {
        input = new string('a', N);
    }

    [Benchmark]
    public string Warpstone2_benchmark()
    {
        return Warpstone2Parser.Parse(input);
    }

    [Benchmark]
    public string Warpstone1_benchmark()
    {
        return Warpstone1Parser.Parse(input);
    }

    [Benchmark]
    public string Pidgin_benchmark()
    {
        return PidginParser.Parse(input).Value;
    }
}
