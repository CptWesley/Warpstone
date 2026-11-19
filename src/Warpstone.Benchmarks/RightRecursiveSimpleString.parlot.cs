using Parlot.Fluent;
using Parlot.SourceGenerator;

namespace Warpstone.Benchmarks;

public partial class RightRecursiveSimpleString
{
    [GenerateParser(nameof(ParlotGeneratedTryParse))]
    private static Parser<string> CreateParlotGeneratedParser()
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
