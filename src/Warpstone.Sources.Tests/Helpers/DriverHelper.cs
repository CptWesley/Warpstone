namespace Warpstone.Sources.Tests.Helpers;

public static class DriverHelper
{
    public static GeneratorDriver CreateDriver<TGenerator>()
        where TGenerator : IIncrementalGenerator, new()
    {
        var generator = new TGenerator().AsSourceGenerator();
        var driverOptions = new GeneratorDriverOptions(
            disabledOutputs: IncrementalGeneratorOutputKind.None,
            trackIncrementalGeneratorSteps: true);

        var properties = new Dictionary<string, string?>();

        var generatorName = typeof(TGenerator).Name;
        var generatorToggleName = $"build_property.Enable{generatorName}";

        properties[generatorToggleName] = "true";

        var driver = CSharpGeneratorDriver.Create(
            generators: [generator],
            driverOptions: driverOptions,
            optionsProvider: new OptionsProvider(properties));

        return driver;
    }

    private sealed class OptionsProvider : AnalyzerConfigOptionsProvider
    {
        private readonly AnalyzerConfigOptions options;

        public OptionsProvider(IReadOnlyDictionary<string, string?> options)
        {
            this.options = new Options(options);
        }

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
            => options;

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
            => options;

        public override AnalyzerConfigOptions GlobalOptions
            => options;

        private sealed class Options(IReadOnlyDictionary<string, string?> properties) : AnalyzerConfigOptions
        {
            public override bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
                => properties.TryGetValue(key, out value) && value is { };
        }
    }
}
