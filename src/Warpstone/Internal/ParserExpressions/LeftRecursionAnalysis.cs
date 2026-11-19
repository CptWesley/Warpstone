using System;
using System.Collections.Generic;
using System.Linq;

namespace Warpstone.Internal.ParserExpressions
{
    /// <summary>
    /// Finds left recursion in a grammar the way pegen (CPython's parser generator) does.
    /// Builds the graph of parsers that can be invoked before any input is consumed, splits it into strongly connected components,
    /// and picks for each cyclic component a single leader that lies on every cycle. Only the leader grows its seed;
    /// the other members must not be memoized, so that each growth iteration sees the leader's latest seed.
    /// </summary>
    /// <seealso href="https://github.com/python/cpython/blob/main/Tools/peg_generator/pegen/parser_generator.py"/>
    internal static class LeftRecursionAnalysis
    {
        /// <summary>
        /// Finds the leaders and members of all left-recursive cycles.
        /// </summary>
        /// <param name="parsers">All parsers in the grammar, in a stable order.</param>
        /// <param name="preferredLeaders">Parsers to prefer as leader, such as the targets of lazy parsers.</param>
        /// <returns>The parsers that need to grow, and all parsers that are part of a left-recursive cycle.</returns>
        /// <exception cref="ArgumentException">Thrown when a cycle has no parser that lies on all of its cycles.</exception>
        public static (HashSet<IParser> Leaders, HashSet<IParser> Members) Analyze(IReadOnlyList<IParser> parsers, ISet<IParser> preferredLeaders)
        {
            var edges = BuildLeftEdges(parsers);
            var order = parsers.Select((p, i) => (p, i)).ToDictionary(x => x.p, x => x.i);
            var leaders = new HashSet<IParser>();
            var members = new HashSet<IParser>();

            foreach (var component in StronglyConnectedComponents(parsers, edges))
            {
                if (component.Count == 1 && !edges[component[0]].Contains(component[0]))
                {
                    continue;
                }

                var set = new HashSet<IParser>(component);
                members.UnionWith(set);

                // Explicit Grow parsers already break every cycle through them.
                var growers = new HashSet<IParser>(component.Where(IsGrow));
                if (growers.Count > 0 && IsAcyclicWithout(set, growers, edges))
                {
                    continue;
                }

                var leader = component
                    .Where(x => x is not ILazyParser && !IsMemo(x) && !IsGrow(x))
                    .OrderBy(x => preferredLeaders.Contains(x) ? 0 : 1)
                    .ThenBy(x => order[x])
                    .FirstOrDefault(x => IsAcyclicWithout(set, new HashSet<IParser>(growers) { x }, edges));

                if (leader is null)
                {
                    throw new ArgumentException(
                        "Provided parser contains left-recursive cycles without a parser that lies on all of them. "
                        + "Disable automatic growing recursion and place Grow parsers manually.",
                        nameof(parsers));
                }

                leaders.Add(leader);
            }

            return (leaders, members);
        }

        /// <summary>
        /// Checks whether the given parser is an explicit <see cref="GrowParser{T}"/>.
        /// </summary>
        /// <param name="parser">The parser to check.</param>
        /// <returns><see langword="true"/> if it is a grow parser.</returns>
        public static bool IsGrow(IParser parser)
            => IsInstanceOf(parser, typeof(GrowParser<>));

        /// <summary>
        /// Checks whether the given parser is an explicit <see cref="MemoParser{T}"/>.
        /// </summary>
        /// <param name="parser">The parser to check.</param>
        /// <returns><see langword="true"/> if it is a memo parser.</returns>
        public static bool IsMemo(IParser parser)
            => IsInstanceOf(parser, typeof(MemoParser<>));

        private static bool IsInstanceOf(IParser parser, Type genericType)
        {
            var type = parser.GetType();
            return type.IsGenericType && type.GetGenericTypeDefinition() == genericType;
        }

        private static Dictionary<IParser, List<IParser>> BuildLeftEdges(IReadOnlyList<IParser> parsers)
        {
            var nullable = FindNullable(parsers);
            var edges = new Dictionary<IParser, List<IParser>>();

            foreach (var parser in parsers)
            {
                var leftEdge = new List<IParser>();

                // ponytail: parsers not deriving from ParserBase are opaque, so left recursion through them is not found.
                if (parser is ILeftEdgeParser p)
                {
                    p.AnalyzeLeftEdge(nullable.Contains, leftEdge);
                }

                edges[parser] = leftEdge.Distinct().ToList();
            }

            return edges;
        }

        private static HashSet<IParser> FindNullable(IReadOnlyList<IParser> parsers)
        {
            var nullable = new HashSet<IParser>();
            var ignored = new List<IParser>();
            bool changed;

            do
            {
                changed = false;
                foreach (var parser in parsers)
                {
                    if (nullable.Contains(parser))
                    {
                        continue;
                    }

                    ignored.Clear();
                    var isNullable = parser is not ILeftEdgeParser p || p.AnalyzeLeftEdge(nullable.Contains, ignored);
                    if (isNullable)
                    {
                        nullable.Add(parser);
                        changed = true;
                    }
                }
            }
            while (changed);

            return nullable;
        }

        // Tarjan's algorithm.
        private static List<List<IParser>> StronglyConnectedComponents(IReadOnlyList<IParser> parsers, Dictionary<IParser, List<IParser>> edges)
        {
            var index = new Dictionary<IParser, int>();
            var low = new Dictionary<IParser, int>();
            var stack = new Stack<IParser>();
            var onStack = new HashSet<IParser>();
            var result = new List<List<IParser>>();

            void Visit(IParser v)
            {
                index[v] = low[v] = index.Count;
                stack.Push(v);
                onStack.Add(v);

                foreach (var w in edges[v])
                {
                    if (!index.TryGetValue(w, out var wIndex))
                    {
                        Visit(w);
                        low[v] = Math.Min(low[v], low[w]);
                    }
                    else if (onStack.Contains(w))
                    {
                        low[v] = Math.Min(low[v], wIndex);
                    }
                }

                if (low[v] == index[v])
                {
                    var component = new List<IParser>();
                    IParser w;
                    do
                    {
                        w = stack.Pop();
                        onStack.Remove(w);
                        component.Add(w);
                    }
                    while (!ReferenceEquals(w, v));

                    result.Add(component);
                }
            }

            foreach (var parser in parsers.Where(p => !index.ContainsKey(p)))
            {
                Visit(parser);
            }

            return result;
        }

        private static bool IsAcyclicWithout(HashSet<IParser> component, HashSet<IParser> removed, Dictionary<IParser, List<IParser>> edges)
        {
            var done = new HashSet<IParser>(removed);
            var active = new HashSet<IParser>();

            bool HasCycle(IParser v)
            {
                active.Add(v);
                foreach (var w in edges[v])
                {
                    if (!component.Contains(w) || done.Contains(w))
                    {
                        continue;
                    }

                    if (active.Contains(w) || HasCycle(w))
                    {
                        return true;
                    }
                }

                active.Remove(v);
                done.Add(v);
                return false;
            }

            return component.All(v => done.Contains(v) || !HasCycle(v));
        }
    }
}
