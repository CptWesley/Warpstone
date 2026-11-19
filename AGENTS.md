# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

Warpstone is a packrat parser combinator library for C# with left-recursion support. The current major version is V3, which is in preview. The README still shows the V1/V2 import styles; for V3 use `using Warpstone; using static Warpstone.Parsers;`.

## Communication Style

Respond terse like smart caveman. All technical substance stay. Only fluff die.

Rules:
- Drop: articles (a/an/the), filler (just/really/basically), pleasantries, hedging
- Fragments OK. Short synonyms. Technical terms exact. Code unchanged.
- Pattern: [thing] [action] [reason]. [next step].
- Not: "Sure! I'd be happy to help you with that."
- Yes: "Bug in auth middleware. Fix:"

Switch level: /caveman lite|full|ultra|wenyan
Stop: "stop caveman" or "normal mode"

Auto-Clarity: drop caveman for security warnings, irreversible actions, user confused. Resume after.

Boundaries: code/commits/PRs written normal.

## Code Navigation

Activate the current dir as project using Serena.

Prefer Roslynaut, LSP or Serena over bash commands, grep and glob when navigating or editing the C# code base. Roslynaut sees the code as the compiler does: use `document_symbols`, `symbol_source`, `find_symbols`, `workspace_symbols`, `references` and `callers` instead of reading and grepping `.cs` files, and `diagnostics` instead of building after an edit. Pass projectPath `src/Warpstone.slnx` (absolute path) and keep passing the same path so the loaded workspace is reused.

## Lessons Learned Previous Sessions

Check [docs/lessons_learned/INDEX.md](docs/lessons_learned/INDEX.md) for a similar prior task before starting new work — one line per entry, links to the full write-up.

After each session, create a "lessons learned" markdown file in `docs/lessons_learned/`. It should give a clear summary of issues you (the AI) ran into and how we solved them, so a next session doesn't run into them again. Add a reference to the file with a single, short sentence summary to `docs/lessons_learned/INDEX.md`.

## Commands

Run everything from `src/`, where the solution `Warpstone.slnx` lives.

```sh
dotnet restore
dotnet build
dotnet test                                                # all test projects
dotnet test Warpstone.Tests                                # one project
dotnet test --filter "FullyQualifiedName~Warpstone.Tests.Parsers.MemoParser"   # one class/test
dotnet run -c Release --project Warpstone.Benchmarks -- --filter "*"           # BenchmarkDotNet
dotnet run --project Warpstone.Sandbox                     # REPL for trying a parser
```

- The library multi-targets `netstandard2.0` up to `net9.0`. CI installs the SDKs for .NET 3.0, 5, 6, 7, 8 and 9. To build or test locally without all of them, pass `-f net9.0`.
- Packages are managed centrally in `src/Directory.Packages.props`, and every project has a `packages.lock.json`. After you change a dependency, restore so the lock files are regenerated. CI builds run in locked mode (`ContinuousIntegrationBuild=true`).
- Analyzers run on every build: StyleCop (packable projects only), Sonar, Qowaiv, AsyncFixer and DotNetProjectFile. Rule overrides live in `src/.editorconfig` and `src/.globalconfig`.
- Package validation is checked against baseline `1.0.8.1`, and suppressions go in `Warpstone/CompatibilitySuppressions.xml`. Public API changes can make the build fail.

## Build layout

Shared MSBuild settings are split into `src/*.props`, which each project imports instead of using `Directory.Build.props`:
- `common.props`: imported by everything. Sets LangVersion 13, nullable, `ImplicitUsings` disabled, and the analyzers.
- `package.props`: packable libraries. Sets target frameworks, version, SourceLink and package validation.
- `sourcegenerator.props`: Roslyn generator, `netstandard2.0`.
- `tests.props` and `executable.props`: NUnit with AwesomeAssertions and AssertNet (`using static AssertNet.Assertions`), plus coverlet.
- `benchmarks.props`: benchmark project settings.

## Architecture

### Two-layer parser model (`src/Warpstone`)

1. **Parser expressions** (`Internal/ParserExpressions/*Parser.cs`, base `ParserBase<T>`) are the immutable combinator tree that users build through `Parsers` (static factory methods) and `ParserExtensions` (fluent methods such as `ThenAdd`, `Transform` and `ThenSkip`). They hold no parsing logic. Each one does two jobs:
   - `PerformAnalysisStep` walks the tree to count occurrences and detect recursion. The result is `ParserAnalysisInfo`, which records the maximum nesting depth and recursive and shared nodes.
   - `CreateUninitializedImplementation` creates its runtime counterpart.
2. **Parser implementations** (`Internal/ParserImplementations/*Impl.cs`, base `ParserImplementationBase<TParser,TResult>`) do the actual parsing. `ParserBase<T>.GetImplementation(ParseOptions)` builds and caches them per simplified `ParseOptions`. That method:
   - resolves `Lazy` parsers to their targets,
   - wraps shared nodes in `MemoParser` (when `EnableAutomaticMemoization` is on),
   - wraps recursive nodes in `GrowParser` (the left-recursion "seed growing" approach, when `EnableAutomaticGrowingRecursion` is on, which is the default),
   - wires up all implementations through a lookup dictionary in `Initialize`.

Adding a combinator therefore means adding an expression class, an `Impl` class, a factory method in `Parsers` or `ParserExtensions`, and support for both execution modes in the impl.

### Execution modes

Every implementation provides two `Apply` overloads:
- `Apply(IRecursiveParseContext, int)`: direct recursive calls that return an `UnsafeParseResult`. This mode is fast but limited by stack depth.
- `Apply(IIterativeParseContext, int)`: pushes work and continuations onto an explicit stack (`ContinuationParserImplementationBase` subclasses), so deep inputs don't overflow the stack.

`ParseContext.Create` picks `IterativeParseContext` or `RecursiveParseContext` according to `ParseOptions.ExecutionMode`. In `Auto` mode it chooses iterative when the grammar is recursive or nested more than 256 deep. Tests usually run as `[TestCaseSource]` over all three modes (see `Warpstone.Tests/Parsers/MemoParser.cs`), and new parser tests should do the same.

Value-type results are boxed. That is why some impls have Ref and Boxed variants, for example `AndRefRefParserImpl` and `MapBoxedParserImpl`, chosen at runtime by `CreateUninitializedImplementation`.

### Warpstone.Sources (source-distribution package)

`Warpstone.Sources` is an incremental source generator. It embeds every `src/Warpstone/**/*.cs` file (except `Properties/`) as a resource and emits each one into the consuming compilation with an `// <auto-generated/>` header. This has three consequences:
- Code in `src/Warpstone` must compile inside arbitrary consumer projects. Keep the explicit `using`s and block-scoped namespaces (`namespace X { }`), and don't rely on implicit usings or anything defined in `Properties/AssemblyInfo.cs`.
- Adding, removing or editing any file in `src/Warpstone` changes the Verify snapshots in `Warpstone.Sources.Tests/Helpers/snapshots/*.verified.cs`. Run `dotnet test Warpstone.Sources.Tests`, check the `*.received.cs` files, and accept them by renaming them to `.verified.cs` (or with the Verify tooling).
- `Snapshots.Incremental` checks that the generator's output is cacheable.

### Other projects

- `Warpstone.Benchmarks/Legacy/Warpstone1` and `Warpstone2` are frozen copies of the V1 and V2 libraries, kept only so benchmarks can compare against them. They have their own `.editorconfig`. Don't modernize or refactor them.
- `Warpstone.Benchmarks` also compares against Pidgin, Parlot and ParsecSharp.
