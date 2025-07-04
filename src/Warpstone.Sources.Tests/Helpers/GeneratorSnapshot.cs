namespace Warpstone.Sources.Tests.Helpers;

public static class GeneratorSnapshot
{
    public static void Verify<TGenerator>(params IEnumerable<string> sources)
        where TGenerator : IIncrementalGenerator, new()
    {
        var driver = DriverHelper.CreateDriver<TGenerator>();
        var compilation = CompilationHelper.Compile(sources);

        driver = driver.RunGenerators(compilation);

        var result = driver.GetRunResult();
        result.GeneratedTrees.Should().HaveCountGreaterThanOrEqualTo(1);

        var settings = new VerifySettings();
        settings.UseDirectory(GetOutputPath());
        settings.AutoVerify(fileName => !File.Exists(fileName));

        Verifier.Verify(driver, settings).GetAwaiter().GetResult();
    }

    private static string GetOutputPath([CallerFilePath] string? path = null)
    {
        var curDir = path is null
            ? Directory.GetCurrentDirectory()
            : Path.GetDirectoryName(path) ?? Directory.GetCurrentDirectory();

        var outputDir = Path.Combine(curDir, "snapshots");
        Directory.CreateDirectory(outputDir);
        return outputDir;
    }
}
