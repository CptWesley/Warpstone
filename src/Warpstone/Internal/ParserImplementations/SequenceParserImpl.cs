using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Warpstone.Errors;
using Warpstone.Internal.ParserExpressions;

namespace Warpstone.Internal.ParserImplementations
{
    /// <summary>
    /// Parses two parsers in sequence and combines their results with a single function call.
    /// </summary>
    /// <typeparam name="TFirst">The result type of the first parser.</typeparam>
    /// <typeparam name="TSecond">The result type of the second parser.</typeparam>
    /// <typeparam name="TResult">The combined result type.</typeparam>
    internal sealed class SequenceParserImpl<TFirst, TSecond, TResult> : ParserImplementationBase<SequenceParser<TFirst, TSecond, TResult>, TResult>, IFirstCharsSource
    {
        private IParserImplementation<TFirst> first = default!;
        private IParserImplementation<TSecond> second = default!;
        private Func<TFirst, TSecond, TResult> combine = default!;
        private Continuation continuation = default!;

        /// <inheritdoc />
        public FirstChars? GetFirstChars(ISet<IParserImplementation> visiting)
            => FirstChars.Of(first, visiting);

        /// <inheritdoc />
        protected override void InitializeInternal(SequenceParser<TFirst, TSecond, TResult> parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
        {
            first = (IParserImplementation<TFirst>)parserLookup[parser.First];
            second = (IParserImplementation<TSecond>)parserLookup[parser.Second];
            combine = parser.Combine;
            continuation = new(this, second);
        }

        /// <inheritdoc />
        public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
        {
            var left = first.Apply(context, position);

            if (!left.Success)
            {
                return left;
            }

            var right = second.Apply(context, left.NextPosition);

            return right.Success
                ? Combine(context, left, right)
                : UnsafeParseResult.Failure(position);
        }

        /// <inheritdoc />
        public override void Apply(IIterativeParseContext context, int position)
        {
            context.ExecutionStack.Push((position, continuation));
            context.ExecutionStack.Push((position, first));
        }

        private UnsafeParseResult Combine(IParseContext context, in UnsafeParseResult left, in UnsafeParseResult right)
        {
            try
            {
                var value = combine(Cast<TFirst>(left.Value), Cast<TSecond>(right.Value));
                return new(left.Position, left.Length + right.Length, value);
            }
            catch (Exception e)
            {
                context.Failures.AddError(new TransformationError(context, this, left.Position, 0, null, e));
                return UnsafeParseResult.Failure(left.Position);
            }
        }

        /// <summary>
        /// Unboxes value types, and skips the type check for reference types: the parser that produced the value guarantees its type.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T Cast<T>(object? value)
        {
#if NETCOREAPP3_0_OR_GREATER
            if (!typeof(T).IsValueType)
            {
                return Unsafe.As<object?, T>(ref value);
            }
#endif
            return (T)value!;
        }

        private sealed class Continuation : ContinuationParserImplementationBase
        {
            private readonly IParserImplementation second;
            private readonly SecondContinuation secondContinuation;

            public Continuation(SequenceParserImpl<TFirst, TSecond, TResult> root, IParserImplementation second)
            {
                this.second = second;
                secondContinuation = new(root);
            }

            /// <inheritdoc />
            public override void Apply(IIterativeParseContext context, int position)
            {
                var left = context.ResultStack.Peek();

                if (!left.Success)
                {
                    return;
                }

                context.ExecutionStack.Push((left.NextPosition, secondContinuation));
                context.ExecutionStack.Push((left.NextPosition, second));
            }
        }

        private sealed class SecondContinuation : ContinuationParserImplementationBase
        {
            private readonly SequenceParserImpl<TFirst, TSecond, TResult> root;

            public SecondContinuation(SequenceParserImpl<TFirst, TSecond, TResult> root)
            {
                this.root = root;
            }

            /// <inheritdoc />
            public override void Apply(IIterativeParseContext context, int position)
            {
                var right = context.ResultStack.Pop();
                var left = context.ResultStack.Pop();

                context.ResultStack.Push(right.Success
                    ? root.Combine(context, left, right)
                    : UnsafeParseResult.Failure(left.Position));
            }
        }
    }
}
