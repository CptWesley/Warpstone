namespace Warpstone.Sources.Tests.Helpers;

public static class CompilationHelper
{
    public static CSharpCompilation Compile(params IEnumerable<string> sources)
    {
        var syntaxTrees = sources.Select(static source =>
        {
            var sb = new StringBuilder();

            var hasNamespace = source.Contains("namespace ");

            if (!hasNamespace)
            {
                sb.AppendLine("namespace Source {");
            }

            sb.AppendLine(source);

            if (!hasNamespace)
            {
                sb.AppendLine("}");
            }

            var code = sb.ToString();
            return CSharpSyntaxTree.ParseText(code);
        });

        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
            .Select(_ => MetadataReference.CreateFromFile(_.Location))
            .Concat(
                [
                    MetadataReference.CreateFromFile(typeof(Generator).Assembly.Location),
                ]);

        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: syntaxTrees,
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        return compilation;
    }
}
