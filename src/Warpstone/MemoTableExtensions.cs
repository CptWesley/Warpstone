using System.Runtime.CompilerServices;

namespace Warpstone
{
    /// <summary>
    /// Provides extension methods for the <see cref="IReadOnlyMemoTable"/>
    /// and <see cref="IMemoTable"/> implementations.
    /// </summary>
    public static class MemoTableExtensions
    {
        /// <summary>
        /// Retrieves a read-only variant of the given <paramref name="memo"/> table.
        /// </summary>
        /// <param name="memo">The memo table to wrap.</param>
        /// <returns>A read-only variant of the given <paramref name="memo"/> table.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IReadOnlyMemoTable AsReadOnly(this IReadOnlyMemoTable memo)
            => memo is ReadOnlyMemoTable
            ? memo
            : new ReadOnlyMemoTable(memo);
    }
}
