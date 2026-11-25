using BenchmarkDotNet.Running;

namespace Warpstone.Benchmarks;

public static class Program
{
    public static void Main(params string[] args)
    {
        if (args is ["generate-minijava", var directory])
        {
            MiniJava.Warpstone3MiniJava.Generate(directory);
            return;
        }

        if (args is ["verify-minijava"])
        {
            Environment.ExitCode = MiniJavaBenchmark.Verify() ? 0 : 1;
            return;
        }

        BenchmarkSwitcher
            .FromAssembly(typeof(Program).Assembly)
            .Run(args);
    }
}
