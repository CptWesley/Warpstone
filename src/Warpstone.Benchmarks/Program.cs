using BenchmarkDotNet.Running;

namespace Warpstone.Benchmarks;

public static class Program
{
    public static void Main(params string[] args)
    {
        BenchmarkSwitcher
            .FromAssembly(typeof(Program).Assembly)
            .Run(args);
    }
}
