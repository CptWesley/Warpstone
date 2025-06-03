#pragma warning disable CS8777 // Intentional.

namespace Legacy.Warpstone2.Internal;

/// <summary>
/// Provides helper methods to deal with the debugger.
/// </summary>
internal static class DebugHelpers
{
    /// <summary>
    /// In debug mode asserts that the type of <paramref name="obj"/>
    /// is <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The expected type.</typeparam>
    /// <param name="obj">The input object.</param>
    /// <returns>The found object as the given type <typeparamref name="T"/>.</returns>
    [MethodImpl(InternalMethodImplOptions.InlinedOptimized)]
    public static T AssertOfType<T>([NotNull] this object? obj)
    {
        Debug.Assert(
            obj is T,
            $"Expected object of type '{obj?.GetType().FullName ?? "null"}' to be of type '{typeof(T).FullName}'.");
        return (T)obj!;
    }
}
