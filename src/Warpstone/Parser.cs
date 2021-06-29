using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Warpstone
{
    /// <summary>
    /// Parser class for parsing textual input.
    /// </summary>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    public abstract class Parser<TOutput> : IParser<TOutput>
    {
        /// <inheritdoc/>
        public IParseResult<TOutput> TryParse(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            IParseResult<TOutput> result = TryParse(input, 0);

            if (!result.Success)
            {
                result.Error.Position.Upgrade(input);
            }

            return result;
        }

        /// <inheritdoc/>
        public TOutput Parse(string input)
        {
            IParseResult<TOutput> result = TryParse(input);
            if (result.Success)
            {
                return result.Value;
            }

            throw new ParseException(result.Error.GetMessage());
        }

        /// <inheritdoc/>
        public abstract IParseResult<TOutput> TryParse(string input, int position);

        /// <inheritdoc/>
        public string ToTextMateGrammar(string languageId)
            => ToTextMateGrammar(languageId, languageId);

        /// <inheritdoc/>
        public string ToTextMateGrammar(string languageId, string languageName)
        {
            Dictionary<object, HighlightingNode> map = new Dictionary<object, HighlightingNode>();
            FillSyntaxHighlightingGraph(map);
            Dictionary<HighlightingNode, Highlight> patterns = new Dictionary<HighlightingNode, Highlight>();
            FillPatterns(patterns, map[this], Highlight.None, map, new HashSet<HighlightingNode>());
            Dictionary<HighlightingNode, string> names = new Dictionary<HighlightingNode, string>();
            int counter = 0;
            foreach (KeyValuePair<HighlightingNode, Highlight> entry in patterns)
            {
                names[entry.Key] = $"el{counter++}";
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("{ \"name\": \"").Append(languageName).Append("\", \"patterns\": [");

            foreach (KeyValuePair<HighlightingNode, Highlight> entry in patterns)
            {
                sb.Append("{ \"include\": \"#").Append(names[entry.Key]).Append("\" }, ");
            }

            if (patterns.Any())
            {
                sb.Length -= 2;
            }

            sb.Append("], \"repository\": { ");

            foreach (KeyValuePair<HighlightingNode, Highlight> entry in patterns)
            {
                sb.Append("\"").Append(names[entry.Key]).Append("\": ")
                    .Append("{ \"name\": \"").Append(ToName(entry.Value)).Append('.').Append(languageId)
                    .Append("\", \"match\": \"").Append(entry.Key.Pattern.Replace("\"", "\\\""))
                    .Append("\" }, ");
            }

            if (patterns.Any())
            {
                sb.Length -= 2;
            }

            sb.Append(" }, \"scopeName\": \"source.").Append(languageId).Append("\" }");
            return sb.ToString();
        }

        private static void FillPatterns(Dictionary<HighlightingNode, Highlight> patterns, HighlightingNode node, Highlight highlight, Dictionary<object, HighlightingNode> map, HashSet<HighlightingNode> visited)
        {
            if (visited.Contains(node))
            {
                return;
            }

            visited.Add(node);

            if (node.Highlight != Highlight.None)
            {
                highlight = node.Highlight;
            }

            if (!string.IsNullOrWhiteSpace(node.Pattern) && highlight != Highlight.None)
            {
                patterns[node] = highlight;
            }

            foreach (object child in node.Children)
            {
                FillPatterns(patterns, map[child], highlight, map, visited);
            }
        }

        /*
         public string ToTextMateGrammar(string languageId, string languageName)
        {
            Dictionary<object, HighlightingNode> map = new Dictionary<object, HighlightingNode>();
            FillSyntaxHighlightingGraph(map);
            Dictionary<object, string> names = new Dictionary<object, string>();
            int counter = 0;
            foreach (KeyValuePair<object, HighlightingNode> entry in map)
            {
                names[entry.Key] = $"el{counter++}";
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("{ \"name\": \"").Append(languageName).Append("\", \"patterns\": [");
            sb.Append("{ \"include\": \"#").Append(names[this]).Append("\" }");
            sb.Append("], \"repository\": {");

            foreach (KeyValuePair<object, HighlightingNode> entry in map)
            {
                bool removeTail = false;
                sb.Append("\"").Append(names[entry.Key]).Append("\": { ");

                string name = ToName(entry.Value.Highlight);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    sb.Append("\"name\": \"").Append(name).Append('.').Append(languageId).Append("\", ");
                    removeTail = true;
                }

                if (!string.IsNullOrWhiteSpace(entry.Value.Pattern))
                {
                    sb.Append("\"match\": \"").Append(entry.Value.Pattern.Replace("\"", "\\\"")).Append("\", ");
                    removeTail = true;
                }

                if (entry.Value.Children.Any())
                {
                    sb.Append("\"pattern\": [");

                    foreach (object child in entry.Value.Children)
                    {
                        sb.Append(" { \"include\": \"#").Append(names[child]).Append("\" }, ");
                    }

                    sb.Append("] ");
                    removeTail = false;
                }

                if (removeTail)
                {
                    sb.Length -= 2;
                }

                sb.Append("}, ");
            }

            if (map.Count > 0)
            {
                sb.Length -= 2;
            }

            sb.Append(" }, \"scopeName\": \"source.").Append(languageId).Append("\" }");
            return sb.ToString();
        }
        */

        /// <inheritdoc/>
        public abstract void FillSyntaxHighlightingGraph(Dictionary<object, HighlightingNode> graph);

        /// <summary>
        /// Gets the found characters.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="position">The position.</param>
        /// <returns>The found characters.</returns>
        protected string GetFound(string input, int position)
            => position < input?.Length ? $"'{input[position]}'" : "EOF";

        private static string ToName(Highlight highlight)
            => highlight switch
            {
                Highlight.Comment => "comment",
                Highlight.Keyword => "keyword",
                Highlight.String => "string.quoted.double",
                Highlight.Constant => "constant",
                _ => string.Empty,
            };

        public string ToRegex()
            => ToRegex(new Dictionary<object, string>());

        public string ToRegex(Dictionary<object, string> names)
        {
            if (names.TryGetValue(this, out string value))
            {
                return $"\\g<{value}>";
            }

            return ToRegex2(names);
        }

        public abstract string ToRegex2(Dictionary<object, string> names);
    }
}
