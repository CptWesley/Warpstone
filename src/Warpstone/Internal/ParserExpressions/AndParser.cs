using System;
using System.Collections.Generic;
using Warpstone.Internal.ParserImplementations;

namespace Warpstone.Internal.ParserExpressions
{
    /// <summary>
    /// Represents a parser that performs two parse operations sequentially and combines the result.
    /// </summary>
    /// <typeparam name="TFirst">The result type of the <see name="First"/> parser.</typeparam>
    /// <typeparam name="TSecond">The result type of the <see name="Second"/> parser.</typeparam>
    internal sealed class AndParser<TFirst, TSecond> : ParserBase<(TFirst First, TSecond Second)>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AndParser{TFirst,TSecond}"/> class.
        /// </summary>
        /// <param name="first">The parser that is executed first.</param>
        /// <param name="second">The parser that is executed after the first one has succeeded.</param>
        public AndParser(IParser<TFirst> first, IParser<TSecond> second)
        {
            First = first;
            Second = second;
        }

        /// <summary>
        /// The parser that is executed first.
        /// </summary>
        public IParser<TFirst> First { get; }

        /// <summary>
        /// The parser that is executed after the first one has succeeded.
        /// </summary>
        public IParser<TSecond> Second { get; }

        /// <inheritdoc />
        public override IParserImplementation<(TFirst First, TSecond Second)> CreateUninitializedImplementation()
        {
            var tLeft = typeof(TFirst);
            var tRight = typeof(TSecond);

            var leftBoxed = tLeft.IsValueType;
            var rightBoxed = tRight.IsValueType;

            var genericParserType = (leftBoxed, rightBoxed) switch
            {
                (false, false) => typeof(AndRefRefParserImpl<,>),
                (false, true) => typeof(AndRefBoxedParserImpl<,>),
                (true, false) => typeof(AndBoxedRefParserImpl<,>),
                (true, true) => typeof(AndBoxedBoxedParserImpl<,>),
            };

            var parserType = genericParserType.MakeGenericType(tLeft, tRight);
            var parser = (IParserImplementation<(TFirst, TSecond)>)Activator.CreateInstance(parserType, args: null)!;
            return parser;
        }

        /// <inheritdoc />
        protected override void PerformAnalysisStepInternal(IParserAnalysisInfo info, IReadOnlyList<IParser> trace)
        {
            First.PerformAnalysisStep(info, trace);
            Second.PerformAnalysisStep(info, trace);
        }
    }
}
