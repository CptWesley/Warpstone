using System.Collections.Generic;
using Warpstone.SyntaxHighlighting;

namespace Warpstone
{
    /// <summary>
    /// Parser interface for parsing textual input.
    /// </summary>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    public interface IParser<out TOutput>
    {
        /// <summary>
        /// Parses the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The result of running the parser.</returns>
        IParseResult<TOutput> TryParse(string input);

        /// <summary>
        /// Parses the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The parsed result.</returns>
        /// <exception cref="ParseException">Thrown when the parser fails.</exception>
        TOutput Parse(string input);

        /// <summary>
        /// Parses the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="position">The position.</param>
        /// <returns>The result of running the parser.</returns>
        IParseResult<TOutput> TryParse(string input, int position);

        /// <summary>
        /// Converts to TextMate grammar in JSON format.
        /// </summary>
        /// <param name="languageId">The language identifier.</param>
        /// <returns>The JSON format of the TextMate grammar.</returns>
        string ToTextMateGrammar(string languageId);

        /// <summary>
        /// Converts to TextMate grammar in JSON format.
        /// </summary>
        /// <param name="languageId">The language identifier.</param>
        /// <param name="languageName">Name of the language.</param>
        /// <returns>The JSON format of the TextMate grammar.</returns>
        string ToTextMateGrammar(string languageId, string languageName);

        /// <summary>
        /// Gets the syntax highlighting graph.
        /// </summary>
        /// <param name="graph">The graph to fill.</param>
        void FillSyntaxHighlightingGraph(Dictionary<object, HighlightingNode> graph);

        string ToRegex();

        string ToRegex(Dictionary<object, string> names);

        //RegexNode ToRegexTree();
    }
}
