//HintName: Warpstone.Sources.embedded.Internal.ParserImplementations.AggregateParserImpl.cs
using System;
using System.Collections.Generic;
using Warpstone.Internal.ParserExpressions;

namespace Warpstone.Internal.ParserImplementations
{
    /// <summary>
    /// Represents a parser that parses a repeated series of elements and aggregates the results.
    /// </summary>
    /// <typeparam name="TSource">The element type.</typeparam>
    /// <typeparam name="TAccumulator">The accumulator type.</typeparam>
    internal sealed class AggregateParserImpl<TSource, TAccumulator> : ParserImplementationBase<AggregateParser<TSource, TAccumulator>, TAccumulator>
    {
        private IParserImplementation<TSource> element = default!;
        private IParserImplementation? delimiter = default!;
        private int minCount = default!;
        private int maxCount = default!;
        private Func<TAccumulator> createSeed = default!;
        private Func<TAccumulator, TSource, TAccumulator> accumulate = default!;

        /// <inheritdoc />
        protected override void InitializeInternal(AggregateParser<TSource, TAccumulator> parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
        {
            element = (IParserImplementation<TSource>)parserLookup[parser.Element];
            delimiter = parser.Delimiter is null ? null : parserLookup[parser.Delimiter];
            minCount = parser.MinCount;
            maxCount = parser.MaxCount;
            createSeed = parser.CreateSeed;
            accumulate = parser.Accumulate;
        }

        /// <inheritdoc />
#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
        public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
        {
            var acc = createSeed();
            var min = minCount;
            var max = maxCount;
            var nextPos = position;
            var length = 0;

            var pastFirst = false;

            while (max > 0)
            {
                if (delimiter is { })
                {
                    if (!pastFirst)
                    {
                        pastFirst = true;
                    }
                    else
                    {
                        var delimiterResult = delimiter.Apply(context, nextPos);

                        if (!delimiterResult.Success)
                        {
                            if (min <= 0)
                            {
                                break;
                            }
                            else
                            {
                                return delimiterResult;
                            }
                        }

                        nextPos = delimiterResult.NextPosition;
                    }
                }

                var result = element.Apply(context, nextPos);

                if (!result.Success)
                {
                    if (min <= 0)
                    {
                        break;
                    }
                    else
                    {
                        return result;
                    }
                }

                nextPos = result.NextPosition;
                acc = accumulate(acc, (TSource)result.Value!);
                length = result.NextPosition - position;

                min--;
                max--;
            }

            return new(position, length, acc);
        }

        /// <inheritdoc />
        public override void Apply(IIterativeParseContext context, int position)
        {
            var acc = createSeed();
            context.ExecutionStack.Push((position, new ContinuationElement(
                element: element,
                delimiter: delimiter,
                startPosition: position,
                length: 0,
                minCount: minCount,
                maxCount: maxCount,
                accumulator: acc,
                accumulate: accumulate)));
            context.ExecutionStack.Push((position, element));
        }

        private sealed class ContinuationElement : ContinuationParserImplementationBase
        {
            private readonly IParserImplementation<TSource> element;
            private readonly IParserImplementation? delimiter;
            private readonly int startPosition;
            private readonly int length;
            private readonly int minCount;
            private readonly int maxCount;
            private readonly TAccumulator accumulator;
            private readonly Func<TAccumulator, TSource, TAccumulator> accumulate;

#pragma warning disable S107 // Methods should not have too many parameters
            public ContinuationElement(
                IParserImplementation<TSource> element,
                IParserImplementation? delimiter,
                int startPosition,
                int length,
                int minCount,
                int maxCount,
                TAccumulator accumulator,
                Func<TAccumulator, TSource, TAccumulator> accumulate)
#pragma warning restore S107 // Methods should not have too many parameters
            {
                this.element = element;
                this.delimiter = delimiter;
                this.startPosition = startPosition;
                this.length = length;
                this.minCount = minCount;
                this.maxCount = maxCount;
                this.accumulator = accumulator;
                this.accumulate = accumulate;
            }

            public override void Apply(IIterativeParseContext context, int position)
            {
                var result = context.ResultStack.Pop();

                if (!result.Success)
                {
                    if (minCount <= 0)
                    {
                        context.ResultStack.Push(new(startPosition, length, accumulator));
                    }
                    else
                    {
                        context.ResultStack.Push(result);
                    }

                    return;
                }

                var nextPos = result.NextPosition;
                var newAccumulator = accumulate(accumulator, (TSource)result.Value!);
                var newLength = result.NextPosition - startPosition;

                var newMin = minCount - 1;
                var newMax = maxCount - 1;

                if (newMax <= 0)
                {
                    context.ResultStack.Push(new(startPosition, newLength, newAccumulator));
                    return;
                }

                if (delimiter is { })
                {
                    context.ExecutionStack.Push((nextPos, new ContinuationDelimiter(
                        element: element,
                        delimiter: delimiter,
                        startPosition: startPosition,
                        length: newLength,
                        minCount: newMin,
                        maxCount: newMax,
                        accumulator: newAccumulator,
                        accumulate: accumulate)));
                    context.ExecutionStack.Push((nextPos, delimiter));
                }
                else
                {
                    context.ExecutionStack.Push((nextPos, new ContinuationElement(
                        element: element,
                        delimiter: delimiter,
                        startPosition: startPosition,
                        length: newLength,
                        minCount: newMin,
                        maxCount: newMax,
                        accumulator: newAccumulator,
                        accumulate: accumulate)));
                    context.ExecutionStack.Push((nextPos, element));
                }
            }
        }

        private sealed class ContinuationDelimiter : ContinuationParserImplementationBase
        {
            private readonly IParserImplementation<TSource> element;
            private readonly IParserImplementation? delimiter;
            private readonly int startPosition;
            private readonly int length;
            private readonly int minCount;
            private readonly int maxCount;
            private readonly TAccumulator accumulator;
            private readonly Func<TAccumulator, TSource, TAccumulator> accumulate;

#pragma warning disable S107 // Methods should not have too many parameters
            public ContinuationDelimiter(
                IParserImplementation<TSource> element,
                IParserImplementation? delimiter,
                int startPosition,
                int length,
                int minCount,
                int maxCount,
                TAccumulator accumulator,
                Func<TAccumulator, TSource, TAccumulator> accumulate)
#pragma warning restore S107 // Methods should not have too many parameters
            {
                this.element = element;
                this.delimiter = delimiter;
                this.startPosition = startPosition;
                this.length = length;
                this.minCount = minCount;
                this.maxCount = maxCount;
                this.accumulator = accumulator;
                this.accumulate = accumulate;
            }

            public override void Apply(IIterativeParseContext context, int position)
            {
                var result = context.ResultStack.Pop();

                if (!result.Success)
                {
                    if (minCount <= 0)
                    {
                        context.ResultStack.Push(new(startPosition, length, accumulator));
                    }
                    else
                    {
                        context.ResultStack.Push(result);
                    }

                    return;
                }

                var nextPos = result.NextPosition;
                context.ExecutionStack.Push((nextPos, new ContinuationElement(
                    element: element,
                    delimiter: delimiter,
                    startPosition: startPosition,
                    length: length,
                    minCount: minCount,
                    maxCount: maxCount,
                    accumulator: accumulator,
                    accumulate: accumulate)));
                context.ExecutionStack.Push((nextPos, element));
            }
        }
    }
}
