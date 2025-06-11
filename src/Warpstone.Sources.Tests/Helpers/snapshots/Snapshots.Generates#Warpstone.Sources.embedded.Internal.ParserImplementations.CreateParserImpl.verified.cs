//HintName: Warpstone.Sources.embedded.Internal.ParserImplementations.CreateParserImpl.cs
using System.Collections.Generic;
using Warpstone.Internal.ParserExpressions;

namespace Warpstone.Internal.ParserImplementations
{
    /// <summary>
    /// Represents a parser that always passes.
    /// </summary>
    /// <typeparam name="T">The type of the value that is always returned.</typeparam>
    internal sealed class CreateParserImpl<T> : ParserImplementationBase<CreateParser<T>, T>
    {
        private readonly T value;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateParserImpl{T}"/> class.
        /// </summary>
        /// <param name="value">The value that is always returned.</param>
        public CreateParserImpl(T value)
        {
            this.value = value;
        }

        /// <inheritdoc />
        protected override void InitializeInternal(CreateParser<T> parser, IReadOnlyDictionary<IParser, IParserImplementation> parserLookup)
        {
            // Do nothing.
        }

        /// <inheritdoc />
        public override UnsafeParseResult Apply(IRecursiveParseContext context, int position)
        {
            return new(position, 0, value);
        }

        /// <inheritdoc />
        public override void Apply(IIterativeParseContext context, int position)
        {
            context.ResultStack.Push(new(position, 0, value));
        }
    }
}
