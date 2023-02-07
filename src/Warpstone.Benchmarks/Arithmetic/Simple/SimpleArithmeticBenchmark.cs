using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warpstone.Benchmarks.Arithmetic.Simple;

[MemoryDiagnoser]
public class SimpleArithmeticBenchmark
{
    private const string input1 = "45 + 3 * 72 + (54 - 2) * -(60 / 3) + 45 + 3 * 72 + (54 - 2) * -(60 / 3) + 45 + 3 * 72 + (54 - 2) * -(60 / 3) + 45 + 3 * 72 + (54 - 2) * -(60 / 3)";
    private static readonly string input = string.Join(" * ", Enumerable.Repeat(input1, 10));

    [Benchmark]
    public Exp? Warpstone()
        => WarpstoneSimpleArithmetic.Parse(input);
}