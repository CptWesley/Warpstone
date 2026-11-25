using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Warpstone.Errors;
using Warpstone.Internal;
using Warpstone.Internal.Input;

namespace Warpstone
{
    /// <summary>
    /// Parsing context for iterative parsing.
    /// </summary>
    /// <typeparam name="T">The result type of the parsing.</typeparam>
    public sealed class IterativeParseContext<T> : IParseContext<T>, IIterativeParseContext, ScratchStacks.IOwner
    {
#if NET9_0_OR_GREATER
        private readonly Lock lck = new();
#else
        private readonly object lck = new();
#endif

        private readonly MemoTable memoTable;
        private readonly IReadOnlyMemoTable readOnlyMemoTable;

        private readonly Stack<UnsafeParseResult> resultStack;
        private readonly Stack<(int Position, IParserImplementation Parser)> executionStack;

        private IParseResult<T>? result;
        private ScratchStacks? scratch;

        /// <summary>
        /// Initializes a new instance of the <see cref="IterativeParseContext{T}"/> class.
        /// </summary>
        /// <param name="input">The input to parse.</param>
        /// <param name="parser">The parser to run.</param>
        /// <param name="options">The options used for parsing.</param>
        public IterativeParseContext(IParseInput input, IParser<T> parser, ParseOptions options)
        {
            Parser = parser;
            Input = input;
            Options = options;

            var implementation = parser.GetImplementation(options);
            Failures = FailureTracker.For(parser, options);

            memoTable = new MemoTable();
            readOnlyMemoTable = memoTable.AsReadOnly();

            resultStack = new();
            executionStack = new();
            executionStack.Push((0, implementation));
        }

        /// <inheritdoc />
        public IParser<T> Parser { get; }

        /// <inheritdoc />
        public ParseOptions Options { get; }

        /// <inheritdoc />
        IParser IReadOnlyParseContext.Parser => Parser;

        /// <inheritdoc />
        public IParseResult<T> Result => RunToEnd(default);

        /// <inheritdoc />
        IParseResult IReadOnlyParseContext.Result => Result;

        /// <inheritdoc />
        public IParseInput Input { get; }

        /// <inheritdoc />
        public IReadOnlyMemoTable MemoTable => readOnlyMemoTable;

        /// <inheritdoc />
        public FailureTracker Failures { get; }

        /// <inheritdoc />
        ScratchStacks ScratchStacks.IOwner.Scratch => scratch ??= new();

        /// <inheritdoc />
        IMemoTable IParseContext.MemoTable => memoTable;

        /// <inheritdoc />
        Stack<UnsafeParseResult> IIterativeParseContext.ResultStack => resultStack;

        /// <inheritdoc cref="IIterativeParseContext.ResultStack" />
        public IReadOnlyCollection<UnsafeParseResult> ResultStack => resultStack;

        /// <inheritdoc />
        Stack<(int Position, IParserImplementation Parser)> IIterativeParseContext.ExecutionStack => executionStack;

        /// <inheritdoc cref="IIterativeParseContext.ExecutionStack" />
        public IReadOnlyCollection<(int Position, IParserImplementation Parser)> ExecutionStack => executionStack;

        /// <inheritdoc />
        public bool Done => result is { };

        /// <inheritdoc />
        public IParseResult<T> RunToEnd(CancellationToken cancellationToken)
            => cancellationToken.CanBeCanceled
            ? RunToEndWithCancellation(cancellationToken)
            : RunToEndWithoutCancellation();

        /// <inheritdoc />
        IParseResult IParseContext.RunToEnd(CancellationToken cancellationToken)
            => RunToEnd(cancellationToken);

        private IParseResult<T> RunToEndWithCancellation(CancellationToken cancellationToken)
        {
            if (result is { })
            {
                return result;
            }

            lock (lck)
            {
                while (executionStack.Count > 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    InternalStep();
                }

                result = resultStack.Pop().AsSafe<T>(this);
            }

            return result;
        }

        private IParseResult<T> RunToEndWithoutCancellation()
        {
            if (result is { })
            {
                return result;
            }

            lock (lck)
            {
                while (executionStack.Count > 0)
                {
                    InternalStep();
                }

                result = resultStack.Pop().AsSafe<T>(this);
            }

            return result;
        }

        /// <summary>
        /// Runs the parser to the end, like <see cref="RunToEnd(CancellationToken)"/>,
        /// but awaits more input instead of blocking when the input is read from a <see cref="System.IO.TextReader"/>
        /// or <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The parse result.</returns>
        public async Task<IParseResult<T>> RunToEndAsync(CancellationToken cancellationToken = default)
        {
            if (Input is not TextReaderParseInput reader)
            {
                return RunToEnd(cancellationToken);
            }

            reader.IsAsynchronous = true;
            try
            {
                while (result is null)
                {
                    lock (lck)
                    {
                        while (executionStack.Count > 0 && !reader.IsPending)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            InternalStep();
                        }

                        if (executionStack.Count <= 0 && result is null)
                        {
                            result = resultStack.Pop().AsSafe<T>(this);
                        }
                    }

                    if (reader.IsPending)
                    {
                        await reader.ReadPendingAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            finally
            {
                reader.IsAsynchronous = false;
            }

            return result;
        }

        /// <inheritdoc />
        public bool Step()
        {
            if (result is { })
            {
                return false;
            }

            lock (lck)
            {
                if (result is { })
                {
                    return false;
                }

                InternalStep();

                if (executionStack.Count <= 0)
                {
                    result = resultStack.Pop().AsSafe<T>(this);
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalStep()
        {
            var (pos, cur) = executionStack.Pop();
            cur.Apply(this, pos);
        }
    }
}
