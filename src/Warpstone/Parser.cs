using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Warpstone.Parsers;
using Warpstone.ParseState;

namespace Warpstone;

/// <summary>
/// Parser class for parsing textual input.
/// </summary>
/// <typeparam name="TOutput">The type of the output.</typeparam>
public abstract class Parser<TOutput> : IParser<TOutput>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Parser{TOutput}"/> class.
    /// </summary>
    /// <param name="subParsers">The list of parsers that might be invoked by this parser.</param>
    public Parser(IEnumerable<IParser> subParsers)
    {
        SubParsers = new List<IParser>(subParsers);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Parser{TOutput}"/> class.
    /// </summary>
    /// <param name="subParsers">The list of parsers that might be invoked by this parser.</param>
    public Parser(params IParser[] subParsers)
        : this(subParsers as IEnumerable<IParser>)
    {
    }

    /// <inheritdoc/>
    public Type OutputType => typeof(TOutput);

    /// <inheritdoc/>
    public virtual IReadOnlyList<IParser> SubParsers { get; }

    /// <inheritdoc/>
    public bool IsTerminal => SubParsers.Count <= 0;

    /// <inheritdoc/>
    public string ToString(int depth)
    {
        if (depth < 0)
        {
            return "...";
        }

        return InternalToString(depth);
    }

    /// <inheritdoc/>
    public sealed override string ToString()
        => ToString(4);

    /// <inheritdoc/>
    public abstract IParseResult<TOutput> Eval(IParseState state, int position, int maxLength, IRecursionParser recurse, CancellationToken cancellationToken);

    /// <inheritdoc/>
    IParseResult IParser.Eval(IParseState state, int position, int maxLength, IRecursionParser recurse, CancellationToken cancellationToken)
        => Eval(state, position, maxLength, recurse, cancellationToken);

    /// <inheritdoc/>
    public string ToDotGraph()
    {
        StringBuilder sb = new StringBuilder()
            .AppendLine("digraph {");

        Queue<IParser> queue = new Queue<IParser>();
        Dictionary<IParser, int> indices = new Dictionary<IParser, int>();
        Dictionary<IParser, HashSet<IParser>> incoming = new Dictionary<IParser, HashSet<IParser>>();
        HashSet<IParser> cached = new HashSet<IParser>();
        int nextIndex = 0;

        queue.Enqueue(this);

        while (queue.Any())
        {
            IParser parser = queue.Dequeue();
            if (indices.ContainsKey(parser))
            {
                continue;
            }

            int index = nextIndex++;
            indices.Add(parser, index);

            foreach (IParser child in parser.SubParsers)
            {
                queue.Enqueue(child);

                if (!incoming.TryGetValue(child, out HashSet<IParser>? incomingSet))
                {
                    incomingSet = new HashSet<IParser>();
                    incoming[child] = incomingSet;
                }

                incomingSet.Add(parser);

                if (parser.SubParsers.Count > 1)
                {
                    cached.Add(child);
                }
            }
        }

        foreach (KeyValuePair<IParser, int> entry in indices)
        {
            IParser parser = entry.Key;
            int index = entry.Value;

            Dictionary<string, string> attributes = new Dictionary<string, string>();
            attributes["label"] = $"\"{parser.ToString(0).Replace(@"\", "\\").Replace("\"", "\\\"")}\"";

            if (parser.IsTerminal)
            {
                attributes["style"] = "filled";
                attributes["color"] = "\"#f5a742\"";
            }

            if (parser == this)
            {
                attributes["style"] = "filled";
                attributes["color"] = "\"#42f58d\"";
            }

            if (cached.Contains(parser))
            {
                attributes["shape"] = "rectangle";
            }

            sb.AppendLine($"\tp{index}[{string.Join(",", attributes.Select(x => $"{x.Key}={x.Value}"))}];");
        }

        foreach (KeyValuePair<IParser, int> entry in indices)
        {
            IParser parser = entry.Key;
            int index = entry.Value;

            foreach (IParser child in parser.SubParsers)
            {
                int childIndex = indices[child];
                sb.AppendLine($"\tp{childIndex}->p{index};");
            }
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    /// <summary>
    /// Gets the found characters.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="position">The position.</param>
    /// <param name="length">The length to retrieve.</param>
    /// <returns>The found characters.</returns>
    protected string GetFound(string input, int position, int length)
    {
        if (length == 1)
        {
            return position < input?.Length ? $"'{input[position]}'" : "EOF";
        }

        int remainingLength = Math.Min(input.Length - position, length);
        string found = input.Substring(position, remainingLength);

        return $"\"{found}\"";
    }

    /// <summary>
    /// Provides a stringified version of the parser without depth checks.
    /// </summary>
    /// <param name="depth">The maximum depth to explore.</param>
    /// <returns>The stringified version of the parser.</returns>
    protected abstract string InternalToString(int depth);
}
