using System.Collections.Generic;
using System.Linq;
using Warpstone.Internal.Input;
using Warpstone.Internal.ParserExpressions;

namespace Warpstone.Internal.ParserImplementations
{
    /// <summary>
    /// Represents a parser that tries the provided alternatives in order and returns the result of the first successful one.
    /// </summary>
    /// <typeparam name="T">The result type of the parsers.</typeparam>
    internal sealed class OrParserImpl<T> : ParserImplementationBase<OrParser<T>, T>
    {
        private IParserImplementation<T>[] alternatives = default!;
        private Continuation[] continuations = default!;

        // Built on first use, when every alternative is initialized. Benign race: two threads build equal tables.
        private PlanTable? planTable;

        /// <inheritdoc />
        protected override void InitializeInternal(OrParser<T> parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
        {
            alternatives = new IParserImplementation<T>[parser.Alternatives.Count];
            for (var i = 0; i < alternatives.Length; i++)
            {
                alternatives[i] = (IParserImplementation<T>)parserLookup[parser.Alternatives[i]];
            }

            // continuations[i] runs after alternative i failed, and tries alternative i + 1. The last alternative needs none.
            continuations = new Continuation[alternatives.Length - 1];
            for (var i = continuations.Length - 1; i >= 0; i--)
            {
                continuations[i] = new(alternatives[i + 1], i + 1 < continuations.Length ? continuations[i + 1] : null);
            }
        }

        /// <inheritdoc />
        public override void Apply(IIterativeParseContext context, int position)
        {
            if (FindPlan(context, position) is { } plan)
            {
                plan.Apply(context, position);
                return;
            }

            context.ExecutionStack.Push((position, continuations[0]));
            context.ExecutionStack.Push((position, alternatives[0]));
        }

        /// <inheritdoc />
        public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
        {
            if (FindPlan(context, position) is { } plan)
            {
                return plan.Apply(context, position);
            }

            var last = alternatives.Length - 1;
            for (var i = 0; i < last; i++)
            {
                var result = alternatives[i].Apply(context, position);
                if (result.Success)
                {
                    return result;
                }
            }

            // Every alternative reports its own failures, so a failing last alternative's result is the result.
            return alternatives[last].Apply(context, position);
        }

        /// <inheritdoc />
        public override FirstChars? GetFirstChars(ISet<IParserImplementation> visiting)
            => FirstChars.Union(alternatives.Select(alternative => FirstChars.Of(alternative, visiting)));

        /// <summary>
        /// Finds the plan for the character at <paramref name="position"/>, or <see langword="null"/> to try every alternative.
        /// Only string input is checked: a reader may not have read the character yet.
        /// </summary>
        private Plan? FindPlan(IParseContext context, int position)
        {
            if ((planTable ??= new(alternatives)).Plans is not { } plans || context.Input is not StringParseInput input)
            {
                return null;
            }

            var content = input.Content;
            var c = position < content.Length ? content[position] : FirstChars.Count;
            return plans[c < FirstChars.Count ? c : FirstChars.Count];
        }

        /// <summary>
        /// One plan per ASCII character, plus one for any other character and the end of the input.
        /// <see cref="Plans"/> is <see langword="null"/> when no alternative can be skipped.
        /// </summary>
        private sealed class PlanTable
        {
            public PlanTable(IParserImplementation<T>[] alternatives)
            {
                var firsts = alternatives.Select(FirstChars.Of).ToArray();
                if (firsts.All(first => first is null))
                {
                    return;
                }

                var plans = new Plan?[FirstChars.Count + 1];
                var shared = new Dictionary<string, Plan>();
                for (var c = 0; c <= FirstChars.Count; c++)
                {
                    // An alternative is skipped when it has first characters and this character is not one of them.
                    var run = firsts.Select(first => first is null || (c < FirstChars.Count && first.Chars[c])).ToArray();
                    if (run.All(static r => r))
                    {
                        continue;
                    }

                    var key = new string(run.Select(r => r ? '1' : '0').ToArray());
                    if (!shared.TryGetValue(key, out var plan))
                    {
                        plan = new(alternatives, firsts, run);
                        shared.Add(key, plan);
                    }

                    plans[c] = plan;
                }

                Plans = plans;
            }

            public Plan?[]? Plans { get; }
        }

        /// <summary>
        /// The alternatives to run for one set of characters. A skipped alternative would fail on the first character,
        /// so its failures are reported where it would have run instead.
        /// </summary>
        private sealed class Plan
        {
            private readonly IParserImplementation<T>[] runs;
            private readonly IReadOnlyList<(IParserImplementation, string)>[] before;
            private readonly IReadOnlyList<(IParserImplementation, string)> after;
            private readonly PlanContinuation?[] continuations;

            public Plan(IParserImplementation<T>[] alternatives, FirstChars?[] firsts, bool[] run)
            {
                var runList = new List<IParserImplementation<T>>();
                var beforeList = new List<IReadOnlyList<(IParserImplementation, string)>>();
                var pending = new List<(IParserImplementation Parser, string Expected)>();
                var seen = FirstChars.CreateSeen();
                for (var i = 0; i < alternatives.Length; i++)
                {
                    if (run[i])
                    {
                        runList.Add(alternatives[i]);
                        beforeList.Add(pending.ToArray());
                        pending.Clear();
                        seen.Clear();
                    }
                    else
                    {
                        FirstChars.AddDistinct(pending, seen, firsts[i]!.Reports);
                    }
                }

                runs = runList.ToArray();
                before = beforeList.ToArray();
                after = pending.ToArray();

                // continuations[i] runs after run i: it stops on success, else reports and tries run i + 1.
                continuations = new PlanContinuation?[this.runs.Length];
                for (var i = 0; i < continuations.Length; i++)
                {
                    var last = i == continuations.Length - 1;
                    continuations[i] = last && after.Count == 0 ? null : new(this, i);
                }
            }

            private static void ReportAll(IParseContext context, int position, IReadOnlyList<(IParserImplementation Parser, string Expected)> reports)
            {
                for (var i = 0; i < reports.Count; i++)
                {
                    context.Failures.AddExpected(position, reports[i].Parser, reports[i].Expected);
                }
            }

            public UnsafeParseResult Apply(IRecursiveParseContext context, int position)
            {
                for (var i = 0; i < runs.Length; i++)
                {
                    ReportAll(context, position, before[i]);
                    var result = runs[i].Apply(context, position);
                    if (result.Success || (i == runs.Length - 1 && after.Count == 0))
                    {
                        return result;
                    }
                }

                ReportAll(context, position, after);
                return UnsafeParseResult.Failure(position);
            }

            public void Apply(IIterativeParseContext context, int position)
            {
                if (runs.Length == 0)
                {
                    ReportAll(context, position, after);
                    context.ResultStack.Push(UnsafeParseResult.Failure(position));
                    return;
                }

                Push(context, position, 0);
            }

            private void Push(IIterativeParseContext context, int position, int index)
            {
                ReportAll(context, position, before[index]);
                if (continuations[index] is { } continuation)
                {
                    context.ExecutionStack.Push((position, continuation));
                }

                context.ExecutionStack.Push((position, runs[index]));
            }

            private sealed class PlanContinuation : ContinuationParserImplementationBase
            {
                private readonly Plan plan;
                private readonly int index;

                public PlanContinuation(Plan plan, int index)
                {
                    this.plan = plan;
                    this.index = index;
                }

                /// <inheritdoc />
                public override void Apply(IIterativeParseContext context, int position)
                {
                    if (context.ResultStack.Peek().Success)
                    {
                        return;
                    }

                    if (index + 1 < plan.runs.Length)
                    {
                        context.ResultStack.Pop();
                        plan.Push(context, position, index + 1);
                        return;
                    }

                    ReportAll(context, position, plan.after);
                    context.ResultStack.Pop();
                    context.ResultStack.Push(UnsafeParseResult.Failure(position));
                }
            }
        }

        private sealed class Continuation : ContinuationParserImplementationBase
        {
            private readonly IParserImplementation alternative;
            private readonly Continuation? next;

            public Continuation(IParserImplementation alternative, Continuation? next)
            {
                this.alternative = alternative;
                this.next = next;
            }

            /// <inheritdoc />
            public override void Apply(IIterativeParseContext context, int position)
            {
                if (context.ResultStack.Peek().Success)
                {
                    return;
                }

                context.ResultStack.Pop();

                if (next is not null)
                {
                    context.ExecutionStack.Push((position, next));
                }

                context.ExecutionStack.Push((position, alternative));
            }
        }
    }
}
