using BenchmarkDotNet.Running;

namespace Warpstone.Benchmarks;

public static class Program
{
	public static void Main(string[] args)
	{
		BenchmarkRunner.Run<JsonBenchmark>();
	}
}
