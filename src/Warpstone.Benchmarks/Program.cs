using BenchmarkDotNet.Running;
using Pidgin;

namespace Warpstone.Benchmarks;

public static class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<RightRecursiveSimpleString>();
    }
}
