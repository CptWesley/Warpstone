using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Warpstone.Tests.SourceGeneration;

/// <summary>
/// The result of a generated parser, in a shape that can be compared with a Warpstone parse result.
/// </summary>
/// <param name="Success">Whether parsing succeeded.</param>
/// <param name="Value">The parsed value.</param>
/// <param name="ErrorPosition">The position of the error, or -1 if parsing succeeded.</param>
/// <param name="Expected">What was expected at the error position.</param>
/// <param name="Exceptions">The number of transformations that threw at the error position.</param>
public sealed record GeneratedResult(bool Success, object? Value, int ErrorPosition, IReadOnlyList<string> Expected, int Exceptions);

/// <summary>
/// Compiles the source generated for a Warpstone parser in memory, and runs it.
/// </summary>
public static class GeneratedParser
{
    private static readonly ConcurrentDictionary<string, Func<string, GeneratedResult>> Cache = new();

    private static readonly Lazy<MetadataReference[]> References = new(() => AppDomain.CurrentDomain.GetAssemblies()
        .Where(a => !a.IsDynamic && a.Location.Length > 0)
        .Select(a => (MetadataReference)MetadataReference.CreateFromFile(a.Location))
        .ToArray());

    /// <summary>
    /// Generates, compiles and loads the parser. Compilations are cached by <paramref name="key"/>.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="key">Identifies the grammar.</param>
    /// <param name="parser">The parser.</param>
    /// <param name="usings">The namespaces the transformations need.</param>
    /// <returns>A function running the generated parser.</returns>
    public static Func<string, GeneratedResult> Compile<T>(string key, IParser<T> parser, params string[] usings)
        => Cache.GetOrAdd(key, _ => CompileSource(parser.GenerateSource("Generated", "GeneratedParser", usings)));

    /// <summary>
    /// Compiles <paramref name="source"/> and loads the parser it declares.
    /// </summary>
    /// <param name="source">The generated source.</param>
    /// <returns>A function running the generated parser.</returns>
    public static Func<string, GeneratedResult> CompileSource(string source)
    {
        var type = Load(source);
        var tryParse = type.GetMethods().Single(m => m.Name == "TryParse" && m.GetParameters().Length == 3);

        return input =>
        {
            var arguments = new object?[] { input, null, null };
            var success = (bool)tryParse.Invoke(null, arguments)!;
            if (success)
            {
                return new(true, arguments[1], -1, [], 0);
            }

            var error = arguments[2]!;
            var errorType = error.GetType();
            return new(
                false,
                null,
                (int)errorType.GetProperty("Position")!.GetValue(error)!,
                (IReadOnlyList<string>)errorType.GetProperty("Expected")!.GetValue(error)!,
                ((System.Collections.ICollection)errorType.GetProperty("Exceptions")!.GetValue(error)!).Count);
        };
    }

    /// <summary>
    /// Compiles <paramref name="source"/> and loads the class <c>Generated.GeneratedParser</c> it declares.
    /// </summary>
    /// <param name="source">The generated source.</param>
    /// <returns>The class.</returns>
    public static Type Load(string source)
    {
        var compilation = CSharpCompilation.Create(
            $"Generated{Guid.NewGuid():N}",
            [CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest, preprocessorSymbols: PreprocessorSymbols()))],
            References.Value,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

        using var stream = new MemoryStream();
        var emit = compilation.Emit(stream);
        var errors = emit.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        if (errors.Count > 0)
        {
            throw new InvalidOperationException($"The generated source doesn't compile:\n{string.Join("\n", errors)}\n\n{source}");
        }

        var assembly = Assembly.Load(stream.ToArray());
        return assembly.GetType("Generated.GeneratedParser")!;
    }

    /// <summary>
    /// Checks that the generated parser and Warpstone agree on <paramref name="input"/>:
    /// the same success, value, error position and expected tokens.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="key">Identifies the grammar.</param>
    /// <param name="parser">The parser.</param>
    /// <param name="input">The input.</param>
    /// <param name="usings">The namespaces the transformations need.</param>
    public static void ShouldMatchWarpstone<T>(string key, IParser<T> parser, string input, params string[] usings)
    {
        var generated = Compile(key, parser, usings)(input);
        var expected = parser.TryParse(input);

        generated.Success.Should().Be(expected.Success, "both parsers should agree on whether '{0}' parses", input);
        if (expected.Success)
        {
            generated.Value.Should().BeEquivalentTo(expected.Value, o => o.WithStrictOrdering());
            return;
        }

        var tokens = expected.Errors.OfType<UnexpectedTokenError>()
            .SelectMany(e => e.InnerErrors.Count > 0 ? e.InnerErrors : [e])
            .SelectMany(e => e.Expected)
            .Distinct()
            .ToList();
        generated.ErrorPosition.Should().Be(expected.Errors.Max(e => e.Position), "the error position for '{0}' should match", input);
        generated.Expected.Should().BeEquivalentTo(tokens, "the expected tokens for '{0}' should match", input);
        generated.Exceptions.Should().Be(expected.Errors.OfType<TransformationError>().Count());
    }

    private static string[] PreprocessorSymbols()
    {
        var version = Environment.Version.Major;
        return [.. Enumerable.Range(5, version - 4).Select(v => $"NET{v}_0_OR_GREATER"), "NETCOREAPP", "NET"];
    }
}
