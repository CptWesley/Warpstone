using BenchmarkDotNet.Attributes;
using Microsoft.CodeAnalysis.Text;
using System.Text.Json;
using Warpstone.Examples;
using Warpstone.Examples.Legacy.Json;

namespace Warpstone.Benchmarks;

public class JsonBenchmark
{
    private static readonly string MediumString = @"{""widget"": {
    ""debug"": ""on"",
    ""window"": {
        ""title"": ""Sample Konfabulator Widget"",
        ""name"": ""main_window"",
        ""width"": 500,
        ""height"": 500
    },
    ""image"": { 
        ""src"": ""Images/Sun.png"",
        ""name"": ""sun1"",
        ""hOffset"": 250,
        ""vOffset"": 250,
        ""alignment"": ""center""
    },
    ""text"": {
        ""data"": ""Click Here"",
        ""size"": 36,
        ""style"": ""bold"",
        ""name"": ""text1"",
        ""hOffset"": 250,
        ""vOffset"": 100,
        ""alignment"": ""center"",
        ""onMouseUp"": ""sun1.opacity = (sun1.opacity / 100) * 90;""
    }
}}";

    private static readonly SourceText MediumSource = SourceText.From(MediumString);

    [Benchmark]
    public IJsonValue Legacy()
        => JsonParser.Parse(MediumString).Value;

    [Benchmark]
    public Tokenizer<JsonNodeKind> Grammar()
        => JsonGrammar.json.Tokenize(MediumSource);

    [Benchmark]
    public JsonDocument System_Text_Json()
        => JsonDocument.Parse(MediumString);
}
