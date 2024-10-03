namespace Legacy.Warpstone
{
    /// <summary>
    /// Provides interface for classes that have a <see cref="object.ToString()"/> override that is bounded by depth.
    /// </summary>
    public interface IBoundedToString
    {
        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <param name="depth">The depth.</param>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        string ToString(int depth);
    }
}
