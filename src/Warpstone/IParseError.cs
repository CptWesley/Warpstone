namespace Warpstone
{
    /// <summary>
    /// Interface for parse errors.
    /// </summary>
    public interface IParseError
    {
        /// <summary>
        /// Turns the error into a readable message.
        /// </summary>
        /// <returns>A string representing the error.</returns>
        string GetMessage();
    }
}
