namespace Warpstone.Sources;

/// <summary>
/// Performs the code generation.
/// </summary>
[Generator]
public sealed class Generator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(Generate);
    }

    private static void Generate(IncrementalGeneratorPostInitializationContext context)
    {
        var asm = typeof(Generator).Assembly;
        var files = asm.GetManifestResourceNames();

        foreach (var file in files.Where(static file => file.EndsWith(".cs")))
        {
            using var stream = asm.GetManifestResourceStream(file);
            using var reader = new StreamReader(stream);
            var content = reader.ReadToEnd();

            context.AddSource(file, content);
        }
    }
}
