using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Warpstone.Errors;
using Warpstone.Internal.ParserExpressions;

namespace Warpstone.Internal.ParserImplementations
{
    /// <summary>
    /// Represents a parser that applies a source parser and then the parser
    /// that a select function picks for its result.
    /// </summary>
    /// <typeparam name="TIn">The result type of the source parser.</typeparam>
    /// <typeparam name="TSelected">The result type of the selected parser.</typeparam>
    /// <typeparam name="TOut">The result type of the combine function.</typeparam>
    internal sealed class BindParserImpl<TIn, TSelected, TOut> : ParserImplementationBase<BindParser<TIn, TSelected, TOut>, TOut>
    {
        private IParserImplementation<TIn> source = default!;
        private Func<TIn, IParser<TSelected>> select = default!;
        private Func<TIn, TSelected, TOut>? combine;
        private IReadOnlyDictionary<IParser, IParserImplementation> lookup = default!;
        private IParserImplementation reentryKey = default!;
        private Continuation continuation = default!;
        private CombineContinuation combineContinuation = default!;

        /// <inheritdoc />
        protected override void InitializeInternal(BindParser<TIn, TSelected, TOut> parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
        {
            source = (IParserImplementation<TIn>)parserLookup[parser.Source];
            select = parser.Select;
            combine = parser.Combine;
            lookup = parserLookup;
            reentryKey = parser.ReentryKey;
            continuation = new(this);
            combineContinuation = new(this);
        }

        /// <inheritdoc />
        public override void Apply(IIterativeParseContext context, int position)
        {
            context.ExecutionStack.Push((position, continuation));
            context.ExecutionStack.Push((position, source));
        }

        /// <inheritdoc />
        public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
        {
            var left = source.Apply(context, position);
            if (!left.Success)
            {
                return left;
            }

            var next = Select(context, position, left);
            if (next is null)
            {
                return UnsafeParseResult.Failure(position);
            }

            // The grammar analysis does not see selected parsers, so recursion through them has no stack guard of its own.
            RuntimeHelpers.EnsureSufficientExecutionStack();

            UnsafeParseResult right;
            if (left.Length > 0)
            {
                right = next.Apply(context, left.NextPosition);
            }
            else if (Enter(context, position))
            {
                right = next.Apply(context, position);
                Leave(context, position);
            }
            else
            {
                return UnsafeParseResult.Failure(position);
            }

            return Combine(left, right);
        }

        /// <summary>
        /// Picks the parser to apply after <paramref name="left"/>, or reports why there is none.
        /// </summary>
        private IParserImplementation<TSelected>? Select(IParseContext context, int position, in UnsafeParseResult left)
        {
            IParser<TSelected> next;
            try
            {
                next = select((TIn)left.Value!) ?? throw new InvalidOperationException("The bind function returned null instead of a parser.");
            }
            catch (Exception e)
            {
                context.Failures.AddError(new TransformationError(context, this, position, 0, null, e));
                return null;
            }

            // A parser of the analyzed grammar shares its memo entries, left recursion handling and stack guards.
            // Any other parser gets its own implementation, which the parser caches.
            return lookup.TryGetValue(next, out var impl)
                ? (IParserImplementation<TSelected>)impl
                : next.GetImplementation(context.Options);
        }

        /// <summary>
        /// Marks that a selected parser runs at <paramref name="position"/> without the source having consumed anything.
        /// Reaching this parser again at the same position before <see cref="Leave"/> is infinite (left) recursion,
        /// which the grammar analysis cannot see through selected parsers.
        /// </summary>
        /// <returns><see langword="false"/> if the parser was already running at this position.</returns>
        private bool Enter(IParseContext context, int position)
        {
            if (context.MemoTable.TryGetValue(position, reentryKey, out var marker) && marker.IsInProgress)
            {
                context.Failures.AddError(new InfiniteRecursionError(context, this, position, 1));
                return false;
            }

            context.MemoTable[position, reentryKey] = UnsafeParseResult.InProgress(position);
            return true;
        }

        private void Leave(IParseContext context, int position)
            => context.MemoTable[position, reentryKey] = UnsafeParseResult.Failure(position);

        private UnsafeParseResult Combine(in UnsafeParseResult left, in UnsafeParseResult right)
        {
            if (!right.Success)
            {
                return UnsafeParseResult.Failure(left.Position);
            }

            // Without a combine function the selected result is passed on as is, so value types are not boxed again.
            var value = combine is null ? right.Value : combine((TIn)left.Value!, (TSelected)right.Value!);
            return new UnsafeParseResult(left.Position, left.Length + right.Length, value);
        }

        /// <summary>
        /// Picks and pushes the selected parser once the source has finished, when executing in iterative mode.
        /// </summary>
        private sealed class Continuation : ContinuationParserImplementationBase
        {
            private readonly BindParserImpl<TIn, TSelected, TOut> owner;

            public Continuation(BindParserImpl<TIn, TSelected, TOut> owner)
            {
                this.owner = owner;
            }

            /// <inheritdoc />
            public override void Apply(IIterativeParseContext context, int position)
            {
                var left = context.ResultStack.Peek();
                if (!left.Success)
                {
                    return;
                }

                var next = owner.Select(context, position, left);
                if (next is null || (left.Length == 0 && !owner.Enter(context, position)))
                {
                    context.ResultStack.Pop();
                    context.ResultStack.Push(UnsafeParseResult.Failure(position));
                    return;
                }

                context.ExecutionStack.Push((left.NextPosition, owner.combineContinuation));
                context.ExecutionStack.Push((left.NextPosition, next));
            }
        }

        /// <summary>
        /// Combines the source and selected results when executing in iterative mode.
        /// </summary>
        private sealed class CombineContinuation : ContinuationParserImplementationBase
        {
            private readonly BindParserImpl<TIn, TSelected, TOut> owner;

            public CombineContinuation(BindParserImpl<TIn, TSelected, TOut> owner)
            {
                this.owner = owner;
            }

            /// <inheritdoc />
            public override void Apply(IIterativeParseContext context, int position)
            {
                var right = context.ResultStack.Pop();
                var left = context.ResultStack.Pop();

                if (left.Length == 0)
                {
                    owner.Leave(context, left.Position);
                }

                context.ResultStack.Push(owner.Combine(left, right));
            }
        }
    }
}
