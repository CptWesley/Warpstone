namespace Legacy.Warpstone.Expressions
{
    /// <summary>
    /// Enum for indicating expression associativity.
    /// </summary>
    public enum Associativity
    {
        /// <summary>
        /// Indicates the expression type is left-to-right associative.
        /// </summary>
        Left,

        /// <summary>
        /// Indicates the expression type is right-to-left associative.
        /// </summary>
        Right,

        /// <summary>
        /// Indicates the expression type is non-associative.
        /// </summary>
        Non,
    }
}
