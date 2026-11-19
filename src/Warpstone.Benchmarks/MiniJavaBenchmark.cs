using BenchmarkDotNet.Attributes;
using Legacy.Warpstone2;
using ParsecSharp;
using Pidgin;
using Warpstone.Benchmarks.MiniJava;

namespace Warpstone.Benchmarks;

/// <summary>Parses MiniJava sources of three sizes into the same AST with every library.</summary>
[MemoryDiagnoser]
public class MiniJavaBenchmark
{
    private static readonly (string Name, Func<string, CompilationUnit> Parse)[] AllParsers =
    [
        ("Warpstone3", s => Warpstone3MiniJava.Parser.Parse(s)),
        ("Warpstone3_left_recursive", s => Warpstone3MiniJava.LeftRecursiveParser.Parse(s)),
        ("Warpstone2", s => Warpstone2MiniJava.Parser.Parse(s)),
        ("Warpstone1", s => Warpstone1MiniJava.Parser.Parse(s)),
        ("Pidgin", s => PidginMiniJava.Parser.ParseOrThrow(s)),
        ("ParsecSharp", s => ParsecSharpMiniJava.Parser.Parse(s).Value),
        ("Farkle", FarkleMiniJava.Parse),
        ("Parlot_runtime", s => ParlotMiniJava.Parser.Parse(s) ?? throw new InvalidOperationException("Parlot failed to parse.")),
        ("Parlot_generated", s => ParlotGeneratedMiniJava.TryParse(s, out var unit) ? unit : throw new InvalidOperationException("Parlot failed to parse.")),
    ];

    [Params("small", "medium", "large")]
    public string Size = "small";

    private string input = string.Empty;

    [GlobalSetup]
    public void Setup()
    {
        input = MiniJavaGrammar.LoadInput(Size);
    }

    [Benchmark(Baseline = true)]
    public CompilationUnit Warpstone3() => Warpstone3MiniJava.Parser.Parse(input);

    [Benchmark]
    public CompilationUnit Warpstone3_left_recursive() => Warpstone3MiniJava.LeftRecursiveParser.Parse(input);

    [Benchmark]
    public CompilationUnit Warpstone2() => Warpstone2MiniJava.Parser.Parse(input);

    [Benchmark]
    public CompilationUnit Warpstone1() => Warpstone1MiniJava.Parser.Parse(input);

    [Benchmark]
    public CompilationUnit Pidgin() => PidginMiniJava.Parser.ParseOrThrow(input);

    [Benchmark]
    public CompilationUnit ParsecSharp() => ParsecSharpMiniJava.Parser.Parse(input).Value;

    [Benchmark]
    public CompilationUnit Farkle() => FarkleMiniJava.Parse(input);

    [Benchmark]
    public CompilationUnit Parlot_runtime() => ParlotMiniJava.Parser.Parse(input)!;

    [Benchmark]
    public CompilationUnit Parlot_generated()
        => ParlotGeneratedMiniJava.TryParse(input, out var unit) ? unit : null!;

    /// <summary>Checks that every parser gives the same AST as Warpstone 3 for every input. Returns whether they all do.</summary>
    public static bool Verify()
    {
        var ok = true;
        foreach (var size in new[] { "small", "medium", "large" })
        {
            var source = MiniJavaGrammar.LoadInput(size);
            var expected = Warpstone3MiniJava.Parser.Parse(source);
            Console.WriteLine($"{size}: {source.Length} chars, {expected.Classes.Count} classes, AST dump {expected.ToString().Length} chars");
            foreach (var (name, parse) in AllParsers)
            {
                string verdict;
                try
                {
                    verdict = expected.Equals(parse(source)) ? "same AST" : "DIFFERENT AST";
                }
                catch (Exception e) when (e is not OutOfMemoryException)
                {
                    var cause = e.GetBaseException();
                    verdict = $"FAILED: {cause.GetType().Name}: {cause.Message}";
                }

                ok &= verdict == "same AST";
                Console.WriteLine($"  {name,-26} {verdict}");
            }
        }

        return ok;
    }
}
