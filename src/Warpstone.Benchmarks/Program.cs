using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Warpstone.Benchmarks.Arithmetic.Simple;

namespace Warpstone.Benchmarks;

public static class Program
{
    public static void Main(string[] args)
    {
        Summary simpleArith = BenchmarkRunner.Run<SimpleArithmeticBenchmark>();
    }
}