namespace Warpstone.Internal;

/// <summary>
/// Contains predefined sets of <see cref="MethodImplOptions"/>.
/// </summary>
internal static class InternalMethodImplOptions
{
    /// <summary>
    /// Sets both the <see cref="MethodImplOptions.AggressiveInlining"/>
    /// and the <c>AggressiveOptimization</c> flag.
    /// </summary>
    public const MethodImplOptions InlinedOptimized
        = (MethodImplOptions)768;
}
