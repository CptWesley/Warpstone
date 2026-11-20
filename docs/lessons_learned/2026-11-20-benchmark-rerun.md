# Rerun MiniJava benchmarks and refresh README tables

Reran `verify-minijava` (all nine parsers same AST), then the full `*MiniJavaBenchmark*` suite with `--inProcess`
(BenchmarkDotNet 0.16.0-preview.2, .NET 11.0), and copied Mean/Allocated into the README `### Performance` tables.

## Notes

- Still needs `--inProcess` and `< /dev/null` on .NET 11; ran it as a background task (~25 min), no issues.
- Since the previous tables V3 large allocations dropped 1,027 → 866 KB and V3 left-recursive 2,528 → 1,696 KB
  (sparse memo chunks, see [2026-11-20-failure-tracker-and-memo-entry.md](2026-11-20-failure-tracker-and-memo-entry.md)),
  so the left-recursive row now sorts above Pidgin in the memory table. V3 small allocations rose 12.0 → 14.7 KB.
- Rows are sorted by the large-input value; re-sort after each rerun.
- Python `open()` in text mode turns CRLF into LF. README is LF in git, so it was harmless here; check with
  `grep -c $'\r'` before rewriting a file that way.
