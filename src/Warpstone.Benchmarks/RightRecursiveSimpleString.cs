using BenchmarkDotNet.Attributes;
using DotNetProjectFile.Resx;
using Legacy.Warpstone1.Parsers;
using Legacy.Warpstone2;
using Legacy.Warpstone2.Parsers;
using Parlot.Fluent;
using ParsecSharp;
using Pidgin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BasicParsers1 = Legacy.Warpstone1.Parsers.BasicParsers;
using BasicParsers2 = Legacy.Warpstone2.Parsers.BasicParsers;

namespace Warpstone.Benchmarks;

[MemoryDiagnoser(true)]
[ReturnValueValidator(failOnError: false)]
public class RightRecursiveSimpleString
{
    private static readonly Legacy.Warpstone2.Parsers.IParser<string> Warpstone2Parser
        = BasicParsers2.Or(BasicParsers2.String("a").ThenAdd(BasicParsers2.Lazy(() => Warpstone2Parser!)).Transform((x, y) => x + y), BasicParsers2.End);

    private static readonly Legacy.Warpstone1.IParser<string> Warpstone1Parser
        = BasicParsers1.Or(BasicParsers1.String("a").ThenAdd(BasicParsers1.Lazy(() => Warpstone1Parser!)).Transform((x, y) => x + y), BasicParsers1.End.Transform(_ => string.Empty));

    private static readonly Pidgin.Parser<char, string> PidginParser
        = Pidgin.Parser.Map((x, y) => x + y, Pidgin.Parser.String("a"), Pidgin.Parser.Rec(() => PidginParser!)).Or(Pidgin.Parser<char>.End.Map(_ => string.Empty));

    private static readonly ParsecSharp.IParser<char, string> ParsecParser
        = ParsecSharp.Parser.Fix<char, string>(value =>
        {
            var a = ParsecSharp.Text.String("a");
            var concat = ParsecSharp.Parser.Append(a, value);
            return ParsecSharp.Parser.Choice(concat, ParsecSharp.Text.EndOfInput().Map(_ => string.Empty));
        });

    private static readonly Parlot.Fluent.Parser<string> ParlotParser = CreateParlotParser();
    private static readonly Parlot.Fluent.Parser<string> ParlotCompiledParser = CreateParlotParser().Compile();

    [Params(10, 1_000, 100_000)]
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

    [Benchmark(Baseline = true)]
    public string Pidgin_benchmark()
    {
        return PidginParser.Parse(input).Value;
    }

    [Benchmark]
    public string Parsec_benchmark()
    {
        return ParsecParser.Parse(input).Value;
    }

    [Benchmark]
    public string Parlot_benchmark()
    {
        return ParlotParser.Parse(input)!;
    }

    [Benchmark]
    public string Parlot_compiled_benchmark()
    {
        return ParlotCompiledParser.Parse(input)!;
    }

    private static Parlot.Fluent.Parser<string> CreateParlotParser()
    {
        var parser = Parlot.Fluent.Parsers.Deferred<string>();

        var a = Parlot.Fluent.Parsers.Literals.Text("a");

        var concat = Parlot.Fluent.Parsers.And(a, parser).Then(x => x.Item1 + x.Item2);
        var end = Parlot.Fluent.Parsers.Literals.Text("");
        var either = Parlot.Fluent.Parsers.Or(concat, end);

        parser.Parser = either;

        return parser;
    }
}
