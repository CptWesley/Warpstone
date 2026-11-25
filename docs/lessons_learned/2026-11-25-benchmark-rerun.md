# Rerun all benchmarks and refresh README tables

Asked to run the whole benchmark suite and update the README results. Ran `verify-minijava` (all eleven parsers same AST),
then `--filter "*" --inProcess`, which crashed; reran `*MiniJavaBenchmark*` alone (11 min) and regenerated the README
`### Performance` tables from the CSV report.

## Issues

- `--filter "*" --inProcess` dies with `Stack overflow.` during `// Validating benchmarks:`.
  `RightRecursiveSimpleString` has `[ReturnValueValidator]`, which runs every case in the host process, and
  `Warpstone1_benchmark` overflows at large `N` (by design: that benchmark measures stack depth). A stack overflow
  can't be caught, so the whole run is lost before any MiniJava result is written. The `results/` files keep their
  old timestamps, so check those before trusting a "completed, exit 0" (the `echo` hides the dotnet exit code).
  Run MiniJava on its own; `RightRecursiveSimpleString` needs the validator removed (and out-of-process runs, which
  .NET 11 didn't support with BenchmarkDotNet 0.15.8) before it can run at all.
- `python -I` ignores `PYTHONIOENCODING`, so printing `μs` fails on the cp1252 console. Use `python -I -X utf8`.
- The github report no longer has a `Median` column, so don't index it by column position. Parse the
  `*-report.csv` by header name instead.
- Third-party libraries came out 5–8% slower than in the 2026-11-20 run (Pidgin 12.4 → 13.7 ms, Parlot generated
  951 → 1,029 μs), so this machine was slower that day; compare Warpstone changes against that drift.

## Results vs 2026-11-20

- V3 large allocations 864 → 704 KB, generated 902 → 576 KB (now the lowest of all), left-recursive 1,694 → 1,591 KB,
  generated left-recursive 1,924 → 1,655 KB.
- Left-recursive time 4.05 → 3.63 ms and generated left-recursive 1.23 → 1.08 ms (now ahead of Parlot runtime),
  despite the slower machine. V3 non-left-recursive time 2.05 → 2.26 ms, roughly in line with the drift.
