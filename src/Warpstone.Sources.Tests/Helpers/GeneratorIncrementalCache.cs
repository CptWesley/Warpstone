namespace Warpstone.Sources.Tests.Helpers;

/// <summary>
/// Used to validate the incremental nature of the generators.
/// </summary>
/// <remarks>
/// Based on: https://andrewlock.net/creating-a-source-generator-part-10-testing-your-incremental-generator-pipeline-outputs-are-cacheable/.
/// </remarks>
public static class GeneratorIncrementalCache
{
    private static readonly FrozenSet<string> TrackingNames
        = typeof(Generator)
        .Assembly
        .GetTypes()
        .Where(t => !t.IsAbstract)
        .Where(t => t.IsAssignableTo(typeof(IIncrementalGenerator)))
        .Select(t => t.Name)
        .ToFrozenSet();

    public static void Verify<TGenerator>(params IEnumerable<string> sources)
        where TGenerator : IIncrementalGenerator, new()
    {
        var driver = DriverHelper.CreateDriver<TGenerator>();
        var compilation1 = CompilationHelper.Compile(sources);
        var compilation2 = compilation1.Clone();

        driver = driver.RunGenerators(compilation1);
        var result1 = driver.GetRunResult();
        result1.GeneratedTrees.Should().HaveCountGreaterThanOrEqualTo(1);

        driver = driver.RunGenerators(compilation2);
        var result2 = driver.GetRunResult();
        result2.GeneratedTrees.Should().HaveCountGreaterThanOrEqualTo(1);

        var steps1 = GetTrackedSteps(result1);
        var steps2 = GetTrackedSteps(result2);

        foreach (var stepName in TrackingNames)
        {
            var s1 = steps1[stepName];
            var s2 = steps2[stepName];
            Validate(stepName, s1, s2);
        }
    }

    private static Dictionary<string, ImmutableArray<IncrementalGeneratorRunStep>> GetTrackedSteps(GeneratorDriverRunResult result)
    {
        var dict = TrackingNames.ToDictionary(n => n, _ => ImmutableArray<IncrementalGeneratorRunStep>.Empty);

        foreach (var entry in result
            .Results[0]
            .TrackedSteps
            .Where(step => TrackingNames.Contains(step.Key)))
        {
            dict[entry.Key] = entry.Value;
        }

        return dict;
    }

    private static void Validate(
        string stepName,
        ImmutableArray<IncrementalGeneratorRunStep> steps1,
        ImmutableArray<IncrementalGeneratorRunStep> steps2)
    {
        steps1.Should().HaveSameCount(steps2);

        if (steps1.Length == 0)
        {
            return;
        }

        for (var i = 0; i < steps1.Length; i++)
        {
            var s1 = steps1[i];
            var s2 = steps2[i];

            var o1 = s1.Outputs.Select(static x => x.Value);
            var o2 = s2.Outputs.Select(static x => x.Value);

            // Check outputs are the same.
            o1.Should()
                .Equal(o2, $"because {stepName} should produce cacheable outputs");

            // Check run 2 only used the cached results.
            s2.Outputs.Should()
                .OnlyContain(
                    x => x.Reason == IncrementalStepRunReason.Cached || x.Reason == IncrementalStepRunReason.Unchanged,
                    $"{stepName} expected to have reason {IncrementalStepRunReason.Cached} or {IncrementalStepRunReason.Unchanged}");

            // Check for illegal types.
            ValidateObjectGraph(s1, stepName);
            ValidateObjectGraph(s2, stepName);
        }
    }

    private static void ValidateObjectGraph(IncrementalGeneratorRunStep runStep, string stepName)
    {
        // Including the stepName in error messages to make it easy to isolate issues
        var because = $"{stepName} shouldn't contain banned symbols";
        var visited = new HashSet<object>();

        // Check all of the outputs - probably overkill, but why not
        foreach (var (obj, _) in runStep.Outputs)
        {
            Visit(obj);
        }

        void Visit(object? node)
        {
            // If we've already seen this object, or it's null, stop.
            if (node is null || !visited.Add(node))
            {
                return;
            }

            // Make sure it's not a banned type
            node.Should()
                .NotBeOfType<Compilation>(because)
                .And.NotBeOfType<ISymbol>(because)
                .And.NotBeOfType<SyntaxNode>(because);

            // Examine the object
            Type type = node.GetType();
            if (type.IsPrimitive || type.IsEnum || type == typeof(string))
            {
                return;
            }

            // If the object is a collection, check each of the values
            if (node is IEnumerable collection and not string)
            {
                foreach (object element in collection)
                {
                    // recursively check each element in the collection
                    Visit(element);
                }

                return;
            }

            // Recursively check each field in the object
            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var fieldValue = field.GetValue(node);
                Visit(fieldValue);
            }
        }
    }
}
