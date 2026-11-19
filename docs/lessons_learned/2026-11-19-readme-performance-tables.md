# README performance tables from the MiniJava benchmark

Added CPU-time and allocation tables (small/medium/large MiniJava inputs, all nine `MiniJavaBenchmark` methods) under
"How does it compare?" in `README.md`, as a `### Performance` subsection with footnote 7 for the left-recursive V3 variant.

## Pitfalls

- Since `a12efe1` (#50) the benchmark project targets `net11.0`. BenchmarkDotNet 0.15.8 doesn't know that moniker and crashes
  during validation with `NotImplementedException: GetRuntimeVersion not implemented for NotRecognized`.
  Workaround: pass `--inProcess` (uses `InProcessEmitToolchain`, `MemoryDiagnoser` still works). Drop the flag once a
  BenchmarkDotNet version that knows .NET 11 is in `Directory.Packages.props`.
- Old results in `src/BenchmarkDotNet.Artifacts/results/` were a V3-only ShortRun on .NET 8. Don't copy numbers from there;
  rerun the full suite (default job, about 25 minutes in-process).
- Run `verify-minijava` first so the tables compare parsers that produce the same AST.

## Updating the tables

Rerun with `dotnet run -c Release --project Warpstone.Benchmarks -- --filter "*MiniJavaBenchmark*" --inProcess < /dev/null`
from `src/`, then copy Mean and Allocated from `BenchmarkDotNet.Artifacts/results/Warpstone.Benchmarks.MiniJavaBenchmark-report-github.md`.
Update the environment line under the tables too.
