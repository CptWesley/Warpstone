# Benchmark comparison packages update (Parlot, ParsecSharp, Pidgin)

## What changed

- Parlot 1.4.0 → 2.3.1, ParsecSharp 4.1.1 → 5.2.0, Pidgin 3.5.0 → 3.5.1 in `src/Directory.Packages.props`.
- ParsecSharp 5 and Pidgin 3.5.1: the existing benchmark code compiled unchanged.
- Parlot 2 removed `Parser<T>.Compile()`. Its replacement is the separate `Parlot.SourceGenerator` package
  (added with `PrivateAssets="all"`). `Parlot_compiled_benchmark` became `Parlot_generated_benchmark`.

## Parlot source generator setup

- The grammar factory goes in a build-only `*.parlot.cs` file (`RightRecursiveSimpleString.parlot.cs`).
  The package's targets remove it from `Compile` and pass it as `AdditionalFiles`, so it isn't compiled into the assembly.
- The factory is a `[GenerateParser(nameof(EntryPoint))]` static method returning `Parser<T>`. The entry point is a
  `static partial bool EntryPoint(string input, out T value)` declared in the same partial class in normal source.
- The benchmark class therefore has to be `partial`.
- If you ignore the `bool` result, Sonar reports S3241, so use it (`? result : string.Empty`).

## Pitfalls

- Generated Parlot parsers fail (return `false`) instead of overflowing the stack on deep recursion. At N=100000 the
  generated benchmark is very fast because it parses nothing. Don't read that row as a real result.
- At N=100000 these rows show NA (stack overflow), as before the update: Warpstone3 recursive, Warpstone1, Pidgin and
  Parlot (non-generated). Warpstone1 is NA at N=1000 too.
- Quick check: `dotnet run -c Release --project Warpstone.Benchmarks -- --filter "*" --job dry`. `ReturnValueValidator`
  prints mismatches under `// Validating benchmarks:`. If nothing follows that line, the results match.
- Browse a package's API offline through its `.xml` docs in `~/.nuget/packages/<id>/<ver>/lib/<tfm>/`. Download
  analyzer-only packages from `api.nuget.org/v3-flatcontainer/...nupkg` and unzip them to read the README.
