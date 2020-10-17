namespace Warpstone.Grammars
{
    /// <summary>
    /// Interface for grammar parsers.
    /// </summary>
    public interface IGrammarParser
    {
        /// <summary>
        /// Creates a parser which parses the given grammar.
        /// </summary>
        /// <param name="grammar">The grammar to create a parser for.</param>
        /// <returns>A parser for the given grammar.</returns>
        IParser<AstNode> CreateParser(string grammar);
    }
}
